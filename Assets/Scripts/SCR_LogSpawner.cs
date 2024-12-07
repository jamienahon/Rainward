using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LogSpawner : MonoBehaviour
{
    public GameObject[] Logs;

    [SerializeField]
    private float spawnTimer;

    [SerializeField]
    private int maxLogs;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject log in Logs)
        {
            log.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0)
        {
            foreach (GameObject log in Logs)
            {
                log.SetActive(true);
            }
            enabled = false;
        }
    }
}
