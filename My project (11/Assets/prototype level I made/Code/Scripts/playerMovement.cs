using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Public Variables
    
    
    
    [Header("Player Settings")]
    
    public float moveSpeed;
    public float normalDrag = 6f;
    public float stopMovingDrag = 0.5f;
    public float airDrag = 2f;
    public float jumpHeight;
    public float jumpCoolDown = 0.3f;
    public float playerHeight;
    public float airSlowMuti = 3f;
    
    
    
    public float airMaxXVel = 180f;
    public float airMaxYVel = 180f;
    public float airMaxZVel = 180f;
    
    
    public float groundCastLength = 2f;
    [Range(0.1f, 1f)] public float toStopMovingSpeed = 0.3f;
    
    
    [Header("References")]
    public Transform playerOrientation;

    public Transform rayRight;
    public Transform rayLeft;

    public Transform groundCast;
    public LayerMask ground;
    
    public TextMeshProUGUI playerMagText;

    [Header("Debug Settings")] [Tooltip("Show debug UI")]

    public bool turnDebugOn;
    
    
    
    
    // Private Variables
    private Rigidbody _rb;
    private Vector3 _moveDir;
    private float _horizontal;
    private float _vertical;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isJumping;
    
    
    
    
    
    [Header("Head bob settings")] 
    public float criteria = 0f;

    public float effectSpeed = 5f;

    public float effectAmpX = 2f;
    public float effectAmpY = 2f;
    public float defaultYPos;
    
    [Header("References")]

    public Transform camRot;
    // Private Variables
    private float _sinTime;
    
    // Start is called before the first frame update
    // Update is called once per frame

    private void Awake()
    {
        defaultYPos = camRot.localPosition.y;
    }

    private void Start()
    {
        // Freeze the rotation of the Rigid Body
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        
        // Subscribe to the methods

        playerActions.Jumping += Jumping;


    }

    void Update()
    {
        GetInputs();
        ControlDrag();
        ControlMag();
        CheckIfGrounded();
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
        else if(!_isGrounded)
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
        else if(flatVel.magnitude > airSlowMuti && !_isGrounded)
        {
            Vector3 limitedVel = flatVel.normalized * (airSlowMuti);
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }


    // Not in use and incompleted
    
    private void HeadBobbing()
    {
        if (_rb.velocity.magnitude > criteria && _isGrounded)
        {
            _sinTime += Time.deltaTime * effectSpeed;
            camRot.localPosition = new Vector3(transform.localPosition.x,
                defaultYPos + Mathf.Sin(_sinTime) * effectAmpX, camRot.localPosition.z);
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
            
            // Velocity Cast
            
            Debug.DrawLine(playerOrientation.position, transform.position + _rb.velocity, Color.red);
            
            // Wall Running Cast
            
            Debug.DrawLine(rayLeft.position, -rayLeft.right * 10, Color.magenta);
            Debug.DrawLine(rayLeft.position, rayLeft.right * 10, Color.magenta);
            
           
            
            
            
        }
    }
    
    
    
    // CoolDowns

    IEnumerator JumpCoolDown()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        _isJumping = false;
    }
    
    
    
}
