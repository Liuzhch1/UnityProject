using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRobBulletLogic : MonoBehaviour
{
    float m_bulletLifeTime = 2.0f;

    float m_bulletSpeed = 10.0f;

    GameObject m_player;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        Vector3 toPlayer = m_player.transform.position - transform.position;
        Vector3 toPlayerPlane = new Vector3(toPlayer.x,0,toPlayer.z);
        toPlayerPlane -= 0.2f*transform.forward;
        GetComponent<Rigidbody>().velocity = toPlayerPlane * m_bulletSpeed;
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
            if (Random.Range(0.0f, 1.0f) < 0.3f)
            {
                return;
            }
            other.GetComponent<FPSplayerLogic>().TakeDamage(4);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
