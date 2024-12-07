using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SpawnLogs : MonoBehaviour
{
    [System.NonSerialized] public Animator anim;
    public GameObject[] logs;

    private GameObject currLog;
    private GameObject nextLog;
    private int logCount;

    public void Start()
    {
        anim = GetComponent<Animator>();
        currLog = logs[0];
        logCount = 0;

    }

    public void StartANIM(string animation)
    {
        logCount++;

        nextLog = logs[logCount];
        Debug.Log(nextLog);
        nextLog.GetComponent<Animator>().Play(animation);

        if (logCount == 2)
        {
            logCount = 0;
        }
    }
}
