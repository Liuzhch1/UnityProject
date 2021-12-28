using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRobBulletLogic : MonoBehaviour
{
    float m_bulletLifeTime = 2.0f;

    float m_bulletSpeed = 40.0f;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = -transform.forward * m_bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bulletLifeTime >= 0)
        {
            m_bulletLifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<FPSplayerLogic>().TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
