using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField]
    float _setAngularVelocityY;
    [SerializeField]
    Vector3 _originalPos;

    #region prop
    public Vector3 WorldAngularVelocity { get { return rb.angularVelocity; } }
    #endregion prop

    Vector3 angularVelocity;
    Rigidbody rb;

    void Start ()
    {
        _originalPos = transform.position;
        rb = GetComponent<Rigidbody>();
        angularVelocity = new Vector3(0, _setAngularVelocityY, 0);
        rb.angularVelocity = transform.TransformDirection(angularVelocity);// spin
    }

    void Update()
    {
        transform.position = _originalPos;//keep it at the same position;

    }

    void FixedUpdate ()
    {
        rb.angularVelocity = transform.TransformDirection(angularVelocity);//keep spinning
        //print(rb.angularVelocity);
    }

}
