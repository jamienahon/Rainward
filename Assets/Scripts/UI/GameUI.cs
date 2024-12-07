using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;
using RainWard.Managers;
using Unity.VisualScripting;

namespace RainWard.UI
{
    /// <summary>
    /// Player HUD UI component
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        SCR_PersistantData data;

        [field: SerializeField] public SCR_AudioManager AudioManager { get; set; }
        [field: SerializeField] public SCR_CC4 CharacterController { get; set; }
        [field: SerializeField] public SCR_Cinematic CinematicManager { get; set; }
        [field: SerializeField] public SCR_GameManager GameManager { get; set; }

        [Space]
        [SerializeField] public GameObject[] uIElements;
        [Space]
        [SerializeField] private GameObject[] uIButtons;
        [Space]
        public TextMeshProUGUI[] uIText;
        [Space]
        public Image[] uIImages;
        [Space]
        public GameObject[] tooltips;
        [Space]
        public Slider[] sliders;
        public GameObject[] cameras;

        [Space]
        [SerializeField]
        private GameObject optionsFirstSelected;
        [SerializeField]
        private GameObject audioFirstSelected;
        [SerializeField]
        private GameObject mainMenuFirstSelected;
        [SerializeField]
        private GameObject totalScoreFirstSelected;
        [SerializeField]
        private GameObject inGameUIFirstSelected;
        [SerializeField]
        private GameObject controlsFirstSelected;
        [SerializeField]
        private GameObject skipCutsceneButton;

        [Space]


        [Header("Other References:")]
        [Space]

        [System.NonSerialized] public Animator uIAnimator;

        private Controls input;

        private int levelToLoad;
        private int currScene;

        [System.NonSerialized] public bool fadeIn;
        [System.NonSerialized] public bool fadeOut;

        private bool levelLoad;
        private bool inGame;
        private bool menuOpen;

        [System.NonSerialized] public bool scoreChanged;

        [System.NonSerialized] public bool[] uiActive = new bool[6];

        private float uiAlpha = 0;
        public float fadeTime;

        [Header("Tween:")]
        [Space]
        public LeanTweenType easeType;

        private List<GameObject> encounteredObjects = new List<GameObject>();

        void Awake()
        {
            

            //Hide MM_Buttons:
            foreach (CanvasGroup canvas in uIElements[0].GetComponentsInChildren<CanvasGroup>())
            {
                canvas.alpha = uiAlpha;
            }
            //Hide TotalScore:
            foreach (CanvasGroup canvas in uIElements[13].GetComponentsInChildren<CanvasGroup>())
            {
                canvas.alpha = uiAlpha;
            }

            //Hide SkipCinematic Button:
            uIButtons[5].GetComponent<CanvasGroup>().alpha = 0;

            input = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_CC4>().input;
            input.Player.Menu.performed += Menu;
            input.Menu.Enable();
            input.Menu.Close.performed += CloseMenu;
        }

        private void Start()
        {
            data = GameObject.Find("PersistantData").GetComponent<SCR_PersistantData>();

            //sliders[0].value = data.masterAudio;
            //sliders[1].value = data.musicAudio;
            //sliders[2].value = data.ambientAudio;
            //sliders[3].value = data.sfxAudio;

            OnMasterSliderChange(AudioManager.masterLevel);
            OnMusicSliderChange(AudioManager.musicLevel);
            OnAmbienceSliderChange(AudioManager.ambienceLevel);
            OnSFXSliderChange(AudioManager.sFXLevel);

            //Initialize Components:
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_CC4>().input;
            uIAnimator = GetComponent<Animator>();

            //Start Scene Fade-In:
            FadeIn();

            //Target FrameRate:
            Application.targetFrameRate = 60;

            PlayerHUDAnim();
        }

        public void Update()
        {
           

            
        }

        public void FadeIn()
        {
            fadeIn = true;

            //Checks for MAIN MENU:
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                inGame = false;
                //MainMenu Player ANIM:
                CharacterController.anim.Play("ANIM_Chillin");
                //Shows Cursor (for UI Navigation):
                Cursor.lockState = CursorLockMode.None;
                //DISABLES Input:
                input.Player.Disable();
                //Initialize FADE-IN:
                LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 0, fadeTime).setEaseInCubic().setDelay(2f).setOnComplete(OnFadeComplete);

