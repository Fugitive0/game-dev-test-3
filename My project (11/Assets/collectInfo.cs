using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class collectInfo : MonoBehaviour
{
    public playerMovement pm;
    public gunRecoil gr;
    public Transform arms;
    public gunData gd;


    private List<TextMeshProUGUI> InfoList;
    
    // Info Boxes

    public float playerSpeed;
    public float bulletCount;
    public string currentGun;
    public float currentGunMag;
    
    // TextBoxs 

    public TextMeshProUGUI playerSpeedText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI currentGunText;
    public TextMeshProUGUI currentGunMagText;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        MakeList();
        returnList();
    }

    // Update is called once per frame
    void Update()
    {
        gd = arms.GetComponent<gunData>();
    }



    private void MakeList()
    {
        bulletCount = gd.currAmmo;
        currentGun = gd.name;
        currentGunMag = gd.magSize;
        playerSpeed = pm.GetComponent<Rigidbody>().velocity.magnitude;
        
    }
    
    private void returnList()
    {
        currentGunText.text = currentGun;
        playerSpeedText.text = playerSpeed.ToString("f2");
        bulletCountText.text = bulletCount.ToString();
        currentGunMagText.text = currentGunMag.ToString();

    }
}
