using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public delegate void GetNextDockID();
    public static event GetNextDockID NextDockIDGenerated;
    public delegate void GameStateEvent(GameStateEnum state);
    public static event GameStateEvent GameStateChanged;

    private static Game_Manager _instance;
    public static Game_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("Game_Manager");
                go.AddComponent<Game_Manager>();
            }
            return _instance;
        }
    }

    private static GameStateEnum _gameState;
    private static int _nextDockID = -1;

    public bool enableGuideOnStart = false;

    public GameObject stationHolder;

    #region prop
    public static int NextDockID { get { return _nextDockID; } }
    public static GameStateEnum GameStatus
    {
        get { return _gameState;}
        set
        {
                _gameState = value;
            
            if (GameStateChanged != null)
            {
                GameStateChanged(_gameState);
            }
        }
    }
    #endregion prop

    public enum GameStateEnum : int
    {
        PreStart,
        Started,
        Paused,
        Resumed,
        Overed,
    }

    private void Awake()
    {
        _instance = this;
    }
    #region OnEnable and OnDisable
    private void OnEnable()
    {
        Dock_DockBase.busDockingStatusUpdated += GenerateNextDockID;

        _gameState = GameStateEnum.Started;
    }
    private void OnDisable()
    {
        Dock_DockBase.busDockingStatusUpdated -= GenerateNextDockID;

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }
    #endregion OnEnable and OnDisable
    private void Start()
    {
        if (enableGuideOnStart)
        {
            TogglePauseGame();
        }

        GenerateNextDockID(true);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TogglePauseGame();
        }
    }
    void GenerateNextDockID(bool isDocked)
    {
        if (isDocked && _gameState != GameStateEnum.Overed)
        {
            int randomDockID;
            do
            {
                //print("gm stationHolder not null? " + (stationHolder != null));
                randomDockID = Random.Range(0, stationHolder.transform.childCount);
            }
            while (_nextDockID == randomDockID && stationHolder.transform.childCount > 1);
            _nextDockID = randomDockID;

            if (NextDockIDGenerated != null)
            {
                NextDockIDGenerated();
            }
        }
    }

    public void TogglePauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            _gameState = GameStateEnum.Paused;
            if (GameStateChanged != null)
            {
                GameStateChanged(_gameState);
            }
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            _gameState = GameStateEnum.Resumed;
            if (GameStateChanged != null)
            {
                GameStateChanged(_gameState);
            }
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}