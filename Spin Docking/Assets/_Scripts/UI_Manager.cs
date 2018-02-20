using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager _instance;
    public static UI_Manager Instance { get { return _instance; } }

    public Text nextDockID;
    public Text angularMeter;
    public Text driftMeter;
    public Text fuelMeter;
    public Text dockStatus;

    public bool doTimer;
    public float timerTime;
    public Text timerText;

    public bool doScore;
    public float score;
    public Text scoreBoard;

    public GameObject pausePanel;

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
        Game_Manager.GameStatusChanged += ToggleEndPanel;
        Game_Manager.GameStatusChanged += DisableScoring;
        Dock_DockBase.busDockingStatusUpdated += UpdateDockStatusText;
        Dock_DockBase.busDockingStatusUpdated += Scoring;
    }
    private void OnDisable()
    {
        Game_Manager.NextDockIDGenerated -= UpdateNextDockText;
        Game_Manager.GameStatusChanged -= ToggleGuide;
        Game_Manager.GameStatusChanged -= ToggleEndPanel;
        Game_Manager.GameStatusChanged -= DisableScoring;
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
        TimerUpdate();
    }

    void UpdateMeters()
    {
        angularMeter.text = string.Format("Angular Velocity |X: {0} |Y: {1} |Z: {2}", _bus.LocalAngularVelocity.x.ToString(" +00.0;-00.0"), _bus.LocalAngularVelocity.y.ToString(" +00.0;-00.0"), _bus.LocalAngularVelocity.z.ToString(" +00.0;-00.0"));
        driftMeter.text = string.Format("Drifting Velocity |X: {0} |Y: {1} |Z: {2}", _bus.LocalVelocity.x.ToString("+00.0;-00.0"), _bus.LocalVelocity.y.ToString("+00.0;-00.0"), _bus.LocalVelocity.z.ToString("+00.0;-00.0"));
        fuelMeter.text = "Fuel: " + _bus.CurrentFuel.ToString("0000");
    }

    void UpdateNextDockText()
    {
        nextDockID.text = "Going to Dock " + Game_Manager.NextDockID.ToString("00");
    }

    void UpdateDockStatusText(bool isDocked)
    {
        if (isDocked)
        {
            dockStatus.text = "Status: Docked";
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
    void ToggleEndPanel(Game_Manager.GameStatusEnum status)
    {
        if (status == Game_Manager.GameStatusEnum.Overed)
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
    }
    public void EndPanelCrossButton()
    {
        if (endPanel.activeSelf)
        {
            endPanel.SetActive(false);
        }
    }
    
    //IEnumerator ShowAnyKey()
    //{
    //    yield return new WaitForSecondsRealtime(3);
    //    anyKey.gameObject.SetActive(true);
    //}

    void TimerUpdate()
    {
        if (doTimer && timerTime > 0.000f)
        {
            timerTime -= Time.deltaTime;
        }
        else if(timerTime <= 0.000f)
        {
            timerTime = 0;
            Game_Manager.GameStatus = Game_Manager.GameStatusEnum.Overed;
        }
        timerText.text = string.Format("Timer: {0}", timerTime.ToString("000"));
    }

    void DisableScoring(Game_Manager.GameStatusEnum status)
    {
        if (status == Game_Manager.GameStatusEnum.Overed)
        {
            doScore = false;
        }
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
        EnableStars();
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
        }
    }
}
