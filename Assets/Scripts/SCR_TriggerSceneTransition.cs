using RainWard.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainWard.UI;
using UnityEngine.VFX;

public class SCR_TriggerSceneTransition : MonoBehaviour
{
    private Controls input;
    private SCR_CC4 cc;
    private SCR_GameManager gameManager;
    private GameUI gameUI;
    private SCR_AudioManager audManager;
    private AudioSource aud;

    public VisualEffect[] vfx;

    void Start()
    {
        input = GetComponentInParent<SCR_CC4>().input;
        cc = GetComponentInParent<SCR_CC4>();
        aud = GetComponentInParent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<SCR_GameManager>();
        audManager = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    public void TriggerSceneTransition()
    {
        cc.SceneTransition();
    }
    
    public void TriggerRespawn()
    {
        gameManager.OnPlayerDeath();

        if (GameObject.Find("SludgeLand(Clone)"))
        {
            GameObject.Destroy(GameObject.Find("SludgeLand(Clone)"));
        }
        if (GameObject.Find("WaterLanding(Clone)"))
        {
            GameObject.Destroy(GameObject.Find("WaterLanding(Clone)"));
        }
    }

    public void TriggerFade()
    {
        gameUI.OnDeath();
        //Freeze All Movement:
        cc.playerVelocity.x = 0;
        cc.playerVelocity.z = 0;
        cc.playerVelocity.y = 0;
        //Disable Input:
        input.Player.Disable();
        //GetComponentInParent<CharacterController>().enabled = false;

        audManager.PlaySound(audManager.uI[3]);
    }

    public void TriggerJump1SFX()
    {
        aud.PlayOneShot(audManager.player[1]);
    }

    public void TriggerJump2SFX()
    {
        aud.PlayOneShot(audManager.player[2]);
    }
    public void TriggerDashSFX()
    {
        aud.PlayOneShot(audManager.player[0]);
    }
    public void TriggerRunSFX()
    {
        if(cc.isGrounded)
        {
            aud.PlayOneShot(audManager.player[UnityEngine.Random.Range(4, 7)]);
        }
    }

    public void WaterDrownVFX()
    {
        VisualEffect waterSplash = Instantiate(vfx[0], transform.position, vfx[0].transform.rotation) as VisualEffect;
    }

    public void SludgeDrownVFX()
    {
        VisualEffect sludgeSplash = Instantiate(vfx[1], transform.position, vfx[1].transform.rotation) as VisualEffect;
    }
}
