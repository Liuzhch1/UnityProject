using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    float m_bulletLifeTime = 2.0f;

    float m_bulletSpeed = 20.0f;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * m_bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
