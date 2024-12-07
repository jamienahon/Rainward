using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using RainWard.UI;
using System.Linq;

namespace RainWard.Managers
{
    public class SCR_AudioManager : MonoBehaviour
    {
        SCR_PersistantData data;

        [field: SerializeField] public GameUI AudioSettings { get; set; }
        [System.NonSerialized] public bool[] audioChannels = new bool[4];

        public AudioSource bGMAudioSource;
        public AudioSource sFXAudioSource;
        public AudioSource ambientAudioSource;

        [Header("SFX:")]
        [Space]
        public AudioClip[] collectibles;
        [Space]
        public AudioClip[] machinery;
        [Space]
        public AudioClip[] uI;
        [Space]
        public AudioClip[] misc;
        [Space]
        public AudioClip[] player;
        [Space]
        public AudioClip[] bogwarts;
        [Space]
        [Header("Enviromental:")]
        [Space]
        public AudioClip[] cinematicSFX;

        [Header("Music / Ambience:")]
        [Space]
        public AudioClip[] mainMenu;
        public AudioClip[] ambience;

        [Header("Other:")]
        [Space]
        public AudioClip[] cinematic;

        [Header("Audio Settings:")]
        [Space]
        [Range(-50, 0)]
        public float masterLevel;
        [Range(-50, 0)]
        public float musicLevel;
        [Range(-50, 0)]
        public float ambienceLevel;
        [Range(-50, 0)]
        public float sFXLevel;

        public AudioMixer masterVolumeMixer;
        public AudioMixerGroup[] audioMixers = new AudioMixerGroup[3];

        public AudioSource musicSource;
        public AudioSource ambientSource;
        public AudioSource[] sfxSources;

        private void Awake()
        {
            //UpdateSliderMixSettings();
        }

        void Start()
        {
            data = GameObject.Find("PersistantData").GetComponent<SCR_PersistantData>();
            //audioMixers = masterVolumeMixer.FindMatchingGroups("Master/");
            //masterLevel = defaultMaster;
            //musicLevel = defaultMusic;
            //sFXLevel = defaultSFX;
            //ambienceLevel = defaultAmbience;
            
        }

        private void Update()
        {
            //musicSource.volume = musicLevel * (masterLevel / 10000);
            //ambientSource.volume = ambienceLevel * (masterLevel / 10000);

            //foreach (AudioSource source in sfxSources)
            //{
            //    source.volume = sFXLevel * (masterLevel / 10000);
            //}
            //foreach (AudioSource source in GameObject.Find("Collectibles").GetComponentsInChildren<AudioSource>())
            //{
            //    source.volume = sFXLevel * (masterLevel / 10000);
            //}
            //foreach (AudioSource source in GameObject.Find("Hazards").GetComponentsInChildren<AudioSource>())
            //{
            //    source.volume = sFXLevel * (masterLevel / 10000);
            //}
            //foreach (AudioSource source in GameObject.Find("Environmental").GetComponentsInChildren<AudioSource>())
            //{
            //    source.volume = sFXLevel * (masterLevel / 10000);
            //}
            //foreach (AudioSource source in GameObject.Find("Enemies").GetComponentsInChildren<AudioSource>())
            //{
            //    source.volume = sFXLevel * (masterLevel / 10000);
            //}

            //Debug.Log(audioMixers[0]);
            //Debug.Log(audioMixers[1]);
            //Debug.Log(audioMixers[2]);
       
        }

        public void PlaySound(AudioClip clip)
        {
            sFXAudioSource.PlayOneShot(clip/*, sFXLevel*/);
        }

        public void PlayAmbient(AudioClip clip)
        {
            ambientAudioSource.PlayOneShot(clip/*, ambienceLevel*/);
        }

        public void PlayBGM(AudioClip clip)
        {
            bGMAudioSource.PlayOneShot(clip/*, musicLevel*/);
        }

        public void UpdateMixSettings()
        {
            masterVolumeMixer.SetFloat("MasterVolume", masterLevel);
            masterVolumeMixer.SetFloat("MusicVolume", musicLevel);
            masterVolumeMixer.SetFloat("AmbienceVolume", ambienceLevel);
            masterVolumeMixer.SetFloat("SFXVolume", sFXLevel);
        }

        public void UpdateSliderMixSettings()
        {
            masterVolumeMixer.SetFloat("MasterVolume", AudioSettings.sliders[0].value);
            masterVolumeMixer.SetFloat("MusicVolume", AudioSettings.sliders[1].value);
            masterVolumeMixer.SetFloat("AmbienceVolume", AudioSettings.sliders[2].value);
            masterVolumeMixer.SetFloat("SFXVolume", AudioSettings.sliders[3].value);
        }
        

        public void UpdateVolume(float volume)
        {
            musicLevel = volume;
            ambienceLevel = volume;
            sFXLevel = volume;
        }

        //public void SetVolume(float volume)
        //{
            
        //}
    }
}