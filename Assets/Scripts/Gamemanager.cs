using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    private void Start()
    {
        instance = this;
    }

    public static int Score = 0;
    public static bool Powered = false;
    private const float _maxPowerupDuration = 5f;
    private float _poweredDuration = 0f;

    public void CollectPowerup()
    {
        _poweredDuration = 5f;
        Score += 500;
    }
    public void CollectFood()
    {
        Score += 100;
    }
    private void Update()
    {
        if (_poweredDuration >= 0f)
        {
            Powered = true;
            _poweredDuration -= Time.deltaTime;
        }
        else
        {
            Powered = false;
        }
    }
}
