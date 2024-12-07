using RainWard.Managers;
using RainWard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_PersistantData : MonoBehaviour
{
    public float masterAudio;
    public float musicAudio;
    public float ambientAudio;
    public float sfxAudio;

    private GameUI gameUI;

    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider ambientSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider musicSlider;
    

    void Awake()
    {
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();


        masterAudio = gameUI.sliders[0].value;
        musicAudio = gameUI.sliders[1].value;
        ambientAudio = gameUI.sliders[2].value;
        sfxAudio = gameUI.sliders[3].value;
    }
    void Start()
    {
        DeleteDuplicates();
    }

    private void DeleteDuplicates()
    {
        int duplicates = FindObjectsOfType(this.GetType()).Length;
        if (duplicates != 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
