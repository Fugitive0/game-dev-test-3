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
    public float wallBoost = 35f;
    public float afterWallJumpDrag = 5f;
    public float wallJumpBoost = 5f;
    public float wallJumpBoostDir = 5f;
    public float maxSlopeAngle = 20f;
    public float dashAmount;
    public float dashCoolDown = 0.5f;
    public int allowGrapple;


    [Header("Grapple Settings")]
    
    
    public float maxGrappleDist = 100f;

    public float massScale = 4.5f;
    public float damperAmount = 2f;
    public float springAmount = 3f;
    
    
    
    [Header("Jump Head Bobbing")]
    [SerializeField] private AnimationCurve headCurve;
    [Range(0.1f, 1f)] public float toStopMovingSpeed = 0.3f;


    [Header("References")] public Transform playerOrientation;
    public LayerMask ground;
    public LayerMask wall;
    public LayerMask slope;
    public LayerMask whatIsGrapple;
    public Camera cam;
    public Transform grappleShootPoint, camPos, player;
    public Transform armOrientation;

    public TextMeshProUGUI playerMagText;
    public TextMeshProUGUI playerYVelText;

    [SerializeField] public List<TextMeshProUGUI> _textBlockList;

    [Header("Debug Settings")] [Tooltip("Show debug UI")]

    public bool turnDebugOn;




    // Private Variables

    private RaycastHit _slopeHit; 
    
    private Rigidbody _rb;
    private LineRenderer _ln;
    
    private Quaternion _camTargetRot;
    
    private Vector3 _moveDir;
    private Vector3 _targetPos;
    private Vector3 _ogPos;
    private Vector3 grapplePoint;

    private SpringJoint playerJoint;
    
    
    private float _horizontal;
    private float _vertical;
    private float _elapsedTime;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool initiatedCoolDown;
    [SerializeField] private bool _isWalledRight;
    [SerializeField] private bool _isWalledLeft;
    [SerializeField] private bool _isWalled;
    [SerializeField] private bool _noLongerWalled;
    [SerializeField] private bool _allowExtraMag;
    [SerializeField] private bool _ExtraMag;
    [SerializeField] private bool _isSloped;
    [SerializeField] private bool _isDashing;



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
    
    private void Start()
    {
        // Freeze the rotation of the Rigid Body
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _ln = GetComponent<LineRenderer>();

        // Subscribe to the methods
        playerActions.Jumping += Jumping;
        
        _textBlockList.Add(playerMagText);
        _textBlockList.Add(playerYVelText);
    }

    void Update()
    {

        _isSloped = OnSlope();
        
        GetInputs();
        WallRunning();
        Grapple();
        ControlDrag();
        PlayerDash();
        ControlMag();
        CheckIfGrounded();
        // JumpCameraShake();
        DebugMode(turnDebugOn);
    }
    

    private void FixedUpdate()
    {
        MovePlayer();
        WallRunning();
    }

    private void LateUpdate()
    {
        DrawRope();
    }


    private void GetInputs()
    {
        _vertical = Input.GetAxisRaw("Vertical");
        _horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void MovePlayer()
    {
            _moveDir = playerOrientation.forward * _vertical + playerOrientation.right * _horizontal;
            if (!OnSlope())
            {
                _rb.useGravity = true;
                _rb.AddForce(_moveDir.normalized * moveSpeed, ForceMode.Force);
            }
            else
            {
                _rb.useGravity = false;
                _rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Force);
            }
          
    }


    private void Grapple()
    {
        if (Input.GetMouseButtonDown(3))
        {
            StartGrapple();
        }

        else if (Input.GetMouseButtonUp(3))
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        _ln.positionCount = 0;
        Destroy(playerJoint);
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        int combinedLayerMask = wall.value | whatIsGrapple.value;
        if (Physics.Raycast(camPos.position, camPos.forward, out hit, maxGrappleDist, combinedLayerMask))
        {
            grapplePoint = hit.point;
            playerJoint = player.gameObject.AddComponent<SpringJoint>();
            playerJoint.autoConfigureConnectedAnchor = false;
            playerJoint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            playerJoint.maxDistance = distanceFromPoint * 0.8f;
            playerJoint.minDistance = distanceFromPoint * 0.25f;

            playerJoint.spring = springAmount;
            playerJoint.damper = damperAmount;
            playerJoint.massScale = massScale;

            _ln.positionCount = 2;
        }
    }

    private void DrawRope()
    {
        if(!playerJoint) return;
        _ln.SetPosition(0, grappleShootPoint.position);
        _ln.SetPosition(1, grapplePoint);
    }
    private void PlayerDash()
    {
        if (Input.GetMouseButtonDown(1) && !_isDashing)
        {
            _rb.AddForce(playerOrientation.forward * dashAmount, ForceMode.Impulse);
            _isDashing = true;
        }

        if (_isDashing)
        {
            StartCoroutine(DashCoolDown());
        }
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCoolDown);
        _isDashing = false;
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
    }

    private void Jumping()
    {
        if (_isGrounded || _isWalled && !_isJumping)
        {
            if (_isGrounded)
            {
                _rb.AddForce(transform.up * (10 * jumpHeight), ForceMode.Impulse);
            }

            if (_isWalled && _isWalledLeft)
            {
                _rb.AddForce(playerOrientation.right * wallJumpBoostDir + playerOrientation.up * wallJumpBoost, ForceMode.Impulse);
            }
            
            if (_isWalled && _isWalledRight)
            {
                _rb.AddForce(-playerOrientation.right * wallJumpBoostDir + playerOrientation.up * wallJumpBoost, ForceMode.Impulse);
            }
            _isJumping = true;
            StartCoroutine(JumpCoolDown());
        }
        
    }


    private void ControlDrag()
    {
        if (_rb.velocity.magnitude <= 3f && _isGrounded && !_ExtraMag)
        {
            _rb.drag = stopMovingDrag;
        }
        else if (!_isGrounded && !_ExtraMag)
        {
            _rb.drag = airDrag;
        }
        else if (_ExtraMag)
        {
            _rb.drag = afterWallJumpDrag;
        }
        else
        {
            _rb.drag = normalDrag;
        }

    }
    
    private void ControlMag()
    {

        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVel.magnitude > moveSpeed && _isGrounded && !_ExtraMag)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
        else if (flatVel.magnitude > airSlowMuti && !_isGrounded && !_ExtraMag)
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
            _rb.AddForce(armOrientation.forward * wallBoost, ForceMode.Force);
            _rb.useGravity = false;
            _isWalled = true;
            _allowExtraMag = true;
        }
        
        else if (_isWalledRight && Input.GetAxis("Horizontal") > 0 && !_isJumping && !_isGrounded)
        {
            _rb.AddForce(armOrientation.forward * wallBoost, ForceMode.Force);
            _rb.useGravity = false;
            _isWalled = true;
            _allowExtraMag = true;
        }
        else 
        {
            _rb.useGravity = true;
            _isWalled = false;
            
            if (_allowExtraMag && _isGrounded)
            {
                _ExtraMag = true;
                StartCoroutine(ExtraMagWindow());
            }
        }
        
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(playerOrientation.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDir, _slopeHit.normal).normalized;
    }
    
    
    IEnumerator ExtraMagWindow()
    {
        yield return new WaitForSeconds(2f);
        _ExtraMag = false;
        _allowExtraMag = false;
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
    
    

}