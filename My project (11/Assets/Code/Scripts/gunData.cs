using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class gunData : ScriptableObject
{


    [Header("Info")] private int ogAmmoCount;
    
    
    public new string name;
    public float damage;


    
    [Header("Gun settings")]
    public int currAmmo;
    public int magSize;
    public float fireRate;
    public float reloadTime;
    public bool reloading;
    public float maxDistance;
    public float toRecoilPos;

    [Header("Gun recoil settings")] 
    
    [Range(-100f, 100f)]
    public float recoilX;
    
    [Range(-100f, 100f)]
    public float recoilY;
    
    [Range(-100f, 100f)]
    public float recoilZ;
    
    [Range(-100f, 100f)]
    public float snappiness;

    [Range(-100f, 100f)] 
    public float returnSpeed;
    
    private void OnEnable()
    {
        
    }
}
