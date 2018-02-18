using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager _instance;
    public static Level_Manager Instance { get { return _instance; } }

    public int levelNum;
    public bool levelDoTimer;
    public float levelTime;
    public float levelMaxFuel;
    public bool levelDoScore;
    public float levelThreeStarScore;

    Bus bus;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        bus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Bus>();
        bus.MaxFuel = levelMaxFuel;
        //print("uiManager != null "+ (UI_Manager.Instance != null));// somehow UI_Manager has no instance when the game is started.
        //print("gameManager != null " + (Game_Manager.Instance != null));
        //UI_Manager.Instance.doTimer = levelDoTimer;
        //UI_Manager.Instance.timerTime = levelTime;
        //UI_Manager.Instance.doScore = levelDoScore;
        //UI_Manager.Instance.threeStarScore = levelThreeStarScore;
    }
    private void OnDisable()
    {
        SaveStars();
    }
    void SaveStars()
    {
        if (UI_Manager.Instance.StarCount > PlayerPrefs.GetInt(("level" + levelNum + "StarCount"), 0))
        {
            PlayerPrefs.SetInt(("level" + levelNum + "StarCount"), UI_Manager.Instance.StarCount);
        }
    }
}
