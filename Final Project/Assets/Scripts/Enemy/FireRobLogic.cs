using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FireRobState
{
    Idel,
    Ready,
    Run,
    Stand,
    Back,
    Dead
}

public class FireRobLogic : MonoBehaviour
{
    const float READY_RADIUS = 20.0f;
    const float RUN_RADIUS = READY_RADIUS-2.0f;
    const float STAND_RADIUS = 2 * READY_RADIUS / 3;
    const float BACK_RADIUS = READY_RADIUS / 2;

    const float WALKBACK_SPEED = 4.0f;
    const float ROTATION_SPEED = 2.0f;

    NavMeshAgent m_navMeshAgent;
    PlayerLogic m_playerLogic;
    GameObject m_player;
    Animator m_animator;

    FireRobState m_fireRobState;

    int health = 100;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<PlayerLogic>();
        m_animator = GetComponent<Animator>();
        m_animator.SetFloat("State", 0.0f);
        m_fireRobState = FireRobState.Idel;
    }
    // Update is called once per frame
    void Update()
    {
        if (!m_player)
        {
            return;
        }
        switch (m_fireRobState)
        {
            case (FireRobState.Idel):
                Debug.Log("Idel");
                UpdateIdelState();
                break;
            case (FireRobState.Ready):
                Debug.Log("Ready");
                UpdateReadyState();
                break;
            case (FireRobState.Run):
                Debug.Log("Run");
                UpdateRunState();
                break;
            case (FireRobState.Stand):
                Debug.Log("Stand");
                UpdateStandState();
                break;
            case (FireRobState.Back):
                Debug.Log("Back");
                UpdateBackState();
                break;
        }
    }

    void UpdateIdelState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) < READY_RADIUS)
        {
            m_fireRobState = FireRobState.Ready;
        }
        else
        {
            m_animator.SetFloat("State", 0.0f);
        }
    }
    void UpdateReadyState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) > READY_RADIUS)
        {
            m_fireRobState = FireRobState.Idel;
        }
        else if(Vector3.Distance(transform.position, m_player.transform.position) < RUN_RADIUS)
        {
            m_fireRobState = FireRobState.Run;
        }
        else
        {
            //Be ready state
            m_animator.SetFloat("State", 0.8f);
        }
    }
    void UpdateRunState()
    {
        if(Vector3.Distance(transform.position, m_player.transform.position) < BACK_RADIUS)
        {
            m_fireRobState = FireRobState.Back;
        }
        else if(Vector3.Distance(transform.position, m_player.transform.position) > STAND_RADIUS)
        {
            Debug.Log("SetDes");
            m_navMeshAgent.SetDestination(m_player.transform.position);
            Debug.Log(m_navMeshAgent.isOnNavMesh);
            Debug.Log("isStop: "+m_navMeshAgent.isStopped);
            //Be running into Player state
            m_animator.SetFloat("State", 1.8f);
            Fire();
        }
        else
        {
            //turn to stand state
            m_fireRobState = FireRobState.Stand;
            //stop run to player
            m_navMeshAgent.destination = transform.position;
        }
    }
    void UpdateStandState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) < BACK_RADIUS)
        {
            m_fireRobState = FireRobState.Back;
        }
        else if(Vector3.Distance(transform.position, m_player.transform.position) > STAND_RADIUS)
        {
            m_fireRobState = FireRobState.Run;
        }
        else
        {
            // stand state, shoot
            m_animator.SetFloat("State", 4.0f);
            Fire();
        }
    }
    void UpdateBackState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) > BACK_RADIUS)
        {
            m_fireRobState = FireRobState.Stand;
        }
        else
        {
            //go back
            m_animator.SetFloat("State", 2.8f);
            Vector3 direction = m_player.transform.position - transform.position;
            direction = direction.normalized;
            float hor = -Mathf.Abs(direction.x)*Time.deltaTime*WALKBACK_SPEED;
            float ver = -Mathf.Abs(direction.z)*Time.deltaTime*WALKBACK_SPEED;
            m_navMeshAgent.enabled = false;
            transform.Translate(Vector3.forward * ver);
            transform.Translate(Vector3.right * hor);
            m_navMeshAgent.enabled = true;
            Debug.DrawLine(transform.position, transform.position + new Vector3(2*direction.x, 0, 2*direction.z), Color.black);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0, 2*direction.z), Color.blue);
            Debug.DrawLine(transform.position, transform.position + new Vector3(2*direction.x, 0, 0), Color.red);
            Fire();
        }
    }
    void Fire()
    {
        //transform.LookAt(m_player.transform.position);
        //return;
        //2021.12.14 Stop here, I need to change slowing LookAt
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_player.transform.position - transform.position), ROTATION_SPEED * Time.deltaTime);

        transform.position += transform.forward * WALKBACK_SPEED * Time.deltaTime;
    }
}
