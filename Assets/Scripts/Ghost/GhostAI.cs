using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GhostAI : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private List<Tile> _tiles = new List<Tile>();
    private TileManager _manager;
    private Tile _currentTile;

    public GhostMove _ghost;
    public Tile _nextTile = null;
    public Tile _targetTile;

    private void Awake()
    {
        _manager = FindObjectOfType<TileManager>();
        _tiles = TileManager.tiles;
    }
    public void AILogic()
    {
        CalculateCurrentNextTile();
        _targetTile = GetTargetTilePerGhost();

        if (_nextTile.occupied || _currentTile.isIntersection)
        {
            //If next tile is wall
            if (_nextTile.occupied && !_currentTile.isIntersection)
            {
                //If ghost is moving horizontal
                if (_ghost.Direction.x != 0)
                {
                    //Go up if down is not available
                    if (_currentTile.down == null)
                    {
                        _ghost.Direction = Vector3.up;
                    }
                    else
                    {
                        _ghost.Direction = Vector3.down;
                    }
                }
                //If ghost is moving vertical
                else if (_ghost.Direction.y != 0)
                {
                    //Go right if left is not available
                    if (_currentTile.left == null)
                    {
                        _ghost.Direction = Vector3.right;
                    }
                    else
                    {
                        _ghost.Direction = Vector3.left;
                    }
                }
            }

            if (_currentTile.isIntersection)
            {
                //Calculate the distance to the target from each available tile and choose te shortest one
                float dist1, dist2, dist3, dist4;
                dist1 = dist2 = dist3 = dist4 = 999999f;
                if (_currentTile.up != null && !_currentTile.up.occupied && !(_ghost.Direction.y < 0)) dist1 = _manager.distance(_currentTile.up, _targetTile);
                if (_currentTile.down != null && !_currentTile.down.occupied && !(_ghost.Direction.y > 0)) dist2 = _manager.distance(_currentTile.down, _targetTile);
                if (_currentTile.left != null && !_currentTile.left.occupied && !(_ghost.Direction.x > 0)) dist3 = _manager.distance(_currentTile.left, _targetTile);
                if (_currentTile.right != null && !_currentTile.right.occupied && !(_ghost.Direction.x < 0)) dist4 = _manager.distance(_currentTile.right, _targetTile);

                float min = Mathf.Min(dist1, dist2, dist3, dist4);
                if (min == dist1) _ghost.Direction = Vector3.up;
                if (min == dist2) _ghost.Direction = Vector3.down;
                if (min == dist3) _ghost.Direction = Vector3.left;
                if (min == dist4) _ghost.Direction = Vector3.right;
            }
        }
        else
        {
            //Setter will update the direction
            _ghost.Direction = _ghost.Direction;
        }
    }
    public void RunLogic()
    {
        CalculateCurrentNextTile();
        if (_nextTile.occupied || _currentTile.isIntersection)
        {
            //If next tile is wall
            if (_nextTile.occupied && !_currentTile.isIntersection)
            {
                //If ghost is moving horizontal
                if (_ghost.Direction.x != 0)
                {
                    //Go up if down is not available
                    if (_currentTile.down == null)
                    {
                        _ghost.Direction = Vector3.up;
                    }
                    else
                    {
                        _ghost.Direction = Vector3.down;
                    }
                }
                //If ghost is moving vertical
                else if (_ghost.Direction.y != 0)
                {
                    //Go right if left is not available
                    if (_currentTile.left == null)
                    {
                        _ghost.Direction = Vector3.right;
                    }
                    else
                    {
                        _ghost.Direction = Vector3.left;
                    }
                }
            }

            if (_currentTile.isIntersection)
            {
                //Choose random available option
                List<Tile> availableTiles = new List<Tile>();
                Tile chosenTile;
                if (_currentTile.up != null && !_currentTile.up.occupied && !(_ghost.Direction.y < 0)) availableTiles.Add(_currentTile.up);
                if (_currentTile.down != null && !_currentTile.down.occupied && !(_ghost.Direction.y > 0)) availableTiles.Add(_currentTile.down);
                if (_currentTile.left != null && !_currentTile.left.occupied && !(_ghost.Direction.x > 0)) availableTiles.Add(_currentTile.left);
                if (_currentTile.right != null && !_currentTile.right.occupied && !(_ghost.Direction.x < 0)) availableTiles.Add(_currentTile.right);

                int rand = Random.Range(0, availableTiles.Count);
                chosenTile = availableTiles[rand];
                _ghost.Direction = Vector3.Normalize(new Vector3(chosenTile.x - _currentTile.x, chosenTile.y - _currentTile.y, 0));
            }
        }
        else
        {
            //Setter will update the direction
            _ghost.Direction = _ghost.Direction;
        }
    }
    private void CalculateCurrentNextTile()
    {
        //Get current tile
        Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
        _currentTile = _tiles[_manager.Index((int)currentPos.x, (int)currentPos.y)];

        //Get the next tile by using the direction
        if (_ghost.Direction.x > 0) _nextTile = _tiles[_manager.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        if (_ghost.Direction.x < 0) _nextTile = _tiles[_manager.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        if (_ghost.Direction.y > 0) _nextTile = _tiles[_manager.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        if (_ghost.Direction.y < 0) _nextTile = _tiles[_manager.Index((int)currentPos.x, (int)(currentPos.y - 1))];
    }
    private Tile GetTargetTilePerGhost()
    {
        Vector3 targetPos;
        Tile targetTile;
        Vector3 dir;

        // get the target tile position (round it down to int so we can reach with Index() function)
        switch (name.ToLower())
        {
            case "blinky":  // target = pacman
                targetPos = new Vector3(_target.position.x + 0.499f, _target.position.y + 0.499f);
                targetTile = _tiles[_manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
            case "pinky":   // target = pacman + 4*pacman's direction (4 steps ahead of pacman)
                dir = _target.GetComponent<PlayerController>().Direction;
                targetPos = new Vector3(_target.position.x + 0.499f, _target.position.y + 0.499f) + 4 * dir;

                // if pacmans going up, not 4 ahead but 4 up and 4 left is the target
                // read about it here: http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
                // so subtract 4 from X coord from target position
                if (dir == Vector3.up) targetPos -= new Vector3(4, 0, 0);

                targetTile = _tiles[_manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
            case "inky":    // target = ambushVector(pacman+2 - blinky) added to pacman+2
                dir = _target.GetComponent<PlayerController>().Direction;
                Vector3 blinkyPos = GameObject.Find("blinky").transform.position;
                Vector3 ambushVector = _target.position + 2 * dir - blinkyPos;
                targetPos = new Vector3(_target.position.x + 0.499f, _target.position.y + 0.499f) + 2 * dir + ambushVector;
                targetTile = _tiles[_manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
            case "clyde":
                targetPos = new Vector3(_target.position.x + 0.499f, _target.position.y + 0.499f);
                targetTile = _tiles[_manager.Index((int)targetPos.x, (int)targetPos.y)];
                if (_manager.distance(targetTile, _currentTile) < 9)
                    targetTile = _tiles[_manager.Index(0, 2)];
                break;
            default:
                targetTile = null;
                Debug.Log("TARGET TILE NOT ASSIGNED");
                break;

        }
        return targetTile;
    }
}