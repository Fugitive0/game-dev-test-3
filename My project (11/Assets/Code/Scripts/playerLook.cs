using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerLook : MonoBehaviour
{
    // Public Variables

    
    [Header("Player Settings")]
    
    public float sensX = 90f;
    public float sensY = 90f;

    
    [Tooltip("Effects how far the player can look up")]
    
    public float clampX = 90f;

    [Header("Referenecs")] 
    public Transform playerOrientation;
    public Transform armOrientation;
    public Transform cameraRot;
    
    
    
    
    
    // Private Variables
    private float _xRot;
    private float _yRot;
    
    
    void Update()
    {
        PlayerLook();
    }


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        // Don't really understand this but it works

        _xRot -= mouseY;
        _yRot += mouseX;
        
        // Control the xRot by clamping it
        _xRot = Mathf.Clamp(_xRot, -clampX, clampX);
        
        // Apply the rotation to the transforms
        transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
        // Apply rotation to the player's orientation
        playerOrientation.localRotation = Quaternion.Euler(0, _yRot, 0);
        // Apply rotation to the player's arm's orientation
        armOrientation.localRotation = Quaternion.Euler(0, _yRot, 0);

    }
}
