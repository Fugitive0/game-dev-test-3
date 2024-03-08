using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class cameraSway : MonoBehaviour
{
    
    // Public Variables
    
    
    [Header("HeadSway Settings")] 
    
    public float headSwayAmount;

    public float rotAmount;

    [Range(0.01f, 1f)]
    
    public float rotSpeed = 0.01f ;


    [Header("References")] 
    public Rigidbody rb;

    public Transform playerOrientation;

    public Transform camRot;
    
    // Private Variables

    private Quaternion _ogRot;
    private Vector3 _targetRot;


    [Header("Debug Mode")] public bool isDebug;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _ogRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        DebugMode(isDebug);
        CameraSway();
    }

    private void CameraSway()
    {
        // Camera tilting from left to right sway
        
        
        float mouseY = Input.GetAxis("Horizontal");
        float rotZ = -mouseY * rotAmount;
        
        Quaternion finalRot = Quaternion.Euler(0f, rotZ, 0f);
        Quaternion targetRot = Quaternion.AngleAxis(rotZ, playerOrientation.transform.forward);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, rotSpeed);
        

    }

    private void DebugMode(bool debugModeOn)
    {
            // Draw line from the right side of the player orientation
            
        Debug.DrawLine(playerOrientation.position, playerOrientation.right  * 100f, Color.red);
        Debug.DrawLine(playerOrientation.position, -playerOrientation.right * 100f, Color.red);
            
    }
}
