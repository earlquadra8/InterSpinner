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
                if (busDockingStatusUpdated != null && Game_Manager.NextDockID == dockID)// check if in the right dock
                {
                    _isDocked = true;
                    if (busDockingStatusUpdated != null)
                    {
                        busDockingStatusUpdated(_isDocked);
                    }
                    bus.CurrentFuel = bus.MaxFuel;
                }
            }
            else
            {
                if (_isDocked)
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
    }
    private void OnTriggerExit(Collider other)// in rare case, the else if in OnTriggerStay is not called for unknown reason, it would duct tape it.
    {
        if (other.tag == "Player")
        {
            print("_isDocked " + _isDocked + " | CanControlSpin " + other.GetComponent<Bus>().CanControlSpin);
            if (!other.GetComponent<Bus>().CanControlSpin)
            {
                other.GetComponent<Bus>().CanControlSpin = true;
            }

            if(_isDocked)
            { 
                _isDocked = false;
                if (busDockingStatusUpdated != null)
                {
                    busDockingStatusUpdated(_isDocked);
                }
            }
        }
    }

    bool CheckIfAllSensorsConnected()
    {
        foreach (GameObject sensor in sensors)
        {
            if (sensor.GetComponent<Dock_Sensors>() != null)
            {
                if (!sensor.GetComponent<Dock_Sensors>().IsConnected)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
