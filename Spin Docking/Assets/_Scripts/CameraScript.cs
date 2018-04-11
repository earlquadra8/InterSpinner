using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    float _mouseSensitivity = 1;
    [SerializeField]
    Vector2 _offset = new Vector2(5, 0);
    [SerializeField]
    Vector2 _tiltLimit = new Vector2(-85, 85);

    GameObject _target;

    float _tilt;
    float _pan;

	void Start ()
    {
        _target = GameObject.FindGameObjectsWithTag("Player")[0];
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (!Cursor.visible)
        //    {
        //        Cursor.visible = true;
        //        Cursor.lockState = CursorLockMode.None;
        //    }
        //    else
        //    {
        //        Cursor.visible = false;
        //        Cursor.lockState = CursorLockMode.Locked;
        //    }
        //}
    }
    void FixedUpdate ()
    {
        SmoothLook();// it has to be go before SmoothFollow, or it will be bumping.
        SmoothFollow();
        //print(_pan);
    }

    void SmoothFollow()
    {
        if (_offset.x < 0)
        {
            _offset = new Vector2(0, _offset.y);
        }
        else if (_offset.x > 10)
        {
            _offset = new Vector3(10, _offset.y);
        }

        _offset -= new Vector2(Input.GetAxis("Mouse ScrollWheel"), 0);
        transform.position = _target.transform.position - transform.forward * _offset.x + transform.up * _offset.y;
    }

    void SmoothLook()
    {
        _pan += Input.GetAxis("Mouse X") * _mouseSensitivity;
        _tilt -= (Input.GetAxis("Mouse Y") * _mouseSensitivity);
        _tilt = Mathf.Clamp(_tilt, _tiltLimit.x, _tiltLimit.y);
        transform.eulerAngles = new Vector3(_tilt, _pan);

    }
}
