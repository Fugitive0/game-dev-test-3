using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSwitching : MonoBehaviour
{

    public Transform[] weapons;
    public KeyCode[] keys;

    private float _timeSinceLastSwitch;
    private int _selectedWeapon;
    public float switchTime;
    
    // Start is called before the first frame update
    void Start()
    {
        SetWeapons();
        Select(_selectedWeapon);

        _timeSinceLastSwitch = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = _selectedWeapon;
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && _timeSinceLastSwitch >= switchTime)
                _selectedWeapon = i;
        }
        
        if(previousSelectedWeapon != _selectedWeapon) Select(_selectedWeapon);

        _timeSinceLastSwitch += Time.deltaTime;
    }


    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        if (keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        _timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected()
    {
        Debug.Log("Print hello");
    }
    
}
