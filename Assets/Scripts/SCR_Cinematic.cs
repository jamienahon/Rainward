using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RainWard.Managers;
using RainWard.UI;

public class SCR_Cinematic : MonoBehaviour
{

    [System.NonSerialized] public Animator anim;
    [System.NonSerialized] public SCR_AudioManager aud;
    [System.NonSerialized] public GameUI gameUI;

    public Animator playerAnim;
    public Animator uIAnim;

    //PLACE IN ORDER OF SHOT SEQUENCE:
    public GameObject[] cams;
    public GameObject[] cinObjects;
    public GameObject[] cinFX;

    private int camNo;

    private GameObject currCam;
    private GameObject nextCam;

    private bool fadeOut = false;


    public void Start()
    {
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        aud = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
        anim = GetComponent<Animator>();
        currCam = cams[0];
        camNo = 0;
    }

    public void Update()
    {
        if(GameObject.Find("BigSapTapper").GetComponent<AudioSource>().volume > 0.1 && fadeOut)
        {
            GameObject.Find("BigSapTapper").GetComponent<AudioSource>().volume -= 0.25f * Time.deltaTime;
        }

    }

    public void ChangeCam()
    {
        camNo++;
        nextCam = cams[camNo];
        nextCam.GetComponent<CinemachineVirtualCamera>().Priority = currCam.GetComponent<CinemachineVirtualCamera>().Priority + 1;
        currCam = nextCam;

        //if(camNo == 2)
        //{
        //    GameObject.Find("Sunglasses").SetActive(false);
        //}

    }

    public void CamFade()
    {
        //FADE-OUT BLACKMASK:
        gameUI.uIElements[9].SetActive(true);
        LeanTween.alphaCanvas(gameUI.uIElements[9].GetComponent<CanvasGroup>(), to: 1, 2f).setEaseInCubic().setOnComplete( ()=>
        {
            gameUI.fadeIn = true;
            gameUI.FadeIn();
        });
    }

    public void CameraShake()
    {
        aud.bGMAudioSource.Stop();

        GameObject.Find("SFXSource").GetComponent<AudioSource>().PlayOneShot(aud.cinematicSFX[UnityEngine.Random.Range(0, 1)]);
        GameObject.Find("SFXSource").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        PlayerAnimations();
    }

    public void SapTapperEntrance()
    {
        aud.bGMAudioSource.PlayOneShot(aud.cinematic[0]);

        //if (aud.AudioSettings.sliders[1].value > -50) { aud.musicLevel += 25; }

        GameObject.Find("SFXSource").GetComponent<AudioSource>().PlayOneShot(aud.cinematicSFX[2]);
        GameObject.Find("SFXSource").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        cinFX[0].SetActive(true);
        Invoke(nameof(HideGameObjects), 2);
    }

    public void SapTapperExit()
    {
        aud.bGMAudioSource.PlayOneShot(aud.cinematic[0]);

        GameObject.Find("BigSapTapper").GetComponent<AudioSource>().PlayOneShot(aud.cinematicSFX[2]);
        GameObject.Find("BigSapTapper").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        cinFX[1].SetActive(true);
        Invoke(nameof(HideGameObjects), 1);

        fadeOut = true;
    }

    public void HideGameObjects()
    {
        if(currCam != cams[4])
        {
            cinObjects[0].SetActive(false);
            cinObjects[2].SetActive(true);
        }
        else
        {
            cinObjects[1].SetActive(false);
            cinObjects[3].SetActive(true);
        }
    }

    public void PlayerAnimations()
    {
        if(camNo == 1)
        {
            playerAnim.SetTrigger("Cin_GroundShake");
        }
        else if(camNo == 2)
        {
            playerAnim.Play("ANIM_Knockback");
        }
    }

    public void ButtScratch()
    {
        playerAnim.Play("ButtScratch_ANIM");
    }

    public void Explosion()
    {
        cinFX[2].SetActive(true);
        GameObject.Find("BigSapTapper").GetComponent<AudioSource>().PlayOneShot(aud.cinematicSFX[2]);
        GameObject.Find("BigSapTapper").GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }


    public void EndCinematic()
    {
        gameUI.SkipCinematic();
    }
}
