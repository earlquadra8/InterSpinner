using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_02 : MonoBehaviour
{
    private static Level_02 _instance;
    public static Level_02 Instance { get { return _instance; } }


    public bool levelDoTimer;
    public float levelTime;
    public float levelMaxFuel;

    Bus bus;
    UI_Manager uiManager;
    Game_Manager gameManager;


    private void Awake()
    {
        _instance = this;
        print("Lv2 Awake "+ UI_Manager.Instance.timerTime);
    }
    private void OnEnable()
    {
        bus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Bus>();
        //uiManager = GameObject.Find("Canvas_Main").GetComponent<UI_Manager>();

        bus.MaxFuel = levelMaxFuel;
        //uiManager.doTimer = levelDoTimer;
        //uiManager.timerTime = levelTime;
        UI_Manager.Instance.doTimer = levelDoTimer;
        UI_Manager.Instance.timerTime = levelTime;

        print("Lv2 Enable " + UI_Manager.Instance.timerTime);
    }
    private void Start()
    {
    }
}
