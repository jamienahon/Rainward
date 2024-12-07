using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RainWard.Managers;

public class SCR_Checkpoint : MonoBehaviour
{
    private GameObject currCheckpoint;
    private GameObject player;
    private SCR_CC4 cc;
    private SCR_AudioManager aud;

    public Animator playerAnim;

    private Vector3 spawnpoint;
    private Vector3 playerFacing;

    private bool cpActive;

    [SerializeField] private GameObject[] particles;

    public Vector3 Spawnpoint { get { return spawnpoint; } set { spawnpoint = value; } }
    public Vector3 PlayerFacing { get { return playerFacing; } set { playerFacing = value; } }


    void Start()
    {
        cc = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_CC4>();
        aud = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        //spawnpoint = player.transform.position;
        //playerFacing = player.transform.forward;
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Activated");
            
            if(gameObject.tag != "ActiveCheckPoint")
            {
                if(GameObject.FindGameObjectWithTag("ActiveCheckPoint") != null)
                {
                    GameObject.FindGameObjectWithTag("ActiveCheckPoint").GetComponentInParent<Animator>().SetBool("isInactive", true);
                    GameObject.FindGameObjectWithTag("ActiveCheckPoint").GetComponentInParent<Animator>().SetBool("isActive", false);
                    GameObject.FindGameObjectWithTag("ActiveCheckPoint").tag = "InactiveCheckPoint";
                }

                gameObject.GetComponentInParent<AudioSource>().enabled = true;
                gameObject.GetComponentInParent<AudioSource>().PlayOneShot(aud.misc[0]);

                gameObject.tag = "ActiveCheckPoint";
                gameObject.GetComponentInParent<Animator>().SetBool("isActive", true);
                currCheckpoint = gameObject;

                spawnpoint = other.transform.position;
                playerFacing = other.transform.forward;
            }

        }
    }

    public void RespawnPlayer()
    {
        cc.input.Enable();
        player.transform.position = spawnpoint;
        player.transform.forward = playerFacing;

        playerAnim.Play("ANIM_Idle");

        cc.isDead = false;
        cc.isDrowning = false;
        
        GameObject.FindGameObjectWithTag("MainCinCam").GetComponent<CinemachineFreeLook>().m_XAxis.Value = cc.transform.rotation.y;
    }
}
