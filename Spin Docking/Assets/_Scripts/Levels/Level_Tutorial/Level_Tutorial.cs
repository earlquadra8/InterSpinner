using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Tutorial : MonoBehaviour
{
    public delegate void TutorialEvent();
    public static event TutorialEvent TutorialItemMovedOn;

    public static TutorialStage tutorialStage;

    private static Level_Tutorial _instance;
    public static Level_Tutorial Instance { get { return _instance; } }

    public Canvas levelTutorial;
    public GameObject pressKey;
    public GameObject UI;
    public GameObject gamePlay;
    public GameObject pressNKey;// The Tab key
    public GameObject refillKey;

    GameObject[] _pressKeyTutorialChilds;
    GameObject[] _uiTutorialChilds;
    GameObject[] _gamePlayTutorialChilds;

    Bus _bus;

    public enum TutorialStage : int
    {
        PressKey,
        UI,
        GamePlay,
        Finished,
    }

    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        _bus = FindObjectOfType<Bus>();

        _bus.busFuelEmptied += OnbusFuelEmptied;
        TutorialItemMovedOn += EnableTutorialItems;
        Dock_DockBase.busDockingStatusUpdated += OnBusDocked;
    }
    private void OnDisable()
    {
        _bus.busFuelEmptied -= OnbusFuelEmptied;
        TutorialItemMovedOn -= EnableTutorialItems;
        Dock_DockBase.busDockingStatusUpdated -= OnBusDocked;
    }
    private void Start()
    {

        tutorialStage = TutorialStage.PressKey;
        _pressKeyTutorialChilds = new GameObject[pressKey.transform.childCount];
        _uiTutorialChilds = new GameObject[UI.transform.childCount];
        _gamePlayTutorialChilds = new GameObject[gamePlay.transform.childCount];
        AddToArray(pressKey, _pressKeyTutorialChilds);
        AddToArray(UI, _uiTutorialChilds);
        AddToArray(gamePlay, _gamePlayTutorialChilds);

        EnableTutorialItems();
    }

    private void Update()
    {
        CheckTutorialConditions();
        RefillFuel();
    }

    #region Fuel empty
    void OnbusFuelEmptied()
    {
        refillKey.SetActive(true);
    }
    void RefillFuel()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            refillKey.SetActive(false);
            _bus.CurrentFuel = _bus.MaxFuel;
        }
    }
    #endregion Fuel empty

    #region Set up all tutorial objects
    void AddToArray(GameObject parent, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = parent.transform.GetChild(i).gameObject;
        }
    }

    int tutorialItemIndex = 0;
    void EnableTutorialItems()
    {
        if (GetTutorialArray() != null)
        {
            if (tutorialItemIndex < GetTutorialArray().Length)
            {
                if (tutorialItemIndex > 0 && GetTutorialArray()[tutorialItemIndex - 1] != null)
                {
                    GetTutorialArray()[tutorialItemIndex - 1].SetActive(false);
                }
                if (tutorialItemIndex >= 0)
                {
                    GetTutorialArray()[tutorialItemIndex].SetActive(true);
                }
                tutorialItemIndex++;
            }
            else
            {
                GetTutorialArray()[tutorialItemIndex - 1].SetActive(false);
                tutorialItemIndex = 0;
                MoveOnTutorialStage();
                EnableTutorialItems();
            }
        }
        
    }

    GameObject[] GetTutorialArray()
    {
        switch (tutorialStage)
        {
            case TutorialStage.PressKey:
                return _pressKeyTutorialChilds;
            case TutorialStage.UI:
                return _uiTutorialChilds;
            case TutorialStage.GamePlay:
                return _gamePlayTutorialChilds;
            case TutorialStage.Finished:
                return null;
            default:
                return null;
        }
    }
    void MoveOnTutorialStage()
    {
        switch (tutorialStage)
        {
            case TutorialStage.PressKey:
                tutorialStage = TutorialStage.UI;
                pressNKey.SetActive(true);
                break;
            case TutorialStage.UI:
                tutorialStage = TutorialStage.GamePlay;
                break;
            case TutorialStage.GamePlay:
                tutorialStage = TutorialStage.Finished;
                pressNKey.SetActive(false);
                break;
        }

    }
    #endregion Set up all tutorial objects

    void CheckTutorialConditions()
    {
        #region PressKey
        if (tutorialStage == TutorialStage.PressKey)
        {
            if (tutorialItemIndex >= 1)
            {
                if (GetTutorialArray()[tutorialItemIndex -1].GetComponent<PressKey>().sign == PosNeg.Positve)// Positive input axis
                {
                    if (Input.GetAxisRaw(GetTutorialArray()[tutorialItemIndex - 1].GetComponent<PressKey>().pressKey.ToString()) > 0)
                    {
                        TriggerTutorialItemMovedOnEvent();
                    }
                }
                else if (GetTutorialArray()[tutorialItemIndex - 1].GetComponent<PressKey>().sign == PosNeg.Negative)// Negative input axis
                {
                    if (Input.GetAxisRaw(GetTutorialArray()[tutorialItemIndex - 1].GetComponent<PressKey>().pressKey.ToString()) < 0)
                    {
                        TriggerTutorialItemMovedOnEvent();
                    }
                }
                else if (GetTutorialArray()[tutorialItemIndex - 1].GetComponent<PressKey>().sign == PosNeg.None)// No input axis
                {
                    if (Input.GetAxisRaw(GetTutorialArray()[tutorialItemIndex - 1].GetComponent<PressKey>().pressKey.ToString()) != 0)
                    {
                        TriggerTutorialItemMovedOnEvent();
                    }
                }
            }
        }
        #endregion PressKey
        #region UI
        else if (tutorialStage == TutorialStage.UI)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TriggerTutorialItemMovedOnEvent();
            }
        }
        #endregion UI
        #region GamePlay
        else if (tutorialStage == TutorialStage.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TriggerTutorialItemMovedOnEvent();
            }
        }
        #endregion GamePlay
        #region Finished
        else if (tutorialStage == TutorialStage.Finished)
        {

        }
        #endregion Finished
    }

    void TriggerTutorialItemMovedOnEvent()
    {
        if (TutorialItemMovedOn != null && Time.timeScale == 1)
        {
            TutorialItemMovedOn();
        }
    }

    bool everDocked = false;// make the end panel only show once
    void OnBusDocked(bool isDockedCorrectly)
    {
        if (isDockedCorrectly && !everDocked)
        {
            everDocked = true;
            Game_Manager.GameStatus = Game_Manager.GameStatusEnum.Overed;
        }
    }
}
