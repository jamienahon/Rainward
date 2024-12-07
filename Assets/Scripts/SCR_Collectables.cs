using RainWard.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainWard.UI;

public class SCR_Collectables : MonoBehaviour
{
    //SCRIPTS:
    private SCR_GameManager gameManager;
    private SCR_AudioManager audioManager;
    private SCR_PersistantData persistantData;
    private GameUI gameUI;

    public void Start()
    {
        //Initialize Scripts:
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SCR_GameManager>();
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        gameManager.GetComponentInChildren<SCR_AudioManager>();
        audioManager = gameManager.GetComponentInChildren<SCR_AudioManager>();
        persistantData = GameObject.Find("PersistantData").GetComponent<SCR_PersistantData>();

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //GUMDROP:
            if (gameObject.tag == "GumDrop")
            {
                audioManager.PlaySound(audioManager.collectibles[Random.Range(0, 3)]);
                gameUI.DisplayGumDropScore();
                gameManager.GumDropScore++;
                Destroy(gameObject);
            }

            //HP MUSHROOM:
            else if (gameObject.tag == "HPMush")
            {

                if(gameManager.currentHP < 3)
                {
                    AudioClip audioClip = audioManager.collectibles[4];
                    audioManager.PlaySound(audioClip);

                    gameManager.SetHP(gameManager.currentHP + 1);
                    Destroy(gameObject);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
