using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class playerMovement : MonoBehaviour
{
    // Public Variables



    [Header("Player Settings")] public float moveSpeed;
    public float normalDrag = 6f;
    public float stopMovingDrag = 0.5f;
    public float airDrag = 2f;
    public float jumpHeight;
    public float jumpCoolDown = 0.3f;
    public float playerHeight;
    public float airSlowMuti = 3f;
    public float wallDetectDistance = 5f;
    public float fovIncrease = 20f;
    public float toNewFovTime = 5f;
    public float wallBoost = 35f;
    [Header("Jump Head Bobbing")]
    [SerializeField] private AnimationCurve headCurve;
    [Range(0.1f, 1f)] public float toStopMovingSpeed = 0.3f;


    [Header("References")] public Transform playerOrientation;
    public LayerMask ground;
    public LayerMask wall;
    public Camera cam;

    public TextMeshProUGUI playerMagText;
    public TextMeshProUGUI playerYVelText;

    [SerializeField] public List<TextMeshProUGUI> _textBlockList;

    [Header("Debug Settings")] [Tooltip("Show debug UI")]

    public bool turnDebugOn;




    // Private Variables
    private Rigidbody _rb;
    private Vector3 _moveDir;
    private Quaternion _camTargetRot;
    private Vector3 _targetPos;
    private Vector3 _ogPos;
    private float _horizontal;
    private float _vertical;
    private float _elapsedTime;
    private float ogFov;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool initiatedCoolDown;
    [SerializeField] private bool _isWalledRight;
    [SerializeField] private bool _isWalledLeft;
    [SerializeField] private bool _isWalled;
    [SerializeField] private bool _noLongerWalled;





    [Header("Head bob settings")] public float criteria = 0f;

    public float effectSpeed = 5f;

    public float effectAmpX = 2f;
    public float effectAmpY = 2f;
    public float defaultYPos;

    [Header("References")] public Transform camRot;

    // Private Variables
    private float _sinTime;

    // Start is called before the first frame update
    // Update is called once per frame

    private void Awake()
    {
    }

    private void Start()
    {
        // Freeze the rotation of the Rigid Body
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        // Subscribe to the methods
        playerActions.Jumping += Jumping;
        playerActions.TextToDisable += ReturnList;
        
        _textBlockList.Add(playerMagText);
        _textBlockList.Add(playerYVelText);

        ogFov = cam.fieldOfView;




    }

    void Update()
    {
        GetInputs();
        WallRunning();
        ControlDrag();
        ControlMag();
        CheckIfGrounded();
        // JumpCameraShake();
        DebugMode(turnDebugOn);
        
    }

    private void LateUpdate()
    {
        MovePlayer();
    }


    private void GetInputs()
    {
        _vertical = Input.GetAxisRaw("Vertical");
        _horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void MovePlayer()
    {
        _moveDir = playerOrientation.forward * _vertical + playerOrientation.right * _horizontal;
        _rb.AddForce(_moveDir.normalized * moveSpeed, ForceMode.Force);
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
    }

    private void Jumping()
    {
        if (_isGrounded && !_isJumping)
        {
            if (_isGrounded)
            {
                _rb.AddForce(transform.up * (10 * jumpHeight), ForceMode.Impulse);
            }
            _isJumping = true;
            StartCoroutine(JumpCoolDown());
        }
        
    }


    private void ControlDrag()
    {
        if (_rb.velocity.magnitude <= 3f && _isGrounded)
        {
            _rb.drag = stopMovingDrag;
        }
        else if (!_isGrounded)
        {
            _rb.drag = airDrag;
        }
        else
        {
            _rb.drag = normalDrag;
        }

    }
    
    private void ControlMag()
    {

        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVel.magnitude > moveSpeed && _isGrounded)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
        else if (flatVel.magnitude > airSlowMuti && !_isGrounded)
        {
            Vector3 limitedVel = flatVel.normalized * (airSlowMuti);
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    IEnumerator ToNotMovingCooldown()
    {
        yield return new WaitForSeconds(toStopMovingSpeed);
        _isMoving = false;

    }


    private void DebugMode(bool debug)
    {
        if (debug)
        {
            playerMagText.text = _rb.velocity.magnitude.ToString("F2");
            playerYVelText.text = _rb.velocity.y.ToString("F2");
            // Velocity Cast

            Debug.DrawLine(playerOrientation.position, transform.position + _rb.velocity, Color.red);
        }
    }


    private void WallRunning()
    {
        _isWalledLeft = Physics.Raycast(playerOrientation.position, -playerOrientation.right, wallDetectDistance, wall);
        _isWalledRight = Physics.Raycast(playerOrientation.position, playerOrientation.right, wallDetectDistance, wall);

        if (_isWalledLeft && Input.GetAxis("Horizontal") < 0 && !_isJumping && !_isGrounded)
        {
            _rb.AddForce(-playerOrientation.right * 45 + playerOrientation.forward * wallBoost, ForceMode.Force);
            _rb.useGravity = false;
            _isWalled = true;
            float newFov = Mathf.Lerp(ogFov, fovIncrease, toNewFovTime * Time.deltaTime);
        }
        
        else if (_isWalledRight && Input.GetAxis("Horizontal") > 0 && !_isJumping && !_isGrounded)
        {
            _rb.AddForce(playerOrientation.right * 45 + playerOrientation.forward * wallBoost, ForceMode.Force);
            _rb.useGravity = false;
            _isWalled = true;
            float newFov = Mathf.Lerp(ogFov, fovIncrease, toNewFovTime * Time.deltaTime);
        }
        else 
        {
            _rb.useGravity = true;
            _isWalled = false;

        }
        
    }

    IEnumerator WaitToFinishFovChange()
    {
        yield return new WaitForSeconds(3f);
        _noLongerWalled = false;
    }


    IEnumerator WallRunCoolDown()
    {
        yield return new WaitForSeconds(3f);
        _isWalled = false;
    }
    
    IEnumerator JumpCoolDown()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        _isJumping = false;
    }

    private List<TextMeshProUGUI> ReturnList()
    {
        return _textBlockList;
    }
    

}