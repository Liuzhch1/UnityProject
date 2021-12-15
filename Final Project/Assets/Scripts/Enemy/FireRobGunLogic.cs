using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRobGunLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_bullet;
    [SerializeField]
    Transform m_bulletSpawnPoint;

    const float MAX_FIRE_COOLDOWN = 1.0f;
    float fire_cooldown = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
