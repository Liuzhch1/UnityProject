using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeLogic : MonoBehaviour
{
    bool attackEnable = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!attackEnable) return;
        if (other.gameObject.tag == "Enemy01")
        {

        }
    }
}
