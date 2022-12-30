using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 1f;
    private Vector2 _movementDirection = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _movementDirection.x = -1f;
            _movementDirection.y = 0f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _movementDirection.x = 1f;
            _movementDirection.y = 0f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _movementDirection.x = 0f;
            _movementDirection.y = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _movementDirection.x = 0f;
            _movementDirection.y = -1f;
        }
        _movementDirection.Normalize();
        _movementDirection *= Time.deltaTime * _movementSpeed; ;
        gameObject.transform.Translate(new Vector3(_movementDirection.x, _movementDirection.y, 0));
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
    }
}
