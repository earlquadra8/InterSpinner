﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public delegate void GetNextDockID();
    public static event GetNextDockID NextDockIDGenerated;
    public delegate void GameStatusEvent(GameStatusEnum status);
    public static event GameStatusEvent GameStatusChanged;

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

    private static GameStatusEnum _gameStatus;
    private static int _nextDockID = -1;

    public bool enableGuideOnStart = false;
    public float maxDistance;
    public float maxDistanceBuffer;
    public float outOfBoundDrag;
    public float backToBoundStep;

    public GameObject stationHolder;

    Bus _bus;
    Vector3 _boundaryPosition;

    float _busPreviousDrag;

    #region prop
    public static int NextDockID { get { return _nextDockID; } }
    public static GameStatusEnum GameStatus
    {
        get { return _gameStatus;}
        set
        {
                _gameStatus = value;
            
            if (GameStatusChanged != null)
            {
                GameStatusChanged(_gameStatus);
            }
        }
    }
    #endregion prop

    public enum GameStatusEnum : int
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

    void printgamestatus(GameStatusEnum status)
    {
        print(status);
    }

    private void OnEnable()
    {
        Dock_DockBase.busDockingStatusUpdated += GenerateNextDockID;
        GameStatusChanged += printgamestatus;

        _gameStatus = GameStatusEnum.Started;
    }
    private void OnDisable()
    {
        Dock_DockBase.busDockingStatusUpdated -= GenerateNextDockID;
        GameStatusChanged -= printgamestatus;

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }
    #endregion OnEnable and OnDisable
    private void Start()
    {
        _bus = FindObjectOfType<Bus>();
        _busPreviousDrag = _bus.gameObject.GetComponent<Rigidbody>().drag;

        if (enableGuideOnStart)
        {
            TogglePauseGame();
        }

        GenerateNextDockID(true);
    }
    private void FixedUpdate()
    {
        OutOfBoundaryBehaviour();
    }
    private void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetButtonDown("Pause"))
        {
            TogglePauseGame();
        }
#endif
#if UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseGame();
        }
#endif
    }
    void GenerateNextDockID(bool isDocked)
    {
        if (isDocked && _gameStatus != GameStatusEnum.Overed)
        {
            int randomDockID;
            do
            {
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
            if (_gameStatus != GameStatusEnum.Overed)
            {
                _gameStatus = GameStatusEnum.Paused;
            }
            if (GameStatusChanged != null)
            {
                GameStatusChanged(GameStatusEnum.Paused);
            }
            Time.timeScale = 0;
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            if (_gameStatus != GameStatusEnum.Overed)
            {
                _gameStatus = GameStatusEnum.Resumed;
            }
            if (GameStatusChanged != null)
            {
                GameStatusChanged(GameStatusEnum.Resumed);
            }
        }
    }

    void OutOfBoundaryBehaviour()
    {
        if (_bus.transform.position.magnitude >= maxDistance && _bus.transform.position.magnitude < maxDistance + maxDistanceBuffer)// near the boundary but not out of it
        {
            _boundaryPosition = _bus.transform.position;// set the boundary
        }
        else if (_bus.transform.position.magnitude >= maxDistance + maxDistanceBuffer)// out of the boundary
        {
            _bus.transform.position = Vector3.MoveTowards(_bus.transform.position, _boundaryPosition, backToBoundStep);
            _bus.gameObject.GetComponent<Rigidbody>().drag = outOfBoundDrag;
        }
        else if (_bus.transform.position.magnitude < maxDistance)
        {
            _bus.gameObject.GetComponent<Rigidbody>().drag = _busPreviousDrag;
        }

    }
}