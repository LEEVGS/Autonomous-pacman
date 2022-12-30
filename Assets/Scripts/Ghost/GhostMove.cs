using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    [SerializeField] private float _speed = 0.07f;
    private Vector3 waypoint;
    private Queue<Vector3> waypoints;

    private Vector3 _direction;
    private Vector3 _startPos;

    enum State { Wait, Init, Scatter, Chase, Run };
    State state;

    public float scatterLength = 5f;
    public float waitLength = 0.0f;
    private float timeToEndScatter;
    private float timeToEndWait;

    private float _timeToWhite;
    private float _timeToToggleWhite;
    private float _toggleInterval;
    private bool isWhite = false;

    public Vector3 Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            Vector3 pos = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
            waypoint = pos + _direction;
        }
    }

    private void Start()
    {
        _startPos = transform.position;
        waypoint = transform.position;
        state = State.Wait;
        timeToEndWait = Time.time + waitLength;
        InitializeWaypoints(state);

    }
    private void InitializeWaypoints(State st)
    {
        //--------------------------------------------------
        // File Format: Init and Scatter coordinates separated by empty line
        // Init X,Y 
        // Init X,Y
        // 
        // Scatter X,Y
        // Scatter X,Y

        //Read from file
        string data = "";
        switch (name.ToLower())
        {
            case "blinky":
                data = @"22 20
22 26

27 26
27 30
22 30
22 26";
                break;
            case "pinky":
                data = @"14.5 17
14 17
14 20
7 20

7 26
7 30
2 30
2 26";
                break;
            case "inky":
                data = @"16.5 17
15 17
15 20
22 20

22 8
19 8
19 5
16 5
16 2
27 2
27 5
22 5";
                break;
            case "clyde":
                data = @"12.5 17
14 17
14 20
7 20

7 8
7 5
2 5
2 2
13 2
13 5
10 5
10 8";
                break;

        }

        string line;
        waypoints = new Queue<Vector3>();
        Vector3 waypoint;

        if (st == State.Init)
        {
            using (StringReader reader = new StringReader(data))
            {
                //Loop over all lines
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0) break;

                    string[] values = line.Split(' ');
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);

                    waypoint = new Vector3(x, y, 0);
                    waypoints.Enqueue(waypoint);
                }
            }
        }

        if (st == State.Scatter)
        {
            //Skips until there is an empty line
            bool isScatterWaypoints = false;

            using (StringReader reader = new StringReader(data))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    //Skip till empty line
                    if (line.Length == 0)
                    {
                        isScatterWaypoints = true;
                        continue;
                    }

                    if (isScatterWaypoints)
                    {
                        string[] values = line.Split(' ');
                        int x = Int32.Parse(values[0]);
                        int y = Int32.Parse(values[1]);

                        waypoint = new Vector3(x, y, 0);
                        waypoints.Enqueue(waypoint);
                    }
                }
            }
        }

        // if in wait state, patrol vertically
        if (st == State.Wait)
        {
            Vector3 pos = transform.position;

            // inky and clyde start going down and then up
            if (transform.name == "inky" || transform.name == "clyde")
            {
                waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
                waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
            }
            // while pinky start going up and then down
            else
            {
                waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
                waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
            }
        }
    }
    void Init()
    {
        _timeToWhite = 0;

        // if the Queue is cleared, do some clean up and change the state
        if (waypoints.Count == 0)
        {
            state = State.Scatter;

            //get direction according to sprite name
            string name = GetComponent<SpriteRenderer>().sprite.name;
            if (name[name.Length - 1] == '0' || name[name.Length - 1] == '1') Direction = Vector3.right;
            if (name[name.Length - 1] == '2' || name[name.Length - 1] == '3') Direction = Vector3.left;
            if (name[name.Length - 1] == '4' || name[name.Length - 1] == '5') Direction = Vector3.up;
            if (name[name.Length - 1] == '6' || name[name.Length - 1] == '7') Direction = Vector3.down;

            InitializeWaypoints(state);
            timeToEndScatter = Time.time + scatterLength;

            return;
        }

        // get the next waypoint and move towards it
        MoveToWaypoint();
    }
    void Scatter()
    {
        if (Time.time >= timeToEndScatter)
        {
            waypoints.Clear();
            state = State.Chase;
            return;
        }

        // get the next waypoint and move towards it
        MoveToWaypoint(true);

    }

    void ChaseAI()
    {

        // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, _speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        // if at waypoint, run AI module
        else GetComponent<GhostAI>().AILogic();

    }

    void RunAway()
    {
        GetComponent<Animator>().SetBool("Run", true);

        if (Time.time >= _timeToWhite && Time.time >= _timeToToggleWhite) ToggleBlueWhite();

        // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, _speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        // if at waypoint, run AI run away logic
        else GetComponent<GhostAI>().RunLogic();

    }

    //Utility functions
    void MoveToWaypoint(bool loop = false)
    {
        waypoint = waypoints.Peek();		// get the waypoint (CHECK NULL?)
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)    // if its not reached
        {                                                           // move towards it
            _direction = Vector3.Normalize(waypoint - transform.position);  // dont screw up waypoint by calling public setter
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, _speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        else    // if waypoint is reached, remove it from the queue
        {
            if (loop) waypoints.Enqueue(waypoints.Dequeue());
            else waypoints.Dequeue();
        }
    }

    public void Frighten()
    {
        state = State.Run;
        _direction *= -1;

        _timeToWhite = Time.time + 10f * 0.66f; //TODO GM difficulty
        _timeToToggleWhite = _timeToWhite;
    }

    public void Calm()
    {
        // if the ghost is not running, do nothing
        if (state != State.Run) return;

        waypoints.Clear();
        state = State.Chase;
        _timeToToggleWhite = 0;
        _timeToWhite = 0;
    }

    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        _timeToToggleWhite = Time.time + _toggleInterval;
    }
}
