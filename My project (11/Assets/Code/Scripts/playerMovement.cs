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
    public float hitGroundSink = 0.2f;
    public float waitToDetectGroundTime = 0.2f;
    public float bringBackCool = 0.2f;
    [Header("Jump Head Bobbing")]
    [SerializeField] private AnimationCurve headCurve;
    [Range(0.1f, 1f)] public float toStopMovingSpeed = 0.3f;


    [Header("References")] public Transform playerOrientation;
    public LayerMask ground;

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
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool initiatedCoolDown;





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




    }

    void Update()
    {
        GetInputs();
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
            _rb.AddForce(transform.up * (10 * jumpHeight), ForceMode.Impulse);
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

    /*

    private void JumpCameraShake()
    {
        // Scratch the last commit. I plan on making this a feature that allows the screen itself to shake. Not rotate
        // Rotating the camera to shake it does not seem like the best option. Instead, changing the position a little bit
        // Does

    

        if (_rb.velocity.y >= yVelKickIn && !_isGrounded)
        {
            _ogPos = camRot.localPosition;
            float shookX = Random.Range(0.3f, 0.6f);
            float shookY = Random.Range(0.3f, 0.6f);

            _targetPos = new Vector3(shookX, shookY, camRot.localPosition.z);
            
        }
        
        if (_rb.velocity.y <= 0 && !_isGrounded)
        {
            _targetPos = Vector3.zero;
            _elapsedTime = 0;
        }


        _elapsedTime += Time.deltaTime;
        float percentageComplete = _elapsedTime / duration;
        float smoothPercentage = Mathf.SmoothStep(0.0f, 1.0f, percentageComplete);
        camRot.localPosition = Vector3.Lerp(camRot.localPosition, _targetPos, smoothPercentage);

    }

    */

    // CoolDowns
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