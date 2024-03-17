using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class gun : MonoBehaviour
{
    [SerializeField] private gunData _gunData;
    private float _timeSinceLastShot;

    [Header("References")]
    public Transform cam;

    public Transform arms;

    public gunRecoil GunRecoil;
    

    private bool CanShoot() => !_gunData.reloading && _timeSinceLastShot > 1f / (_gunData.fireRate / 60f);
    
    // Start is called before the first frame update
    void Start()
    {
        playerActions.GunShoot += Shoot;
        playerActions.GunReload += GunReload;
        arms.GetComponent<gunRecoil>();
    }

    // Update is called once per frame
    void Update()
    {
        GunRecoil._gunData = _gunData;
        _timeSinceLastShot += Time.deltaTime;
    }

    private void Shoot()
    {
        if (_gunData.currAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, _gunData.maxDistance))
                {
                    Debug.Log("You hit: " + hitInfo.transform.name);
                }
                
                _gunData.currAmmo--;
                _timeSinceLastShot = 0;
                OnGunShot();
            }
            
        }
        
        
    }

    private void OnDisable() => _gunData.reloading = false;

    private void GunReload()
    {
        if (!_gunData.reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        _gunData.reloading = true;
        yield return new WaitForSeconds(_gunData.reloadTime);
        _gunData.currAmmo = _gunData.magSize;
        _gunData.reloading = false;
    }
    
    
    private void OnGunShot()
    {
        GunRecoil.RecoilFire();
    }
    
    
}
