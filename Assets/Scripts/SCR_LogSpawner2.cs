using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LogSpawner2 : MonoBehaviour
{
    public GameObject[] logs;
    public GameObject logPrefab;

    [SerializeField]
    private float spawnTimer;

    [SerializeField]
    private int maxLogs;

    [SerializeField]
    private Vector3 despawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        logs=new GameObject[maxLogs];
    }

    // Update is called once per frame
    void Update()
    {
        float timer = spawnTimer;
        timer -= Time.deltaTime;
        if(timer >= 0)
        {
            SpawnLog();
        }

        foreach(GameObject log in logs)
        {
            if(log.transform.position == despawnPosition)
            {
                DespawnLog(log);
            }
        }
    }

    void SpawnLog()
    {
        for (int i = 0; i < logs.Length; i++)
        {
            if (logs[i] != null)
            {
                GameObject spawnedLog = Instantiate(logPrefab, transform.position, logPrefab.transform.rotation);
                logs[i] = spawnedLog;
                break;
            }
        }
    }

    void DespawnLog(GameObject despawningLog)
    {
        for (int i = 0; i < logs.Length; i++)
        {
            if (logs[i] == despawningLog)
            {
                Destroy(logs[i]);
                logs[i] = null;
                break;
            }
        }
    }
}
