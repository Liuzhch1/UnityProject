using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItemLogic : MonoBehaviour
{
    public void pickUp(){
        Destroy(gameObject);
    }
}
