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
    public bool levelDoScore;

    Bus bus;
    //UI_Manager uiManager;
    Game_Manager gameManager;


    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        bus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Bus>();
        bus.MaxFuel = levelMaxFuel;

        //uiManager = GameObject.Find("Canvas_Main").GetComponent<UI_Manager>();
        //uiManager.doTimer = levelDoTimer;
        //uiManager.timerTime = levelTime;

        UI_Manager.Instance.doTimer = levelDoTimer;
        UI_Manager.Instance.timerTime = levelTime;
        UI_Manager.Instance.doScore = levelDoScore;
    }
}
