using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationCloudLogic : MonoBehaviour
{
    GameObject m_player;
    FPSplayerLogic m_playerController;
    float damageCooldown = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player)
        {
            m_playerController = m_player.GetComponent<FPSplayerLogic>();
        }
    }
    void Update() {
        if(damageCooldown > 0.0f){
            damageCooldown -= Time.deltaTime;
        }
    }
    void OnTriggerStay(Collider other)
    {
        FPSplayerLogic m_playerController = other.GetComponent<FPSplayerLogic>();
        if(m_playerController && damageCooldown <= 0.0f)
        {
            m_playerController.TakeDamage(1);
            damageCooldown = 1.0f;
        }
    }

}
