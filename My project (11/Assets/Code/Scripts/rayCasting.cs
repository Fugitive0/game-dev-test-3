using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayCasting : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, 10);
        Debug.DrawLine(transform.position, transform.forward * 10, Color.red);
    }
    
    
}
