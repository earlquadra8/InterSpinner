using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    public delegate void busFuelState();
    public event busFuelState busFuelEmptied;

    [SerializeField]
    float _thrustForce;
    [SerializeField]
    float _spinForce;
    [SerializeField]
    float _maxFuel;
    [SerializeField]
    float _currentFuel;
    [SerializeField]
    float _fuelConsumptionRate;
    [SerializeField]
    float _fullBrakeRatio;
    [SerializeField]
    float _fullBrakeDriftThreshold;
    [SerializeField]
    float _fullBrakeSpinThreshold;
    [SerializeField]
    Vector3 _stopSpinThreshold;
    [SerializeField]
    Vector3 _stopDriftThreshold;
    [SerializeField]
    GameObject[] _thrusterParticle;

    bool _canControlSpin = true;
    bool[] _activeThruster;
    float _horizontal;
    float _vertical;
    float _forward;
    float _yaw;
    float _pitch;
    float _roll;

    Vector3 _localAngularVelocity;
    Vector3 _localVelocity;
    Rigidbody _rb;
    AudioSource _thrusterSFX;

    Level_Manager _levelManager;

    #region prop
    public bool CanControlSpin
    {
        get { return _canControlSpin; }
        set { _canControlSpin = value; }
    }
    public Vector3 LocalAngularVelocity
    {
        get
        { return _localAngularVelocity; }
   }
    public Vector3 LocalVelocity
    {
        get
        {
            return _localVelocity;
        }
        set
        {
            if (_localVelocity.y < 0)
            {
                _localVelocity = new Vector3(_localVelocity.x, value.y, _localVelocity.z);
            }
        }
    }
    public Vector3 SetWorldAngularVelocity
    {
        set
        {
            if (!_canControlSpin)
            {
                _rb.angularVelocity = value;
            }
        }
    }

    public float MaxFuel { get { return _maxFuel; } set { _maxFuel = value; } }
    public float CurrentFuel { get { return _currentFuel; } set { _currentFuel = value; } }
    #endregion prop

    #region thruster particle enum
    enum thrusterNum : int//Rear, Front; Up, Low; Left, Right; Main, Sub;
    {
        RULM,
        RURM,
        RLLM,
        RLRM,
        RULS,
        RURS,
        RLLS,
        RLRS,
        FULM,
        FURM,
        FLLM,
        FLRM,
        FULS,
        FURS,
        FLLS,
        FLRS,
    }
    # endregion thruster particle enum
    private void OnEnable()
    {
        Game_Manager.GameStatusChanged += FuelEndGameChange;
    }
    private void OnDisable()
    {
        Game_Manager.GameStatusChanged -= FuelEndGameChange;

    }
    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _thrusterSFX = GetComponent<AudioSource>();
        _levelManager = FindObjectOfType<Level_Manager>();//pull value for initialization instead of push, Returns the first active loaded object of Type type.
        _maxFuel = _levelManager.levelMaxFuel;
        _currentFuel = _maxFuel;
        _activeThruster = new bool[_thrusterParticle.Length];

        
    }
	void FixedUpdate ()
    {
        Movement();
        FullBrake();
    }

    void Movement()
    {
        _localAngularVelocity = transform.InverseTransformDirection(_rb.angularVelocity);
        _localVelocity = transform.InverseTransformVector(_rb.velocity);

        //postion
        _horizontal = Input.GetAxisRaw("Horizontal");//x
        _vertical = Input.GetAxisRaw("Vertical");//y
        _forward = Input.GetAxisRaw("Forward");//z
        //rotation
        _pitch = Input.GetAxisRaw("Pitch");//x
        _yaw = Input.GetAxisRaw("Yaw");//y
        _roll = Input.GetAxisRaw("Roll");//z

        Vector3 movement = new Vector3(_horizontal, _vertical, _forward);
        Vector3 rotation = new Vector3(_pitch, _yaw, _roll);

        if (_currentFuel > 0)
        {
            _rb.AddRelativeForce(movement * _thrustForce);
            if (_canControlSpin)
            {
                _rb.AddRelativeTorque(rotation * _spinForce);
            }
        }
            ThrusterVFXControl(_horizontal, _vertical, _forward, _pitch, _yaw, _roll);
            ThrusterSFXControl(_horizontal, _vertical, _forward, _pitch, _yaw, _roll);

        BrakeSpinning();
        BrakeDrifting();

        FuelConsumption(_horizontal, _vertical, _forward, _pitch, _yaw, _roll, 1f);

    }
    void BrakeSpinning()//Halt the spin if angular speed is lower than the threshold.
    {
        if (Mathf.Abs(_localAngularVelocity.x) <= _stopSpinThreshold.x && _pitch == 0)
        {
            _rb.angularVelocity -= transform.TransformDirection(new Vector3(_localAngularVelocity.x/3, 0, 0));
        }
        if (Mathf.Abs(_localAngularVelocity.y) <= _stopSpinThreshold.y && _yaw == 0)
        {
            _rb.angularVelocity -= transform.TransformDirection(new Vector3(0, _localAngularVelocity.y/3, 0));
        }
        if (Mathf.Abs(_localAngularVelocity.z) <= _stopSpinThreshold.z && _roll == 0)
        {
            _rb.angularVelocity -= transform.TransformDirection(new Vector3(0, 0, _localAngularVelocity.z / 3));
        }
    }
    void BrakeDrifting()//Halt the drift if angular speed is lower than the threshold.
    {
        if (Mathf.Abs(_localVelocity.x) <= _stopDriftThreshold.x && _horizontal == 0)
        {
            _rb.velocity -= transform.TransformDirection(new Vector3(_localVelocity.x/3, 0.0000f, 0.0000f));
        }
        if (Mathf.Abs(_localVelocity.y) <= _stopDriftThreshold.y && _vertical == 0)
        {
            _rb.velocity -= transform.TransformDirection(new Vector3(0.0000f, _localVelocity.y/3, 0.0000f));
        }

        if (Mathf.Abs(_localVelocity.z) <= _stopDriftThreshold.z && _forward == 0)
        {
            _rb.velocity -= transform.TransformDirection(new Vector3(0.0000f, 0.0000f, _localVelocity.z/3));
        }
    }
    void FuelConsumption(float hor, float ver, float frw, float pit, float yaw, float rol, float ratio)
    {
        float consumptionIndex
            = (Mathf.Abs(hor)
            + Mathf.Abs(ver)
            + Mathf.Abs(frw)
            + Mathf.Abs(pit) * 0.3f
            + Mathf.Abs(yaw) * 0.3f
            + Mathf.Abs(rol) * 0.3f) * ratio;
        if (_currentFuel > 0)
        {
            _currentFuel -= consumptionIndex * _fuelConsumptionRate;
        }
        else
        {
            _currentFuel = 0.000f;
            FuelEmptiedEvent();
        }
    }
    bool fuelEmptiedEventRaised = false;
    void FuelEmptiedEvent()
    {
        if (busFuelEmptied != null)
        {
            busFuelEmptied();
        }
    }
    void FuelEndGameChange(Game_Manager.GameStatusEnum status)
    {
        if (status == Game_Manager.GameStatusEnum.Overed)
        {
            _currentFuel = 9999;
            _fuelConsumptionRate = 0;
        }
    }

    bool isFullBrakeing; // added for checking if able to shut off thrusters; shut off thrusters if not match Fullbrake conditions
    void FullBrake()
    {
        if (Input.GetButton("FullBrake") && _canControlSpin && _currentFuel > 0 && (_rb.velocity != Vector3.zero || _rb.angularVelocity != Vector3.zero))
        {
            isFullBrakeing = true;
            if (_rb.velocity.magnitude >= _fullBrakeDriftThreshold)// shut the motion instead of lerp it to zero slowly, as the velocity is very low below this threshold.
            {
                _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, _fullBrakeRatio * Time.fixedDeltaTime);
            }
            else
            {
                _rb.velocity = Vector3.zero;
            }

            if (_rb.angularVelocity.magnitude >= _fullBrakeSpinThreshold)
            {
                _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, _fullBrakeRatio * Time.fixedDeltaTime);
            }
            else
            {
                _rb.angularVelocity = Vector3.zero;
            }

            FuelConsumption(_localVelocity.x, _localVelocity.y, _localVelocity.z, _localAngularVelocity.x, _localAngularVelocity.y, _localAngularVelocity.z, 0.25f);
            ThrusterVFXControl(-_localVelocity.x, -_localVelocity.y, -_localVelocity.z, -_localAngularVelocity.x, -_localAngularVelocity.y, -_localAngularVelocity.z);
            ThrusterSFXControl(-_localVelocity.x, -_localVelocity.y, -_localVelocity.z, -_localAngularVelocity.x, -_localAngularVelocity.y, -_localAngularVelocity.z);
        }
        else
        {
            isFullBrakeing = false;
        }
    }
    void ThrusterVFXControl(float hor, float ver, float frw, float pit, float yaw, float rol)
    {
        #region particle enable thusterNum[], define thruster set to be enabled according to possible movement
        thrusterNum[] horP = new thrusterNum[] { thrusterNum.RULS, thrusterNum.RLLS, thrusterNum.FULS, thrusterNum.FLLS };
        thrusterNum[] horM = new thrusterNum[] { thrusterNum.RURS, thrusterNum.RLRS, thrusterNum.FURS, thrusterNum.FLRS };
        thrusterNum[] verP = new thrusterNum[] { thrusterNum.RLLS, thrusterNum.RLRS, thrusterNum.FLLS, thrusterNum.FLRS };
        thrusterNum[] verM = new thrusterNum[] { thrusterNum.RULS, thrusterNum.RURS, thrusterNum.FULS, thrusterNum.FURS };
        thrusterNum[] frwP = new thrusterNum[] { thrusterNum.RULM, thrusterNum.RURM, thrusterNum.RLLM, thrusterNum.RLRM };
        thrusterNum[] frwM = new thrusterNum[] { thrusterNum.FULM, thrusterNum.FURM, thrusterNum.FLLM, thrusterNum.FLRM };
        thrusterNum[] pitP = new thrusterNum[] { thrusterNum.FULS, thrusterNum.FURS, thrusterNum.RLLS, thrusterNum.RLRS };
        thrusterNum[] pitM = new thrusterNum[] { thrusterNum.RULS, thrusterNum.RURS, thrusterNum.FLLS, thrusterNum.FLRS };
        thrusterNum[] yawP = new thrusterNum[] { thrusterNum.FULS, thrusterNum.FLLS, thrusterNum.RURS, thrusterNum.RLRS, thrusterNum.FURM, thrusterNum.FLRM, thrusterNum.RULM, thrusterNum.RLLM };
        thrusterNum[] yawM = new thrusterNum[] { thrusterNum.FURS, thrusterNum.FLRS, thrusterNum.RULS, thrusterNum.RLLS, thrusterNum.FULM, thrusterNum.FLLM, thrusterNum.RURM, thrusterNum.RLRM };
        thrusterNum[] rolP = new thrusterNum[] { thrusterNum.FULS, thrusterNum.RULS, thrusterNum.FLRS, thrusterNum.RLRS };
        thrusterNum[] rolM = new thrusterNum[] { thrusterNum.FLLS, thrusterNum.RLLS, thrusterNum.FURS, thrusterNum.RURS };
        #endregion particle enable thusterNum[]

        if (!isFullBrakeing || _currentFuel <= 0)// FullBrakeing is a special condition, bcoz thrust would be stopped while having FullBrake button pressed, unlike the other input that thrust will be stopped while have no button pressed.
        {
            for (int i = 0; i < _activeThruster.Length; i++)
            {
                _activeThruster[i] = false;
            }
        }

        if (_currentFuel > 0)
        {
            #region activate thruster sets according to conditions
            if (hor > 0)
            {
                ActivateThrusterSet(_activeThruster, horP);
            }
            else if (hor < 0)
            {
                ActivateThrusterSet(_activeThruster, horM);
            }
            if (ver > 0)
            {
                ActivateThrusterSet(_activeThruster, verP);
            }
            else if (ver < 0)
            {
                ActivateThrusterSet(_activeThruster, verM);
            }
            if (frw > 0)
            {
                ActivateThrusterSet(_activeThruster, frwP);
            }
            else if (frw < 0)
            {
                ActivateThrusterSet(_activeThruster, frwM);
            }
            if (pit > 0)
            {
                ActivateThrusterSet(_activeThruster, pitP);
            }
            else if (pit < 0)
            {
                ActivateThrusterSet(_activeThruster, pitM);
            }
            if (yaw > 0)
            {
                ActivateThrusterSet(_activeThruster, yawP);
            }
            else if (yaw < 0)
            {
                ActivateThrusterSet(_activeThruster, yawM);
            }
            if (rol > 0)
            {
                ActivateThrusterSet(_activeThruster, rolP);
            }
            else if (rol < 0)
            {
                ActivateThrusterSet(_activeThruster, rolM);
            }
            #endregion activate thruster sets according to conditions
        }
        for (int i = 0; i < _activeThruster.Length; i++)
        {
            _thrusterParticle[i].SetActive(_activeThruster[i]);
        }

    }
    void ActivateThrusterSet(bool[] activateT, thrusterNum[] thrusterSets)
    {
        foreach (thrusterNum set in thrusterSets)
        {
            activateT[(int)set] = true;
        }
    }
    void ThrusterSFXControl(float hor, float ver, float frw, float pit, float yaw, float rol)
    {
        if ((hor != 0 || ver != 0 || frw != 0 || pit != 0 || yaw != 0 || rol != 0) && _currentFuel > 0)
        {
            if (!_thrusterSFX.isPlaying)
            {
                _thrusterSFX.Play();
            }
        }
        else if(!isFullBrakeing || _currentFuel <= 0)
        {
            _thrusterSFX.Stop();
        }
    }

    void TheNoodAssist()
    {
        #region n
        if (Input.GetKey(KeyCode.N))
        {
            int targetDockID = Game_Manager.NextDockID;
            GameObject targetDock = GameObject.Find("Stations_Parent").transform.GetChild(targetDockID).gameObject;
            StationType targetType = targetDock.transform.GetComponentInChildren<Station_Type>().type;
            if (targetType == StationType.A)
            {
                //transform.up = targetDock.transform.up;
                Vector3 newDir = Vector3.RotateTowards(transform.up, targetDock.transform.up, 0.05f, 0);
                transform.up = newDir;
                print(newDir);
            }
            else if (targetType == StationType.B)
            {
                //transform.forward = -targetDock.transform.up;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, -targetDock.transform.up, 0.05f, 0);
                transform.forward = newDir;
                print(newDir);

            }
            else if (targetType == StationType.C)
            {
                //transform.right = -targetDock.transform.up;
                Vector3 newDir = Vector3.RotateTowards(transform.right, -targetDock.transform.up, 0.05f, 0);
                transform.right = newDir;
                print(newDir);

            }
        }
        #endregion
    }// Maybe someday
}
