using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunRecoil : MonoBehaviour
{
    private Vector3 _currRot;
    private Vector3 _targetRot;

    public gunData _gunData;

    public Transform camRot;
    public Transform refRot;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _targetRot = Vector3.Lerp(_targetRot, Vector3.zero, _gunData.returnSpeed * Time.deltaTime);
        _currRot = Vector3.Slerp(_currRot, _targetRot, _gunData.snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(_currRot);
        Vector3 camCurrRot = _currRot * 0.01f;
        Quaternion rotationDelta =  Quaternion.Euler(refRot.localRotation * camCurrRot);
        camRot.rotation = camRot.rotation * (rotationDelta);
    }

    public void RecoilFire()
    {
        if (!_gunData.reloading) _targetRot += new Vector3(_gunData.recoilX, Random.Range(-_gunData.recoilY, _gunData.recoilY),
            Random.Range(-_gunData.recoilZ, _gunData.recoilZ));
    }
}
