using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Billboard : MonoBehaviour
{
    void Update()
    {
        if(Camera.main.gameObject.activeInHierarchy && Camera.main != null)
        {
            Vector3 v = Camera.main.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(Camera.main.transform.position - v);
        }
        else
        {
            return;
        }
            
    }
}
