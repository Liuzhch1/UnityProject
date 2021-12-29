using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierLogic : MonoBehaviour
{
    #region Parameter
    const float RUN_SPEED = 4.5f;
    const float WALKBACK_SPEED = 2.5f;

    [SerializeField]
    int m_health = 50;

    bool m_isDead = false;
    #endregion

    #region SerializeField
    [SerializeField]
    Transform m_neckBone;

    [SerializeField]
    Transform m_rightShoulderBone;

    [SerializeField]
    Transform m_leftShoulderBone;

    [SerializeField]
    Transform m_hips;

    [SerializeField]
    Transform m_weapon;

    [SerializeField]
    Transform m_leftFoot;

    [SerializeField]
    Transform m_rightFoot;

    
    #endregion

    Animator m_animator;
    NavMeshAgent m_navMeshAgent;
    GameObject m_player;
    FPSplayerLogic m_playerLogic;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<FPSplayerLogic>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
