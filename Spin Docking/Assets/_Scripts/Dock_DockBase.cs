using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dock_DockBase : MonoBehaviour
{
    public delegate void busDockingStatus(bool isDocked);
    public static event busDockingStatus busDockingStatusUpdated;

    public int dockID;
    public GameObject[] sensors;
    public GameObject stationDish;
    [Header("Text")]
    public Canvas dockCanvas;
    public Text dockIDText;
    public Text angularText;
    public Vector3 dockCanvasOffset;

    bool _isDocked = false;
    private void Start()
    {
        dockID = stationDish.transform.parent.transform.GetSiblingIndex();
        dockCanvas.transform.position = transform.TransformPoint(transform.localPosition + dockCanvasOffset);
    }
    private void Update()
    {
        if (dockCanvas != null)
        {
            dockCanvas.transform.rotation = Quaternion.LookRotation(dockCanvas.transform.position - Camera.main.transform.position);
        }
        if (dockIDText != null)
        {
            dockIDText.text = "Dock: " + dockID.ToString("00");
        }
        if (angularText != null)
        {
            angularText.text = "Angular Speed:\n" + stationDish.GetComponent<Station>().WorldAngularVelocity.magnitude.ToString("00.0");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Bus bus = other.GetComponent<Bus>();
            if (CheckIfAllSensorsConnected())
            {
                bus.CanControlSpin = false;
                bus.SetWorldAngularVelocity = stationDish.GetComponent<Station>().WorldAngularVelocity;
                _isDocked = true;
                if (busDockingStatusUpdated != null && Game_Manager.NextDockID == dockID)// check if in the right dock
                {
                    busDockingStatusUpdated(_isDocked);
                    bus.CurrentFuel = bus.MaxFuel;
                }
            }
            else
            {
                bus.CanControlSpin = true;
                _isDocked = false;
                if (busDockingStatusUpdated != null)
                {
                    busDockingStatusUpdated(_isDocked);
                }
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        other.GetComponent<Bus>().CanControlSpin = true;
    //        if (busDockingStatusUpdated != null)
    //        {
    //            _isDocked = false;
    //            busDockingStatusUpdated(_isDocked);
    //        }
    //    }
    //}

    bool CheckIfAllSensorsConnected()
    {
        bool isAllSensorsConnected = false;
        foreach (GameObject sensor in sensors)
        {
            if (sensor.GetComponent<Dock_Sensors>() != null)
            {
                if (sensor.GetComponent<Dock_Sensors>().IsConnected)
                {
                    isAllSensorsConnected = sensor.GetComponent<Dock_Sensors>().IsConnected;
                }
                else
                {
                    isAllSensorsConnected = sensor.GetComponent<Dock_Sensors>().IsConnected;
                    break;
                }
            }
        }
        return isAllSensorsConnected;
    }
}
