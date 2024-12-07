using RainWard.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Hazards : MonoBehaviour
{
    [Header("Script References:")]
    [Space]
    public SCR_GameManager gameManager;
    public SCR_AudioManager audioManager;
    public SCR_CC4 cC;
    private SCR_TriggerSceneTransition playerAnims;

    private void Start()
    {
        playerAnims = GameObject.Find("Thorn_Beta").GetComponent<SCR_TriggerSceneTransition>();
    }

    //Hazard Collision:
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Player Detection (Machine Hazards)
            if (gameObject.tag == "HazardPlayerDetection" && !cC.IsDazed)
            {
                GetComponent<Animator>().SetBool("isActive", true);
            }

            //Machine Hazards:
            if (gameObject.tag == "MachineHazard" && !cC.IsDazed)
            {
                Debug.Log("Ouch!");

                gameManager.SetHP(gameManager.currentHP - 1);

                if (gameManager.currentHP > 0)
                {
                    Vector3 toPlayer = other.transform.position - transform.position;
                    toPlayer.y = 0;
                    toPlayer.Normalize();
                    //if less than 0, player is on the back side of the blade
                    bool frontSideOfBlade = Vector3.Dot(toPlayer, transform.right) > 0;

                    Vector3 throwDirection = frontSideOfBlade ? -transform.forward : transform.forward;

                    other.GetComponent<SCR_CC4>().DamagePlayer(throwDirection);

                    cC.OnDamaged();
                }
                else
                {
                    if(!cC.IsDead)
                    {
                        cC.OnDeath();
                    }
                    
                }

                if(!cC.isDead)
                {
                    GetComponent<AudioSource>().PlayOneShot(audioManager.misc[2]);
                }
                
            }
            else if (gameObject.tag == "Water" && !cC.isDrowning)
            {
                cC.OnDrown();
                audioManager.PlaySound(audioManager.cinematicSFX[2]);
                playerAnims.WaterDrownVFX();
            }
            else if (gameObject.tag == "Sludge" && !cC.isDrowning)
            {
                cC.OnDrown();
                audioManager.PlaySound(audioManager.cinematicSFX[3]);
                playerAnims.SludgeDrownVFX();
            }

            if (gameObject.tag == "Abyss")
            {
                if (!cC.IsDead)
                {
                    cC.OnDeath();
                }
            }
        }
        else if (other.tag == "EnemyType")
        {
            if (gameObject.tag == "MachineHazard")
            {
                other.GetComponent<SCR_EnemyAI>().Death();
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.tag == "Water" || gameObject.tag == "Sludge")
            {
                cC.OnDrown();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            //Player Detection (Machine Hazards)
            if (gameObject.tag == "HazardPlayerDetection")
            {
                GetComponent<Animator>().SetBool("isActive", true);
            }
            else if (gameObject.tag == "Water" || gameObject.tag == "Sludge")
            {
                cC.isDrowning = false;
            }
        }
    }

    public void ResetBlade()
    {
        GetComponentInParent<Animator>().SetBool("isActive", false);
        GetComponentInChildren<AudioSource>().Stop();

    }

    public void BladeSwingSFX()
    {
        GetComponentInChildren<AudioSource>().clip = audioManager.misc[1];
        GetComponentInChildren<AudioSource>().Play();
    }
}
