using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum CloseEnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public class CloseEnemyLogic : MonoBehaviour
{
    GameObject m_player;
    FPSplayerLogic m_playerController;

    NavMeshAgent m_navMeshAgent;

    CloseEnemyState m_enemyState;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
