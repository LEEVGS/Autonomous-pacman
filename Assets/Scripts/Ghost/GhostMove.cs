using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private bool _flee = false;

    private GhostAI ghostAI;
    private Transform playerTransform;
    private List<Vector2> checkpoints;

    private DateTime nextUpdate = DateTime.Now;
    private const float updateInterval = 1000f;

    private void Start()
    {
        ghostAI = FindObjectOfType<GhostAI>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
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
            checkpoints = ghostAI.CalculatePath(new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f), new Vector3(playerTransform.position.x + 0.499f, playerTransform.position.y + 0.499f));
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
    }
    private Vector3 GetFleePosition()
    {
        Vector3 finalPosition = Vector3.zero;
        Vector2 playerPosition = new Vector2(playerTransform.position.x + 0.499f, playerTransform.position.y + +0.499f);
        List<Vector2> fleeTiles = new List<Vector2>();
        foreach (Tile tile in TileManager.tiles)
        {
            if (tile.occupied == false)
            {
                fleeTiles.Add(new Vector2(tile.x, tile.y));
            }
        }
        float distanceToPlayer = 0f;
        foreach (Vector2 fleePos in fleeTiles)
        {
            if ((fleePos-playerPosition).sqrMagnitude > distanceToPlayer)
            {
                distanceToPlayer = (fleePos - playerPosition).sqrMagnitude;
                finalPosition = fleePos;
            }
        }
        return finalPosition;
    }
}