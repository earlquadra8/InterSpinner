using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock_Sensors : MonoBehaviour
{

    bool _isConnected = false;

    public SensorColor sensorColor;

    #region prop
    public bool IsConnected { get { return _isConnected; } }
    #endregion prop

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Bus_Rod>() != null)
        {
            if (other.GetComponent<Bus_Rod>().rodColor == this.sensorColor)
            {
                _isConnected = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Bus_Rod>() != null)
        {
            if (other.GetComponent<Bus_Rod>().rodColor == this.sensorColor)
            {
                _isConnected = false;
            }
        }
    }

}
