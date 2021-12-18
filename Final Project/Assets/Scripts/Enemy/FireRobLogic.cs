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
    const float READY_RADIUS = 30.0f;
    const float RUN_RADIUS = READY_RADIUS - 2.0f;
    const float STAND_RADIUS = 2 * READY_RADIUS / 3;
    const float BACK_RADIUS = READY_RADIUS / 3;

    // 射击的夹角，大于这个夹角就不射击
    const float FIRE_ANGLE = 6.0f;

    const float WALKBACK_SPEED = 2.5f;
    //the speed Rob turn to player
    const float ROTATION_SPEED = 3.0f;

    #region View Parameter
    //检测Player的视野
    //最远能够看到的距离
    const float MAX_VIEWDISTANCE = 3*READY_RADIUS;
    //检测角度
    const float VIEW_ANGLE = 140.0f;
    //最大检测距离
    const float VIEW_RADIUS = READY_RADIUS;
    //检测密度
    const int VIEW_ANGLE_STEP = 100;
    //检测的视野高度
    const float VIEW_HEIGHT = 2.0f;
    // head up and down angle
    float m_viewRotationX = 0.0f;
    float m_viewRotationY = 0.0f;
    #endregion

    

    [SerializeField]
    Transform rayCastPoint;
    [SerializeField]
    Transform m_neckBone;
    [SerializeField]
    Transform m_rightShoulderBone;
    [SerializeField]
    Transform m_leftShoulderBone;
    [SerializeField]
    Transform m_hips;

    NavMeshAgent m_navMeshAgent;
    testEnemyPlayerLogic m_playerLogic;
    GameObject m_player;
    Animator m_animator;
    FireRobGunLogic m_gunLogic;

    FireRobState m_fireRobState;

    int m_health = 100;
    bool isAlert = false;
    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gunLogic = GetComponentInChildren<FireRobGunLogic>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<testEnemyPlayerLogic>();
        m_animator = GetComponent<Animator>();
        m_animator.SetFloat("State", 0.0f);
        m_fireRobState = FireRobState.Idel;
    }
    // Update is called once per frame
    void Update()
    {
        EfficientDetectPlayer();
        Debug.Log("Is alert: " + isAlert);
        Debug.Log("Now State: " + m_fireRobState);
        if (!m_player || isDead)
        {
            Debug.Log("This FireRob is dead!");
            return;
        }
        switch (m_fireRobState)
        {
            case (FireRobState.Idel):
                UpdateIdelState();
                break;
            case (FireRobState.Ready):
                UpdateReadyState();
                break;
            case (FireRobState.Run):
                UpdateRunState();
                break;
            case (FireRobState.Stand):
                UpdateStandState();
                break;
            case (FireRobState.Back):
                UpdateBackState();
                break;
        }

        // calculate y-axis increse of view(head up/down to player)

    }

    #region UpdateState
    void UpdateIdelState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) < READY_RADIUS && isAlert)
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
        else if (Vector3.Distance(transform.position, m_player.transform.position) < RUN_RADIUS)
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
        if (Vector3.Distance(transform.position, m_player.transform.position) < BACK_RADIUS)
        {
            m_fireRobState = FireRobState.Back;
        }
        else if (Vector3.Distance(transform.position, m_player.transform.position) > STAND_RADIUS)
        {
            m_navMeshAgent.SetDestination(m_player.transform.position);
            //Be running into Player state
            m_animator.SetFloat("State", 1.8f);
            TryFire();
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
        if (Vector3.Distance(transform.position, m_player.transform.position) < BACK_RADIUS && isAlert)
        {
            m_fireRobState = FireRobState.Back;
        }
        else if (Vector3.Distance(transform.position, m_player.transform.position) > STAND_RADIUS && isAlert)
        {
            m_fireRobState = FireRobState.Run;
        }
        else
        {
            // stand state, shoot
            m_animator.SetFloat("State", 3.5f);
            TryFire();
        }
    }
    void UpdateBackState()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) > BACK_RADIUS || !isAlert)
        {
            m_fireRobState = FireRobState.Stand;
        }
        else
        {
            // LookAt player so that FireRob can turn quickly
            Vector3 lookAtPlayer = m_player.transform.position;
            lookAtPlayer.y = transform.position.y;
            transform.LookAt(lookAtPlayer);

            //go back
            m_animator.SetFloat("State", 2.8f);
            Vector3 direction = m_player.transform.position - transform.position;
            direction = direction.normalized;
            float hor = -Mathf.Abs(direction.x) * Time.deltaTime * WALKBACK_SPEED;
            float ver = -Mathf.Abs(direction.z) * Time.deltaTime * WALKBACK_SPEED;
            m_navMeshAgent.enabled = false;
            transform.Translate(Vector3.forward * ver);
            transform.Translate(Vector3.right * hor);
            m_navMeshAgent.enabled = true;
            Debug.DrawLine(transform.position, transform.position + new Vector3(2 * direction.x, 0, 2 * direction.z), Color.black);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0, 2 * direction.z), Color.blue);
            Debug.DrawLine(transform.position, transform.position + new Vector3(2 * direction.x, 0, 0), Color.red);
            TryFire();
        }
    }
    #endregion

    void TryFire()
    {
        if (!isAlert)
        {
            return;
        }
        Vector3 playerDir = m_player.transform.position - transform.position;
        //slowly trun to player
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(playerDir), ROTATION_SPEED * Time.deltaTime);

        //m_gunLogic.Fire();
        if (Vector3.Angle(transform.forward, playerDir) <= FIRE_ANGLE || m_fireRobState == FireRobState.Back)
        {
            m_gunLogic.Fire();
        }
    }
    public void SpawnBullet()
    {
        m_gunLogic.SpawnBullet();
    }

    #region Head down and head up
    void LateUpdate()
    {
        
    }
    #endregion

    #region Detect
    void EfficientDetectPlayer()
    {
        if(!m_player || Vector3.Distance(transform.position, m_player.transform.position) > READY_RADIUS)
        {
            return;
        }
        isAlert = false;
        Vector3 dir = m_player.transform.position - transform.position;
        dir.y = 0;
        float toPlayerAngle = Vector3.Angle(transform.forward, dir);
        Vector3 normal = Vector3.Cross(transform.forward, dir);
        toPlayerAngle = Mathf.Abs(toPlayerAngle*Mathf.Sign(Vector3.Dot(normal, transform.up)));

        Ray ray = new Ray(rayCastPoint.position, m_player.transform.position - rayCastPoint.position);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, MAX_VIEWDISTANCE) && toPlayerAngle <= VIEW_ANGLE / 2)
        {
            Debug.Log("What is angle: " + toPlayerAngle);
            Debug.DrawLine(rayCastPoint.position, hit.transform.position,Color.red);
            if (hit.transform.gameObject.tag == "Player")
            {
                isAlert = true;
            }
        }
        Debug.DrawLine(transform.position, m_player.transform.position);
    }
    void DetectPlayer()
    {
        isAlert = false;
        Vector3 forward_left = Quaternion.Euler(0, -VIEW_ANGLE / 2, 0) * transform.forward * VIEW_RADIUS;
        for (int i = 0; i <= VIEW_ANGLE_STEP; i++)
        {
            Vector3 v = Quaternion.Euler(0, (VIEW_ANGLE / VIEW_ANGLE_STEP) * i, 0) * forward_left;

            Vector3 rayFrom = new Vector3(transform.position.x, VIEW_HEIGHT, transform.position.z);
            Ray ray = new Ray(rayFrom, v);
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(ray, out hit, VIEW_RADIUS);

            Vector3 pos = transform.position + v;
            if (hit.transform != null)
            {
                pos = hit.point;
            }
            Debug.DrawLine(rayFrom, pos, Color.green);

            if (hit.transform != null && hit.transform.gameObject.tag == "Player")
            {
                Debug.Log("ray hit player");
                isAlert = true;
            }
        }
    }
    #endregion
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        if (!isAlert)
        {
            isAlert = true;
            m_fireRobState = FireRobState.Run;
        }

        m_health -= damage;
        if (m_health <= 0)
        {
            isDead = true;
            m_animator.SetTrigger("Die");
        }
    }
    public void Destroy()
    {
        Destroy(gameObject, 10.0f);
    }
}