using RainWard.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using RainWard.Managers;
using Cinemachine;

public class SCR_GameManager : MonoBehaviour
{
    private Controls input;
    [SerializeField] private Managers managers;

    [Serializable]
    public struct Managers
    {
        public SCR_SceneManagerRW sceneManager;
        public SCR_AudioManager audioManager;
    }

    //JAMIE CODE: Locate CC Script
    public SCR_CC4 cc;

    [Space]
    [SerializeAs("UI Elements:")] public UIObjects uiObjects;

    [Serializable]
    public struct UIObjects
    {
        public GameUI gameUI;
    }

    [Header("Collectables:")]
    [Space]

    [SerializeField] private int _gumDropScore = 0;
    public int GumDropScore
    {
        get => _gumDropScore;
        set
        {
            _gumDropScore = value;

            if (uiObjects.gameUI.uIText[5] != null)
            {
                uiObjects.gameUI.uIText[5].text = value.ToString();
                uiObjects.gameUI.scoreChanged = true;
            }
        }
    }

    [SerializeField] private int _sapTapperScore = 0;
    public int SapTapperScore
    {
        get => _sapTapperScore;
        set
        {
            _sapTapperScore = value;

            if (uiObjects.gameUI.uIText[4] != null)
            {
                uiObjects.gameUI.uIText[4].text = value.ToString();
            }
        }
    }

    //JAMIE CODE: Set (+/-) HP UI
    [Header("Player Resources:")]
    [Space]

    public int totalHP = 3;
    [System.NonSerialized] public int currentHP;


    private void Awake()
    {
        input = new Controls();
    }

    private void Start()
    {
        if (managers.audioManager == null)
        {
            managers.audioManager = GetComponentInChildren<SCR_AudioManager>();
            if (managers.audioManager == null)
                Debug.Log("Failed to get AudioManager component from children");
        }

        currentHP = totalHP;
        Debug.Log(currentHP);

    }

    public void SetHP(int value)
    {
        //Lose HP:
        if (currentHP <= 3)
        {
            currentHP = value;
            Debug.Log(currentHP);

            if (uiObjects.gameUI.uIElements[4] != null)
            {

                if (currentHP == 3)
                {
                    uiObjects.gameUI.uIImages[0].enabled = true;
                    uiObjects.gameUI.uIImages[1].enabled = true;
                    uiObjects.gameUI.uIImages[2].enabled = true;
                }
                else if (currentHP == 2)
                {
                    uiObjects.gameUI.uIImages[0].enabled = true;
                    uiObjects.gameUI.uIImages[1].enabled = true;
                    uiObjects.gameUI.uIImages[2].enabled = false;
                }
                else if (currentHP == 1)
                {
                    uiObjects.gameUI.uIImages[0].enabled = true;
                    uiObjects.gameUI.uIImages[1].enabled = false;
                    uiObjects.gameUI.uIImages[2].enabled = false;
                }
                else if (currentHP == 0)
                {
                    uiObjects.gameUI.uIImages[0].enabled = false;
                    uiObjects.gameUI.uIImages[1].enabled = false;
                    uiObjects.gameUI.uIImages[2].enabled = false;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void OnPlayerDeath()
    {
        PlayerRespawn(GameObject.FindGameObjectWithTag("Player"));
    }

    public void PlayerDrown()
    {
        PlayerRespawn(GameObject.FindGameObjectWithTag("Player"));
    }

    public void PlayerRespawn(GameObject player)
    {
        //player.GetComponent<CharacterController>().enabled = false;

        //GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        //foreach(GameObject checkpoint in checkpoints)
        //{
        //    checkpoint.GetComponent<SCR_Checkpoint>().
        //}

        GameObject cp = GameObject.FindGameObjectWithTag("ActiveCheckPoint");
        if (cp)
        {
            Debug.Log("Checkpoint respawn");
            player.transform.position = cp.GetComponent<SCR_Checkpoint>().Spawnpoint;
        }
        else
        {
            Debug.Log("No checkpoint respawn");
            player.transform.position = player.GetComponent<SCR_CC4>().LevelPosition;
        }

        //player.GetComponent<CharacterController>().enabled = true;
        Debug.Log(player.GetComponent<CharacterController>().enabled);
        input.Player.Enable();
        player.GetComponent<SCR_CC4>().anim.Play("ANIM_Idle");
        player.GetComponent<SCR_CC4>().isDead = false;
        player.GetComponent<SCR_CC4>().isDrowning = false;
        GameObject.FindGameObjectWithTag("MainCinCam").GetComponent<CinemachineFreeLook>().m_XAxis.Value = cc.transform.rotation.y;

        SetHP(totalHP);
    }
}

