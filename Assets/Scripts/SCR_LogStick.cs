using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LogStick : MonoBehaviour
{
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.transform.parent = null;
        }
    }
}
