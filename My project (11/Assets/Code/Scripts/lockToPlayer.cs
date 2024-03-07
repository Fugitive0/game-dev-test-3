using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockToPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("References")] public Transform player;
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = player.localPosition;
    }
}
