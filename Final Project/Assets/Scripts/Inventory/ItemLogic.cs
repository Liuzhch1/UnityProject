using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogic : MonoBehaviour
{
    public ItemData_SO itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
        {
            //TODO: Add into inventory
        }
    }
}
