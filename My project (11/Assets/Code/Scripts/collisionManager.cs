using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionManager : MonoBehaviour
{

    [Header("References")] public Transform respawnPoint;
    
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.transform.position = respawnPoint.position;
    }
}
