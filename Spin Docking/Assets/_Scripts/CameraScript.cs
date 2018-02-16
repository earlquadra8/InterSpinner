using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity;
    [SerializeField]
    Vector2 offset;

    GameObject target;

    float _pan;
    float _tilt;

	void Start ()
    {
        target = GameObject.FindGameObjectsWithTag("Player")[0];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    void FixedUpdate ()
    {
        SmoothLook();// it has to be go before SmoothFollow, or it will be bumping.
        SmoothFollow();
        //print(_pan);
    }

    void SmoothFollow()
    {
        if (offset.x < 0)
        {
            offset = new Vector2(0, offset.y);
        }
        else if (offset.x > 10)
        {
            offset = new Vector3(10, offset.y);
        }

        offset -= new Vector2(Input.GetAxis("Mouse ScrollWheel"), 0);
        transform.position = target.transform.position - transform.forward * offset.x + transform.up * offset.y;
    }

    void SmoothLook()
    {
        _tilt += Input.GetAxis("Mouse X") * mouseSensitivity;
        _pan -= (Input.GetAxis("Mouse Y") * mouseSensitivity);
        transform.eulerAngles = new Vector3(_pan, _tilt);

    }
}