                currScene = 0;
            }

            //Checks for CINEMATIC:
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
            {
                inGame = false;

                //MainMenu Player ANIM:
                CharacterController.anim.Play("ANIM_Chillin");
                //Hides Cursor (SKIP w/ Spacebar):
                Cursor.lockState = CursorLockMode.Locked;
                //DISABLES Input:
                input.Player.Disable();
                //Initialize FADE-IN:
                LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 0, fadeTime).setEaseInCubic().setDelay(2f).setOnComplete(OnFadeComplete);

                currScene = 1;
                EventSystem.current.SetSelectedGameObject(skipCutsceneButton);
            }

            //Otherwise ENABLES Input:
            else
            {
                inGame = true;

                //Hides Cursor (for Gameplay):
                Cursor.lockState = CursorLockMode.Locked;
                //Deactivates PLAYER Keyboard On GAME AWAKE:
                input.Player.Enable();
                //Initialize FADE-IN:
                LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 0, fadeTime - 2f).setEaseInCubic().setDelay(2f).setOnComplete(OnFadeComplete);

                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
                {
                    currScene = 2;
                }
                else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
                {
                    currScene = 3;
                }
                else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(4))
                {
                    currScene = 4;
                }
            }
        }

        public void FadeToLevel(int levelIndex)
        {
            //Enables BlackMask:
            uIElements[9].SetActive(true);

            levelLoad = true;
            levelToLoad = levelIndex;

            //Sets FadeOut:
            LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 1, fadeTime).setEaseInOutCubic().setOnComplete(OnFadeComplete);

            if (currScene == 0)
            {
                //FADE-OUT UI:
                LeanTween.alphaCanvas(uIButtons[0].GetComponent<CanvasGroup>(), to: 0, fadeTime - 2f).setEaseInCubic(); //RainWard Title
                LeanTween.alphaCanvas(uIButtons[1].GetComponent<CanvasGroup>(), to: 0, fadeTime -2.5f).setEaseInCubic(); //StartGame
                LeanTween.alphaCanvas(uIButtons[2].GetComponent<CanvasGroup>(), to: 0, fadeTime - 2.5f).setEaseInCubic(); //Options
                LeanTween.alphaCanvas(uIButtons[3].GetComponent<CanvasGroup>(), to: 0, fadeTime - 2.5f).setEaseInCubic(); //Credits
                LeanTween.alphaCanvas(uIButtons[4].GetComponent<CanvasGroup>(), to: 0, fadeTime - 2.5f).setEaseInCubic(); //ExitGame
            }
            else if (currScene == 1)
            {
                //FADE-OUT UI:
                LeanTween.alphaCanvas(uIButtons[5].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic(); //SkipCinematic
            }
            else if (currScene == 2)
            {
                OnClick();

                //FADE-OUT UI:
                LeanTween.alphaCanvas(uIElements[13].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic();
                uIElements[13].gameObject.LeanMoveLocalY(-Screen.height, 0.75f).setEaseInOutSine().setOnComplete(() =>
                {
                    //Hide Total Score:
                    uIElements[13].SetActive(false);
                });
            }
            else if (currScene == 3)
            {
                OnClick();

                AudioManager.PlaySound(AudioManager.uI[4]);

                if (uIElements[13].activeInHierarchy)
                {
                    //FADE-OUT UI:
                    LeanTween.alphaCanvas(uIElements[13].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic();
                    uIElements[13].gameObject.LeanMoveLocalY(-Screen.height, 0.75f).setEaseInOutSine().setOnComplete(() =>
                    {
                        //Hide Total Score:
                        uIElements[13].SetActive(false);
                    });
                }
                else if (uIElements[14].activeInHierarchy)
                {
                    //FADE-OUT UI:
                    LeanTween.alphaCanvas(uIElements[14].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic();
                    uIElements[14].gameObject.LeanMoveLocalY(-Screen.height, 0.75f).setEaseInOutSine().setOnComplete(() =>
                    {
                        //Hide Total Score:
                        uIElements[14].SetActive(false);
                    });
                }
            }
        }

        public void OnFadeComplete()
        {
            if(levelLoad)
            {
                levelLoad = false;
                //Loads Scene:
                SceneManager.LoadScene(levelToLoad);
            }
            else if (fadeIn)
            {
                fadeIn = false;

                AudioManager.UpdateMixSettings();

                //Disable BlackMask:
                uIElements[9].SetActive(false);

                if(currScene == 0)
                {
                    //FADE-IN UI:
                    LeanTween.alphaCanvas(uIButtons[0].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(2f); //RainWard Title
                    LeanTween.alphaCanvas(uIButtons[1].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(3f); //StartGame
                    LeanTween.alphaCanvas(uIButtons[2].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(3.25f); //Options
                    LeanTween.alphaCanvas(uIButtons[3].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(3.5f); //Credits
                    LeanTween.alphaCanvas(uIButtons[4].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(3.75f); //ExitGame

                    //Set UI Tweening:
                    //LeanTween.moveY(uIButtons[0], uIButtons[0].transform.position.y - 10f, 5f).setLoopPingPong().setEaseInBounce();
                    LeanTween.scale(uIButtons[0], new Vector3(1.05f, 1.05f, 1.05f), 10f).setLoopPingPong().setEaseInOutSine();
                }
                else if (currScene == 1)
                {
                    //FADE-IN UI:
                    LeanTween.alphaCanvas(uIButtons[5].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setDelay(5f); //SkipCinematic
                }
            }
        }

        public void StartGame()
        {
            OnClick();
            FadeToLevel(1);
        }

        public void Options()
        {
            EventSystem.current.SetSelectedGameObject(optionsFirstSelected);
            OnClick();

            if(uIElements[0].activeInHierarchy)
            {
                AudioManager.PlaySound(AudioManager.uI[4]);

                //Hide MAIN-MENU Buttons:
                foreach (Transform mmButtons in uIElements[0].transform)
                {
                    if (mmButtons.gameObject.activeInHierarchy)
                    {
                        //Set MM_Buttons FadeOut:
                        LeanTween.alphaCanvas(mmButtons.GetComponent<CanvasGroup>(), to: 0, 0.25f).setEaseInCubic().setOnComplete(() =>
                        {
                            //Hide MM_Buttons:
                            mmButtons.gameObject.SetActive(false);
                        }
                        );

                        //Display & Tween Options_Extended:
                        uIElements[2].gameObject.SetActive(true);
                        uIElements[2].transform.localPosition = new Vector2(0, -Screen.height);
                        uIElements[2].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic();
                    }

                }
            }

            if(uIElements[3].activeInHierarchy)
            {
                //RETURN from Audio_Extended:
                AudioManager.PlaySound(AudioManager.uI[4]);
                uIElements[3].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    //Hide Audio_Extended:
                    uIElements[3].gameObject.SetActive(false);

                    //Display & Tween Options_Extended:
                    uIElements[2].gameObject.SetActive(true);
                    uIElements[2].transform.localPosition = new Vector2(0, -Screen.height);
                    uIElements[2].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
                }
                );
            }
            else if (uIElements[11].activeInHierarchy)
            {
                //RETURN from Controls_Extended:
                AudioManager.PlaySound(AudioManager.uI[4]);
                uIElements[11].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    //Hide Controls_Extended:
                    uIElements[11].gameObject.SetActive(false);

                    //Display & Tween Options_Extended:
                    uIElements[2].gameObject.SetActive(true);
                    uIElements[2].transform.localPosition = new Vector2(0, -Screen.height);
                    uIElements[2].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
                }
                );
            }

            if (inGame)
            {
                //Tween InGame Menu:
                AudioManager.PlaySound(AudioManager.uI[4]);
                uIElements[8].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    //Hide InGame Menu:
                    uIElements[8].SetActive(false);

                    //Display & Tween Options_Extended:
                    uIElements[2].gameObject.SetActive(true);
                    uIElements[2].transform.localPosition = new Vector2(0, -Screen.height);
                    uIElements[2].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
                });
            }
        }

        public void Audio()
        {
            EventSystem.current.SetSelectedGameObject(audioFirstSelected);
            OnClick();

            AudioManager.PlaySound(AudioManager.uI[4]);

            uIElements[2].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
            {
                //Hide Options_Extended:
                uIElements[2].gameObject.SetActive(false);

                //Display Audio_Extended:
                uIElements[3].gameObject.SetActive(true);
                uIElements[3].transform.localPosition = new Vector2(0, -Screen.height);
                uIElements[3].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
            });
        }

        public void Controls()
        {
            EventSystem.current.SetSelectedGameObject(controlsFirstSelected);
            OnClick();

            AudioManager.PlaySound(AudioManager.uI[4]);

            uIElements[2].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
            {
                //Hide Options_Extended:
                uIElements[2].gameObject.SetActive(false);

                //Display Audio_Extended:
                uIElements[11].gameObject.SetActive(true);
                uIElements[11].transform.localPosition = new Vector2(-1086, -Screen.height);
                uIElements[11].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
            });
        }

        public void Credits()
        {
            OnClick();

            //Hide MAIN-MENU Buttons:
            foreach (Transform mmButtons in uIElements[0].transform)
            {
                if (mmButtons.gameObject.activeInHierarchy)
                {
                    //Set MM_Buttons FadeOut:
                    LeanTween.alphaCanvas(mmButtons.GetComponent<CanvasGroup>(), to: 0, 0.25f).setEaseInCubic().setOnComplete(() =>
                    {
                        //Hide MM_Buttons:
                        mmButtons.gameObject.SetActive(false);

                        //FadeIn Credits (DESIGNERS):
                        uIElements[12].SetActive(true);
                        LeanTween.alphaCanvas(uIElements[12].GetComponent<CanvasGroup>(), to: 1, 2f).setEaseInCubic().setOnComplete(() =>
                        {
                            //FadeOut DESIGNERS
                            LeanTween.alphaCanvas(uIText[6].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic().setDelay(3f).setOnComplete(() =>
                            {
                                //FadeIn ARTISTS:
                                LeanTween.alphaCanvas(uIText[7].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setOnComplete(() =>
                                {
                                    //FadeOut ARTISTS:
                                    LeanTween.alphaCanvas(uIText[7].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic().setDelay(3f).setOnComplete(() =>
                                    {
                                        //FadeIn PROGRAMMERS:
                                        LeanTween.alphaCanvas(uIText[8].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setOnComplete(() =>
                                        {
                                            //FadeOut ARTISTS:
                                            LeanTween.alphaCanvas(uIText[8].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic().setDelay(3f).setOnComplete(() =>
                                            {
                                                //FadeIn SFX/AUDIO:
                                                LeanTween.alphaCanvas(uIText[9].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic().setOnComplete(() =>
                                                {
                                                    //FadeOut ARTISTS:
                                                    LeanTween.alphaCanvas(uIText[9].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic().setDelay(3f).setOnComplete(() =>
                                                    {
                                                        Menu();
                                                    }
                                                    );
                                                }
                                                );
                                            }
                                            );
                                        }
                                        );
                                    }
                                    );
                                }
                                );

                            }
                            );
                        }
                        );

                    }
                    );
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void SkipCinematic()
        {
            CinematicManager.enabled = false;
            FadeToLevel(2);
        }

        void Menu(InputAction.CallbackContext context)
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) && !uIElements[8].activeInHierarchy)
            {
                Menu();
            }

        }

        void CloseMenu(InputAction.CallbackContext context)
        {
            if (uIElements[8].activeInHierarchy)
            {
                Menu();
            }

        }

        public void Menu()
        {
            if(!uIElements[13].activeInHierarchy)
            {
                if (!inGame)
                {
                    OnClick();

                    foreach (Transform mmButtons in uIElements[0].transform)
                    {
                        //Show MM_Buttons:
                        mmButtons.gameObject.SetActive(true);
                        //Set MM_Buttons FadeIn:
                        LeanTween.alphaCanvas(mmButtons.GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInCubic();
                    }

                    //FromOPTIONS:
                    if (uIElements[2].activeInHierarchy)
                    {
                        AudioManager.PlaySound(AudioManager.uI[4]);
                        uIElements[2].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setOnComplete(() =>
                        {
                            //Hide Options_Extended:
                            uIElements[2].gameObject.SetActive(false);

                        });
                    }

                    //FromCREDITS:
                    if (uIElements[12].activeInHierarchy)
                    {
                        //Set CREDITS FadeOut:
                        LeanTween.alphaCanvas(uIElements[12].GetComponent<CanvasGroup>(), to: 0, 0.25f).setEaseInCubic().setOnComplete(() =>
                        {
                            //FadeIn Credits (DESIGNERS):
                            uIElements[12].SetActive(false);
                            uIText[6].GetComponent<CanvasGroup>().alpha = 1;
                        }
                        );
                    }
                    EventSystem.current.SetSelectedGameObject(inGameUIFirstSelected);
                }
                else
                {

                    if (!menuOpen)
                    {
                        OnClick();

                        //Show InGame Menu:
                        uIElements[8].SetActive(true);
                        EventSystem.current.SetSelectedGameObject(inGameUIFirstSelected);
                        //Tween InGame Menu:
                        uIElements[8].transform.localPosition = new Vector2(0, -Screen.height);
                        AudioManager.PlaySound(AudioManager.uI[4]);
                        uIElements[8].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                        {
                            menuOpen = true;

                            //Disable GamePlay Functions:
                            input.Player.Disable();
                            Cursor.lockState = CursorLockMode.None;
                            GameObject.FindGameObjectWithTag("MainCinCam").GetComponent<CinemachineFreeLook>().enabled = false;

                            //Hide OPEN Panels:
                            uIElements[2].SetActive(false);

                            //PAUSE GAME:
                            Time.timeScale = 0;

                        });
                    }
                    else
                    {
                        OnClick();

                        AudioManager.PlaySound(AudioManager.uI[4]);

                        //FromOPTIONS:
                        if (uIElements[2].activeInHierarchy)
                        {
                            uIElements[2].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                            {
                                //Hide Options_Extended:
                                uIElements[2].gameObject.SetActive(false);

                                //Show InGame Menu:
                                uIElements[8].SetActive(true);
                                //Tween InGame Menu:
                                uIElements[8].transform.localPosition = new Vector2(0, -Screen.height);
                                uIElements[8].LeanMoveLocalY(0, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);

                            });
                            EventSystem.current.SetSelectedGameObject(inGameUIFirstSelected);
                        }
                        else
                        {
                            //Tween InGame Menu:
                            uIElements[8].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                            {
                                menuOpen = false;

                                //Hide InGame Menu:
                                uIElements[8].SetActive(false);

                                //Enable GamePlay Functions:
                                input.Player.Enable();
                                Cursor.lockState = CursorLockMode.Locked;
                                GameObject.FindGameObjectWithTag("MainCinCam").GetComponent<CinemachineFreeLook>().enabled = true;

                                //UN-PAUSE GAME:
                                Time.timeScale = 1;
                            }
                            );

                        }
                    }
                }
            }
            
        }

        public void OnResume()
        {
            OnClick();

            //Tween InGame Menu:
            AudioManager.PlaySound(AudioManager.uI[4]);
            uIElements[8].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
            {
                menuOpen = false;

                //Hide InGame Menu:
                uIElements[8].SetActive(false);

                //Enable GamePlay Functions:
                input.Player.Enable();
                Cursor.lockState = CursorLockMode.Locked;
                GameObject.FindGameObjectWithTag("MainCinCam").GetComponent<CinemachineFreeLook>().enabled = true;

                //UN-PAUSE GAME:
                Time.timeScale = 1;

            });
        }

        public void OnDeath()
        {
            //Sets FadeOut:
            uIElements[9].SetActive(true);
            LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 1, 2f).setEaseInOutCubic().setOnComplete(FadeIn);
        }

        public void OnMasterSliderChange(float volume)
        {
            AudioManager.masterLevel = volume;
            AudioManager.UpdateMixSettings();
            uIText[0].text = (volume + 50).ToString() + "%";
        }
        public void OnMusicSliderChange(float volume)
        {
            AudioManager.musicLevel = volume;
            AudioManager.UpdateMixSettings();
            uIText[1].text = (volume + 50).ToString() + "%";
        }
        public void OnAmbienceSliderChange(float volume)
        {
            AudioManager.ambienceLevel = volume;
            AudioManager.UpdateMixSettings();
            uIText[2].text = (volume + 50).ToString() + "%";
        }
        public void OnSFXSliderChange(float volume)
        {
            AudioManager.sFXLevel = volume;
            AudioManager.UpdateMixSettings();
            uIText[3].text = (volume + 50).ToString() + "%";
        }

        public void OnClick()
        {
            AudioManager.PlaySound(AudioManager.uI[0]);
        }

        public void OnRestart()
        {
            OnClick();

            //UN-PAUSE GAME:
            Time.timeScale = 1;

            FadeToLevel(currScene);

            //Hides MM:
            if(uIElements[8].activeInHierarchy)
            {
                uIElements[8].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    menuOpen = false;

                    //Hide InGame Menu:
                    uIElements[8].SetActive(false);

                    Cursor.lockState = CursorLockMode.Locked;

                });
            }

            //Hides Total Score:
            if (uIElements[13].activeInHierarchy)
            {
                //FADE-OUT UI:
                LeanTween.alphaCanvas(uIButtons[5].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic(); //FadeToLevel
                LeanTween.alphaCanvas(uIElements[13].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic();
                uIElements[13].gameObject.LeanMoveLocalY(-Screen.height, 0.75f).setEaseInOutSine().setOnComplete( ()=>
                {
                    //Hide Total Score:
                    uIElements[13].SetActive(false);
                });
            }
        }

        public void OnMainMenuReturn()
        {
            EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
            OnClick();

            //Tween InGame Menu:
            uIElements[8].LeanMoveLocalY(-Screen.height, 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
            {
                //UN-PAUSE GAME:
                Time.timeScale = 1;

                menuOpen = false;

                AudioManager.PlaySound(AudioManager.uI[4]);
                Cursor.lockState = CursorLockMode.Locked;

                //Hide InGame Menu:
                uIElements[8].SetActive(false);
                FadeToLevel(0);
            }
            );
        }

        public void DisplayTooltip(int curr, GameObject encounteredObject)
        {
            if (!tooltips[curr].gameObject.activeInHierarchy)
            {
                AudioManager.PlaySound(AudioManager.uI[2]);

                tooltips[curr].gameObject.SetActive(true);
                tooltips[curr].transform.localPosition = new Vector2(-264, -Screen.height);
                tooltips[curr].LeanMoveLocalY(0, 0.5f).setEaseInOutCubic().setOnComplete( ()=>
                {
                    LeanTween.moveY(tooltips[curr], tooltips[curr].transform.position.y - 20f, 3f).setLoopPingPong().setEaseInOutSine();
                    tooltips[curr].LeanMoveLocalY(-Screen.height, 0.5f).setEaseInOutCubic().setDelay(5f).setOnComplete( ()=>
                    {
                        tooltips[curr].gameObject.SetActive(false);
                        tooltips[curr] = null;
                    });
                });

                foreach (GameObject tt in tooltips)
                {
                    if (tt.activeInHierarchy && tt != tooltips[curr])
                    {
                        LeanTween.alphaCanvas(tt.GetComponent<CanvasGroup>(), to: 0, 0.5f).setEaseInCubic();
                    }
                }
            }


        }

        private bool EncounteredSearch(GameObject a)
        {
            return encounteredObjects.Contains(a);
        }

        public void DisplaySapTapperScore()
        {
            if (!uIElements[5].gameObject.activeInHierarchy)
            {
                uIElements[5].gameObject.SetActive(true);
                uIElements[5].transform.localPosition = new Vector2(300, +Screen.height);
                uIElements[5].LeanMoveLocalY(456, 1f).setEaseInOutCubic().setOnComplete(() =>
                {
                    LeanTween.moveY(uIElements[5], uIElements[5].transform.position.y - 5f, 1f).setLoopPingPong().setEaseInOutSine();
                    uIElements[5].LeanMoveLocalY(+Screen.height, 1f).setEaseInOutCubic().setDelay(5f).setOnComplete(() =>
                    {
                        uIElements[5].gameObject.SetActive(false);
                    });

                });
            }
        
        }

        public void DisplayGumDropScore()
        {
            LeanTween.scale(uIText[5].gameObject, new Vector3(1.1f, 1.1f, 1.1f), .25f).setEaseInOutSine().setOnComplete( ()=>
            {
                LeanTween.scale(uIText[5].gameObject, new Vector3(1f, 1f, 1f), .25f).setEaseInOutSine();
            }
            );
        }

        public void PlayerHUDAnim()
        {
            //GumDrop Tweening:
            LeanTween.moveY(uIImages[3].gameObject, uIImages[3].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine();
            LeanTween.scale(uIImages[3].gameObject, new Vector3(0.65f, 0.65f, 0.65f), 10f).setLoopPingPong().setEaseInOutSine();

            //HPMush Tweening:
            //1
            LeanTween.moveY(uIImages[0].gameObject, uIImages[0].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine();
            LeanTween.scale(uIImages[0].gameObject, new Vector3(1.1f, 1.1f, 1.1f), 10f).setLoopPingPong().setEaseInOutSine();
            //2
            LeanTween.moveY(uIImages[1].gameObject, uIImages[0].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine().setDelay(2f);
            LeanTween.scale(uIImages[1].gameObject, new Vector3(1.1f, 1.1f, 1.1f), 10f).setLoopPingPong().setEaseInOutSine().setDelay(2f);
            //3
            LeanTween.moveY(uIImages[2].gameObject, uIImages[0].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine();
            LeanTween.scale(uIImages[2].gameObject, new Vector3(1.1f, 1.1f, 1.1f), 10f).setLoopPingPong().setEaseInOutSine();
        }

        IEnumerator UpdateGDTally()
        {
            int totalGumDrops = 0;

            //Finds Total Gum Drops:
            foreach (GameObject gumdrop in GameObject.FindGameObjectsWithTag("GumDrop"))
            {
                totalGumDrops++;
            }
            uIText[14].text = "/ " + (totalGumDrops + GameManager.GumDropScore).ToString();
            for (int i = 0; i <= GameManager.GumDropScore; i++)
            {
                uIText[13].text = i.ToString();
                yield return new WaitForSeconds(0.1f);
                AudioManager.PlaySound(AudioManager.collectibles[UnityEngine.Random.Range(0, 3)]);

                if (i >= GameManager.GumDropScore)
                {
                    //Displays SAPTAPPER SCORE:
                    LeanTween.alphaCanvas(uIImages[4].GetComponent<CanvasGroup>(), to: 1, 0.75f).setEaseInOutCubic().setDelay(1f).setOnComplete(() =>
                    {
                        AudioManager.PlaySound(AudioManager.uI[1]);
                        LeanTween.scale(uIImages[4].gameObject, new Vector3(1.85f, 1.85f, 1.5f), 5f).setLoopPingPong().setEaseInOutSine(); //STImage
                        LeanTween.alphaCanvas(uIText[12].GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInOutCubic().setDelay(0.5f).setOnComplete( ()=>
                        {
                            LeanTween.alphaCanvas(uIText[11].GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInOutCubic(); //STScore

                            StartCoroutine(UpdateSTTally());
                        }
                        );
                    }
                    );
                }
            }

        }

        IEnumerator UpdateSTTally()
        {
            int totalSapTappers = 0;

            //Finds Total SapTappers:
            foreach (GameObject st in GameObject.FindGameObjectsWithTag("SapTapper"))
            {
                totalSapTappers++;
            }
            uIText[12].text = "/ " + (totalSapTappers + GameManager.SapTapperScore).ToString();
            for (int i = 0; i <= GameManager.SapTapperScore; i++)
            {
                uIText[11].text = i.ToString();
                yield return new WaitForSeconds(0.1f);
                AudioManager.PlaySound(AudioManager.collectibles[UnityEngine.Random.Range(0, 3)]);

                //Display Continue Button:
                LeanTween.alphaCanvas(uIButtons[15].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic().setDelay(1f).setOnComplete( ()=>
                {
                    LeanTween.alphaCanvas(uIButtons[16].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic();
                }
                ); 

            }

        }

        public void DisplayTotalScore()
        {
            EventSystem.current.SetSelectedGameObject(totalScoreFirstSelected);
            //FadeOut Volume:
            AudioManager.UpdateVolume(0);

            //Enables BlackMask:
            uIElements[9].SetActive(true);

            //Sets FadeOut:
            LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 1, fadeTime).setEaseInOutCubic().setOnComplete( ()=>
            {
                //Disable GamePlay Functions:
                input.Player.Disable();
                Cursor.lockState = CursorLockMode.None;

                //Change Cameras:
                cameras[0].SetActive(false);
                cameras[1].SetActive(true);

                //Hides InGame UI:
                uIElements[1].SetActive(false);

                //Sets FadeIn:
                LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 0, 2).setEaseInOutCubic().setOnComplete(() =>
                {
                    AudioManager.bGMAudioSource.Stop();

                    //Disables BlackMask:
                    uIElements[9].SetActive(false);

                    //Enables / Tweens TotalScore:
                    uIElements[13].SetActive(true);
                    uIElements[13].transform.localPosition = new Vector2(0, -Screen.height);
                    AudioManager.PlaySound(AudioManager.uI[4]);
                    LeanTween.alphaCanvas(uIElements[13].GetComponent<CanvasGroup>(), to: 1, 0.75f).setEaseInOutCubic();
                    uIElements[13].gameObject.LeanMoveLocalY(0, 0.75f).setEaseInOutSine().setOnComplete(() =>
                    {
                        LeanTween.moveY(uIElements[13].gameObject, uIElements[13].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine();

                        //Displays GUM DROP SCORE:
                        LeanTween.scale(uIImages[5].gameObject, new Vector3(0.95f, 0.95f, 0.95f), 5f).setLoopPingPong().setEaseInOutSine(); //GDImage
                        LeanTween.alphaCanvas(uIImages[5].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic().setDelay(0.5f).setOnComplete( ()=>
                        {
                            AudioManager.PlaySound(AudioManager.uI[1]);
                            LeanTween.alphaCanvas(uIText[14].GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInOutCubic().setDelay(0.5f).setOnComplete( ()=>
                            {
                                LeanTween.alphaCanvas(uIText[13].GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInOutCubic(); //GDScore

                                StartCoroutine(UpdateGDTally());
                            }
                            );
                        }
                        ); 
                    });

                }
                );
            }
            );
        }

        public void GameEndScreen()
        {
            //Enables BlackMask:
            uIElements[9].SetActive(true);

            //Hides Total Score:
            if (uIElements[13].activeInHierarchy)
            {
                //FADE-OUT UI:
                LeanTween.alphaCanvas(uIButtons[5].GetComponent<CanvasGroup>(), to: 1, 1f).setEaseInCubic(); //FadeToLevel
                LeanTween.alphaCanvas(uIElements[13].GetComponent<CanvasGroup>(), to: 0, 1f).setEaseInCubic();
                AudioManager.PlaySound(AudioManager.uI[4]);
                uIElements[13].gameObject.LeanMoveLocalY(-Screen.height, 0.75f).setEaseInOutSine().setOnComplete(() =>
                {
                    //Hide Total Score:
                    uIElements[13].SetActive(false);

                    //Sets FadeOut:
                    LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 1, fadeTime).setEaseInOutCubic().setOnComplete(() =>
                    {
                        //Sets FadeIn:
                        LeanTween.alphaCanvas(uIElements[9].GetComponent<CanvasGroup>(), to: 0, 2).setEaseInOutCubic().setOnComplete(() =>
                        {
                            AudioManager.bGMAudioSource.Play();

                            //Disables BlackMask:
                            uIElements[9].SetActive(false);

                            //Enables / Tweens EndGame Panel:
                            uIElements[14].SetActive(true);
                            uIElements[14].transform.localPosition = new Vector2(0, -Screen.height);
                            AudioManager.PlaySound(AudioManager.uI[4]);
                            LeanTween.alphaCanvas(uIElements[14].GetComponent<CanvasGroup>(), to: 1, 0.75f).setEaseInOutCubic();
                            uIElements[14].gameObject.LeanMoveLocalY(0, 0.75f).setEaseInOutSine().setOnComplete(() =>
                            {
                                LeanTween.moveY(uIElements[14].gameObject, uIElements[14].gameObject.transform.position.y - 5f, 2f).setLoopPingPong().setEaseInOutSine();

                                //Displays Text:
                                LeanTween.alphaCanvas(uIText[15].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic().setDelay(0.5f).setOnComplete(() =>
                                {
                                    LeanTween.scale(uIImages[6].gameObject, new Vector3(1.05f, 1.05f, 1.05f), 5f).setLoopPingPong().setEaseInOutSine();
                                    LeanTween.alphaCanvas(uIImages[6].GetComponent<CanvasGroup>(), to: 1, 0.25f).setEaseInOutCubic().setDelay(0.5f).setOnComplete(() =>
                                    {
                                        //PlayAgain
                                        LeanTween.alphaCanvas(uIButtons[17].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic().setOnComplete(() =>
                                        {
                                            LeanTween.alphaCanvas(uIButtons[18].GetComponent<CanvasGroup>(), to: 1, 0.5f).setEaseInOutCubic();
                                        });

                                    }
                                    );
                                }
                                );
                            });

                        }
                        );
                    }
                    );
                });
            }

            
        }

    }

}