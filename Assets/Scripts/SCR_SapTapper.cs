using RainWard.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using RainWard.UI;

public class SCR_SapTapper : MonoBehaviour
{

    [SerializeField]
    GameObject workingTapper;
    [SerializeField]
    GameObject brokenTapper;

    [SerializeField]
    GameObject gumDrop;

    [SerializeField] private GameObject[] stVFX;

    [System.NonSerialized]
    public SCR_GameManager gameManager;
    [System.NonSerialized]
    public SCR_AudioManager audManager;
    [System.NonSerialized]
    public GameUI gameUI;

    [SerializeField]
    private float gumDropSpawnRadius;

    private bool isDestroyed = false;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<SCR_GameManager>();
        audManager = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    public void Update()
    {
        if (GetComponent<AudioSource>().volume == audManager.sFXLevel)
        {
            return;
        }
        else
        {
            GetComponent<AudioSource>().volume = audManager.sFXLevel;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<SCR_CC4>().IsDashing)
            {

                TapperDestroy();
                other.GetComponent<SCR_CC4>().DamagePlayer(new Vector3(-other.GetComponent<SCR_CC4>().DashVelocity.x, 0, -other.GetComponent<SCR_CC4>().DashVelocity.z));
            }
        }
    }

    public void TapperDestroy()
    {
        if(isDestroyed)
        {
            return;
        }

        workingTapper.SetActive(false);
        brokenTapper.SetActive(true);

        stVFX[0].SetActive(true);

        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audManager.machinery[3]);

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * gumDropSpawnRadius + transform.position;
            Instantiate(gumDrop, new Vector3(randomPosition.x, Random.Range(transform.position.y, transform.position.y + 5), randomPosition.z), gumDrop.transform.rotation);
        }

        if (gameObject.tag == "SapTapper")
        {
            gameUI.DisplaySapTapperScore();
            gameManager.SapTapperScore++;

        }

        isDestroyed = true;

    }
}
