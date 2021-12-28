using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationCloudLogic : MonoBehaviour
{
    GameObject m_player;
    FPSplayerLogic m_playerController;
    float damageCooldown = 0.5f;
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
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Trigger!");
        FPSplayerLogic m_playerController = other.GetComponent<FPSplayerLogic>();
        if(m_playerController)
        {
            m_playerController.TakeDamage(10);
            Debug.Log("Take Damage!");
        }
    }

}
