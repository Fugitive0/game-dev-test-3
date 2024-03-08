using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayCasting : MonoBehaviour
{
    [Header("Ray Cast Settings")] public float maxDistance = 50f;


    private RaycastHit rayHit;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out rayHit, maxDistance);


        if (rayHit.collider != null)
        {
            if (rayHit.transform.gameObject.name == "enemy")
            {
                Debug.Log("Hit enemy");
            }
            
        }
      
    }
    
    
}
