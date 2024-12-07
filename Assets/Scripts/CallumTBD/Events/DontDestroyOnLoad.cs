using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainWard
{
    /// <summary>
    /// Keeps the GameObject alive during a scene transition
    /// </summary>
    /// <remarks>
    /// Deletes any duplicate objects
    /// </remarks>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            // Get all GameObjects with the same tag
            GameObject[] objs = GameObject.FindGameObjectsWithTag(this.tag);

            if (objs.Length > 1)
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }
    }
}
