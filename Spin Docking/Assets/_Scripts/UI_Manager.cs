using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager _instance;
    public static UI_Manager Instance { get { return _instance; } }
    [Header("Next Dock ID")]
    public Animator nextDockIDAnimator;
    public Text nextDockID;
    [Header("UI")]
    public Text angularMeter;
    public Text driftMeter;
    public Text fuelMeter;
    [Header("Dock Status")]
    public Animator dockStatusAnimator;
    public Text dockStatus;
    [Header("Timer")]
    public Animator timerAnimator;
    public bool doTimer;
    public float timerTime;
    public Text timerText;
    [Header("Score Board")]
    public bool doScore;
    public float score;
    public Text scoreBoard;
    [Header("Pause Panel")]
    public GameObject pausePanel;
    [Header("End Panel")]
    public GameObject endPanel;
    public Text endScore;
    public GameObject starsParent;
    public float threeStarScore;

    int _starCount;
    Level_Manager _levelManager;
    Bus _bus;

    public int StarCount { get { return _starCount; } }

    private void Awake()
    {
        _instance = this;
    }
    #region OnEnable & OnDisable
    private void OnEnable()
    {
        Game_Manager.NextDockIDGenerated += UpdateNextDockText;
        Game_Manager.GameStatusChanged += ToggleGuide;
        Game_Manager.GameStatusChanged += OnGameOvered;
        Dock_DockBase.busDockingStatusUpdated += UpdateDockStatusText;
        Dock_DockBase.busDockingStatusUpdated += Scoring;
    }
    private void OnDisable()
    {
        Game_Manager.NextDockIDGenerated -= UpdateNextDockText;
        Game_Manager.GameStatusChanged -= ToggleGuide;
        Game_Manager.GameStatusChanged -= OnGameOvered;
        Dock_DockBase.busDockingStatusUpdated -= UpdateDockStatusText;
        Dock_DockBase.busDockingStatusUpdated -= Scoring;
    }
    #endregion OnEnable & OnDisable

    void Start ()
    {
        _bus = FindObjectOfType<Bus>();//GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Bus>();

        _levelManager = FindObjectOfType<Level_Manager>();//GameObject.Find("Level_Manager").GetComponent<Level_Manager>();
        doTimer = _levelManager.levelDoTimer;// initialize
        timerTime = _levelManager.levelTime;// initialize
        doScore = _levelManager.levelDoScore;// initialize
        threeStarScore = _levelManager.levelThreeStarScore;// initialize

        UpdateDockStatusText(false);// initialize
        endPanel.SetActive(false);// initialize

        timerText.text = string.Format("Timer: {0}", timerTime.ToString("000"));
        UpdateScoreBoard();
    }

    void Update ()
    {
        UpdateMeters();
    }
    private void FixedUpdate()
    {
        TimerUpdate();
    }

    void OnGameOvered(Game_Manager.GameStatusEnum status)
    {
        if (status == Game_Manager.GameStatusEnum.Overed)
        {
            ToggleEndPanel();
            DisableScoring();
            EnableStars();
        }
    }

    void UpdateMeters()
    {
        float AngularX = _bus.LocalAngularVelocity.x, AngularY = _bus.LocalAngularVelocity.y, AngularZ = _bus.LocalAngularVelocity.z;
        AngularX = (float)Math.Round(AngularX, 1);
        AngularY = (float)Math.Round(AngularY, 1);
        AngularZ = (float)Math.Round(AngularZ, 1);
        float DriftX = _bus.LocalVelocity.x, DriftY = _bus.LocalVelocity.y, DriftZ = _bus.LocalVelocity.z;
        DriftX = (float)Math.Round(DriftX, 1);
        DriftY = (float)Math.Round(DriftY, 1);
        DriftZ = (float)Math.Round(DriftZ, 1);

        angularMeter.text = string.Format("Angular Velocity |X: {0} |Y: {1} |Z: {2}", AngularX.ToString("＋00.0;－00.0"), AngularY.ToString("＋00.0;－00.0"), AngularZ.ToString("＋00.0;－00.0"));
        driftMeter.text = string.Format("Drifting Velocity |X: {0} |Y: {1} |Z: {2}", DriftX.ToString("＋00.0;－00.0"), DriftY.ToString("＋00.0;－00.0"), DriftZ.ToString("＋00.0;－00.0"));
        fuelMeter.text = "Fuel: " + _bus.CurrentFuel.ToString("0000");
    }
    void UpdateNextDockText()
    {
        nextDockID.text = "Going to Dock " + Game_Manager.NextDockID.ToString("00");
        nextDockIDAnimator.SetTrigger("textTrigger");
    }

    void UpdateDockStatusText(bool isDocked)
    {
        if (isDocked)
        {
            dockStatus.text = "Status: Docked";
            dockStatusAnimator.SetTrigger("textTrigger");
        }
        else
        {
            dockStatus.text = "Status: Free";
        }
    }

    void ToggleGuide(Game_Manager.GameStatusEnum status)
    {
        if (status == Game_Manager.GameStatusEnum.Paused)
        {
            if (!pausePanel.activeSelf)
            {
                pausePanel.SetActive(true);
            }
        }
        else if (status == Game_Manager.GameStatusEnum.Resumed)
        {
            if (pausePanel.activeSelf)
            {
                pausePanel.SetActive(false);
            }
        }
    }
    void ToggleEndPanel()
    {
        if (!endPanel.activeSelf)
        {
            endPanel.SetActive(true);
        }
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void EndPanelCrossButton()
    {
        if (endPanel.activeSelf)
        {
            endPanel.SetActive(false);
        }
    }

    float deltaTime;// for better text animation control
    void TimerUpdate()
    {
        if (doTimer && timerTime > 0)
        {
            if (deltaTime < 1)
            {
                deltaTime += Time.fixedDeltaTime;
            }
            else
            {
                deltaTime = 1;
                timerTime -= deltaTime;
                timerText.text = string.Format("Timer: {0}", timerTime.ToString("000"));
                if (timerTime % 30 == 0 || timerTime <= 10)
                {
                    timerAnimator.SetTrigger("textTrigger");
                }
                deltaTime = 0;
            }
        }
        else if (timerTime <= 0)
        {
            if (Game_Manager.GameStatus != Game_Manager.GameStatusEnum.Overed)
            {
                Game_Manager.GameStatus = Game_Manager.GameStatusEnum.Overed;
            }
        }
    }

    void DisableScoring()
    {
        doScore = false;
    }

    void Scoring(bool isCorrectlyDocked)
    {
        if (doScore && isCorrectlyDocked)
        {
            score += (100 + ((_bus.CurrentFuel / _bus.MaxFuel) * 100));
            UpdateScoreBoard();
        }
    }

    void UpdateScoreBoard()
    {
        scoreBoard.text = string.Format("Score: {0}", score.ToString("0000"));
        endScore.text = string.Format("Your Score: {0}", score.ToString("0000"));
    }

    void EnableStars()
    {
        float starScore = score;
        float scoreDivision = threeStarScore / 3;
        for (int i = 0; i < 3; i++)
        {
            starScore -= scoreDivision;
            if (starScore >= 0)
            {
                starsParent.transform.GetChild(i).gameObject.SetActive(true);
                _starCount++;
            }
            else
            {
                break;
            }
        }
        Level_Manager.Instance.SaveStars();
    }
    //void SaveStars()
    //{
    //    if (_starCount > PlayerPrefs.GetInt(("level" + Level_Manager.Instance.levelNum + "StarCount"), 0))
    //    {
    //        PlayerPrefs.SetInt(("level" + Level_Manager.Instance.levelNum + "StarCount"), _starCount);
    //    }
    //}
}
