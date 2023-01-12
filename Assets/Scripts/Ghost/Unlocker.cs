using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    [SerializeField] private List<GameObject> ghosts = new List<GameObject>();
    [SerializeField] private float ghostUnlockTimer = 30f;
    private float timer = 0f;
    private int ghostsUnlocked = 0;

    void Start()
    {
        foreach (GameObject ghost in ghosts)
        {
            ghost.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = ghostUnlockTimer;
            ghostsUnlocked++;
            for (int i = 0; i < ghosts.Count; i++)
            {
                if (i < ghostsUnlocked)
                {
                    ghosts[i].SetActive(true);
                }
            }
        }
    }
}
