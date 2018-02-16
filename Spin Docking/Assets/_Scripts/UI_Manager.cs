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

    public GameObject pausePanel;
    public GameObject endPanel;
    public Text anyKey;

    GameObject busGObj;
    Bus bus;


    private void Awake()
    {
        _instance = this;
        print("UI Awake " + timerTime);
    }
    #region OnEnable & OnDisable
    private void OnEnable()
    {
        Game_Manager.NextDockIDGenerated += UpdateNextDockText;
        Game_Manager.GameStatusChanged += ToggleGuide;
        Game_Manager.GameStatusChanged += ToggleEndPanel;
        Dock_DockBase.busDockingStatusUpdated += UpdateDockStatusText;

        print("UI Enable "+ timerTime);
    }
    private void OnDisable()
    {
        Game_Manager.NextDockIDGenerated -= UpdateNextDockText;
        Game_Manager.GameStatusChanged -= ToggleGuide;
        Game_Manager.GameStatusChanged -= ToggleEndPanel;
        Dock_DockBase.busDockingStatusUpdated -= UpdateDockStatusText;
    }
    #endregion OnEnable & OnDisable
    void Start ()
    {
        busGObj = GameObject.FindGameObjectsWithTag("Player")[0];
        bus = busGObj.GetComponent<Bus>();
        UpdateDockStatusText(false);// initialize
        endPanel.SetActive(false);

        timerText.text = string.Format("Timer: {0}", timerTime.ToString("000"));
        print("UI Start " + timerTime);
    }

    void Update ()
    {
        UpdateMeters();
        TimerUpdate();
    }

    void UpdateMeters()
    {
        angularMeter.text = string.Format("Angular Velocity |X: {0} |Y: {1} |Z: {2}", bus.LocalAngularVelocity.x.ToString(" +00.0;-00.0"), bus.LocalAngularVelocity.y.ToString(" +00.0;-00.0"), bus.LocalAngularVelocity.z.ToString(" +00.0;-00.0"));
        driftMeter.text = string.Format("Drifting Velocity |X: {0} |Y: {1} |Z: {2}", bus.LocalVelocity.x.ToString("+00.0;-00.0"), bus.LocalVelocity.y.ToString("+00.0;-00.0"), bus.LocalVelocity.z.ToString("+00.0;-00.0"));
        fuelMeter.text = "Fuel: " + bus.CurrentFuel.ToString("0000");
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
        }
    }
    public void EndPanelCrossButton()
    {
        if (endPanel.activeSelf)
        {
            endPanel.SetActive(false);
        }
    }
    
    IEnumerator ShowAnyKey()
    {
        yield return new WaitForSecondsRealtime(3);
        anyKey.gameObject.SetActive(true);
    }

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
}
