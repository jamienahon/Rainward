using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RainWard.Managers
{
    public class SCR_SceneManagerRW : MonoBehaviour
    {
        public RW_Scenes currentScene;
        public RW_Scenes nextScene;
        public RW_Scenes prevScene;

        SCR_PersistantData persistantData;

        public GameObject player;
        public GameObject cam;

        public float distanceFromBurrow;

        public enum RW_Scenes
        {
            RW_Scene_MainMenu,
            RW_Scene_Cinematic,
            RW_Scene_Tutorial,
            RW_Ryan,
            RW_Tristan,
        }

        void Start()
        {
            persistantData = GameObject.Find("PersistantData").GetComponent<SCR_PersistantData>();
            currentScene = (RW_Scenes)SceneManager.GetActiveScene().buildIndex;
            //DecideSpawnBurrow();
        }

        ////private void OnTriggerEnter(Collider other)
        ////{
        ////    if (other.tag == "Player")
        ////    {
        ////        persistantData.prevScene = SceneManager.GetActiveScene().buildIndex;
        ////        SceneManager.LoadScene((int)nextScene);
        ////    }
        ////}

        //void DecideSpawnBurrow()
        //{
        //    switch(prevScene)
        //    {
        //        case RW_Scenes.RW_Ryan:
        //            SetSpawnPoint(GameObject.FindGameObjectWithTag("WombatHoleEnd"));
        //            break;
        //        case RW_Scenes.RW_Tristan:
        //            SetSpawnPoint(GameObject.FindGameObjectWithTag("WombatHoleStart"));
        //            break;
        //        case RW_Scenes.RW_Scene_Tutorial:
        //            if (currentScene == RW_Scenes.RW_Ryan)
        //                SetSpawnPoint(GameObject.FindGameObjectWithTag("WombatHoleEnd"));
        //            else if (currentScene == RW_Scenes.RW_Tristan)
        //                SetSpawnPoint(GameObject.FindGameObjectWithTag("WombatHoleStart"));
        //            break;
        //    }
        //}

        //void SetSpawnPoint(GameObject burrow)
        //{
        //    Vector3 forwardVector = Quaternion.AngleAxis(0, burrow.transform.up) * -burrow.transform.forward * 5;
        //    Vector3 destination = burrow.transform.position + forwardVector;
        //    player.transform.position = destination;
        //    player.transform.forward = -burrow.transform.forward;
        //    GetComponent<Camera>().GetComponent<CinemachineFreeLook>().m_XAxis.Value = player.transform.rotation.y;
        //}
    }
}