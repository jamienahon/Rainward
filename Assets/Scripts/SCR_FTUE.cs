using RainWard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_FTUE : MonoBehaviour
{
    [System.NonSerialized] public GameUI gameUI;

    void Start()
    {
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            if (other.tag == "Player")
            {
                if(gameObject.tag == "HPMush")
                {
                    //Disables FTUE Script on all BouncePads in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("HP").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    gameUI.DisplayTooltip(0, other.gameObject);
                }
                else if (gameObject.tag == "FTUE_BouncePad")
                {
                    //Disables FTUE Script on all BouncePads in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("BouncePads").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    //Disables TriggerColliders on all BouncePads in Scene:
                    foreach (CapsuleCollider collider in GameObject.Find("BouncePads").GetComponentsInChildren <CapsuleCollider>())
                    {
                        collider.enabled = false;
                    }

                    gameUI.DisplayTooltip(1, other.gameObject);
                }
                else if (gameObject.tag == "FTUE_Bogwarts")
                {
                    //Disables FTUE Script on all Bogwarts in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("Enemies").GetComponentsInChildren<SCR_FTUE>())
                    {
                        if(scr.gameObject.tag != "EnemyType2")
                        {
                            scr.enabled = false;
                        }
                    }

                    //Disables FTUE Colliders on all Bogwarts in Scene:
                    foreach (SphereCollider collider in GameObject.Find("Enemies").GetComponentsInChildren<SphereCollider>())
                    {
                        if (collider.gameObject.name != "Hammer" && collider.gameObject.name != "Hatchet" && collider.gameObject.name != "Hand_3_L" && collider.gameObject.name != "Hand_4_R"
                            && collider.gameObject.name != "FTUE_MechToads")
                        {
                            collider.enabled = false;
                        }
                    }

                    gameUI.DisplayTooltip(2, other.gameObject);
                }
                else if (gameObject.tag == "FTUE_SapTapper")
                {
                    //Disables FTUE Script on all SapTappers in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("SapTappers").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    //Disables FTUE Colliders on all SapTappers in Scene:
                    foreach (Collider collider in GameObject.Find("FTUE_SapTapper").GetComponents<Collider>())
                    {
                        collider.enabled = false;
                    }

                    gameUI.DisplayTooltip(3, other.gameObject);
                }
                else if (gameObject.tag == "GumDrop")
                {
                    //Disables FTUE Script on all GumDrops in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("GumDrops").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    gameUI.DisplayTooltip(4, other.gameObject);
                }
                else if (gameObject.tag == "FTUE_MechToads")
                {
                    gameUI.DisplayTooltip(5, other.gameObject);

                    //Disables FTUE Script on all GumDrops in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("MechToad").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }
                }
                else if (gameObject == GameObject.Find("CheckpointRadius"))
                {
                    //Disables FTUE Script on all GumDrops in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("Checkpoints").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    gameUI.DisplayTooltip(6, other.gameObject);
                }
                else if (gameObject.tag == ("HazardPlayerDetection"))
                {
                    //Disables FTUE Script on all GumDrops in Scene:
                    foreach (SCR_FTUE scr in GameObject.Find("Machinery").GetComponentsInChildren<SCR_FTUE>())
                    {
                        scr.enabled = false;
                    }

                    gameUI.DisplayTooltip(7, other.gameObject);
                }
            }
        }

    }


    void Update()
    {
        
    }
}
