using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GhostMove : MonoBehaviour
{
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private bool _flee = false;

    private GhostAI ghostAI;
    private Transform playerTransform;
    private List<Vector2> checkpoints;
    private List<Vector2> openTiles = new List<Vector2>();

    private DateTime nextUpdate = DateTime.Now;
    private const float updateInterval = 1000f;

    private void Start()
    {
        ghostAI = FindObjectOfType<GhostAI>();
        playerTransform = FindObjectOfType<PlayerController>().transform;

        foreach (Tile tile in TileManager.tiles)
        {
            if (tile.occupied == false)
            {
                openTiles.Add(new Vector2(tile.x, tile.y));
            }
        }
    }
    private void Update()
    {
        if (_flee)
        {
            UpdateFlee();
        }
        else
        {
            UpdateChase();
        }
    }
    private void UpdateFlee()
    {
        if (nextUpdate <= DateTime.Now)
        {
            nextUpdate = DateTime.Now.AddMilliseconds(updateInterval);
            checkpoints = ghostAI.CalculatePath(new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f), GetFleePosition());
        }
        UpdateCheckPoints();
    }
    private void UpdateChase()
    {
        if ((playerTransform.position - transform.position).sqrMagnitude < 0.1f)
        {
            return;
        }
        if (nextUpdate <= DateTime.Now)
        {
            nextUpdate = DateTime.Now.AddMilliseconds(updateInterval);
            checkpoints = ghostAI.CalculatePath(new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f), GetTargetTilePerGhost());
        }
        UpdateCheckPoints();
    }
    private void UpdateCheckPoints()
    {
        if (checkpoints.Count > 0)
        {
            if ((checkpoints[0] - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude < 0.0001f)
            {
                checkpoints.RemoveAt(0);
            }
            if (checkpoints.Count == 0)
            {
                nextUpdate = DateTime.Now;
                return;
            }
            Vector2 p = Vector2.MoveTowards(transform.position, checkpoints[0], _speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        else
        {
            checkpoints = ghostAI.CalculatePath(new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f), new Vector3(playerTransform.position.x + 0.499f, playerTransform.position.y + 0.499f));
        }
    }
    private Vector3 GetFleePosition()
    {
        Vector3 finalPosition = Vector3.zero;
        Vector2 playerPosition = new Vector2(playerTransform.position.x + 0.499f, playerTransform.position.y + +0.499f);
        
        float distanceToPlayer = 0f;
        foreach (Vector2 fleePos in openTiles)
        {
            if ((fleePos-playerPosition).sqrMagnitude > distanceToPlayer)
            {
                distanceToPlayer = (fleePos - playerPosition).sqrMagnitude;
                finalPosition = fleePos;
            }
        }
        return finalPosition;
    }
    Vector3 GetTargetTilePerGhost()
    {
        Vector3 targetPos = Vector3.zero;
        Vector3 dir;

        switch (name.ToLower())
        {
            //Targets player
            case "blinky":
                targetPos = new Vector3(playerTransform.position.x + 0.499f, playerTransform.position.y + 0.499f);
                break;
            case "pinky":   // target = pacman + 4*pacman's direction (4 steps ahead of pacman)
                dir = playerTransform.GetComponent<PlayerController>().Direction;
                targetPos = new Vector3(playerTransform.position.x + 0.499f, playerTransform.position.y + 0.499f) + 4 * dir;

                // if pacmans going up, not 4 ahead but 4 up and 4 left is the target
                // read about it here: http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
                // so subtract 4 from X coord from target position
                if (dir == Vector3.up) targetPos -= new Vector3(4, 0, 0);

                targetPos = FindNearestTile(targetPos);

                break;
            case "inky":    // target = ambushVector(pacman+2 - blinky) added to pacman+2
                dir = playerTransform.GetComponent<PlayerController>().Direction;
                Vector3 blinkyPos = GameObject.Find("blinky").transform.position;
                Vector3 ambushVector = playerTransform.position + 2 * dir - blinkyPos;
                targetPos = new Vector3(playerTransform.position.x + 0.499f, playerTransform.position.y + 0.499f) + 2 * dir + ambushVector;
                
                break;
            case "clyde":
                break;

        }
        return targetPos;
    }
    Vector3 FindNearestTile(Vector2 pos)
    {
        Vector2 finalPosition = Vector2.zero;

        float distanceToPlayer = 100000f;
        foreach (Vector2 tilePos in openTiles)
        {
            if ((tilePos - pos).sqrMagnitude < distanceToPlayer)
            {
                distanceToPlayer = (tilePos - pos).sqrMagnitude;
                finalPosition = tilePos;
            }
        }
        return finalPosition;
    }
}