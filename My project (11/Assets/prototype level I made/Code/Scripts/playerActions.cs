using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActions : MonoBehaviour
{
    [Header("KeyBinds")] public KeyCode jumpKey = KeyCode.Space;
    public static Action Jumping;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JumpAction();
    }

    private void JumpAction()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            Jumping?.Invoke();
        }
    }
}
