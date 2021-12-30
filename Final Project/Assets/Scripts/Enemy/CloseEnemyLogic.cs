using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum CloseEnemyState
{
    Idle, // 1
    Patrol, //2
    Chase, //3
    Attack //4
}

enum CloseEnemyType
{
    type1,
    type2,
    type3
}

public class CloseEnemyLogic : MonoBehaviour
{
    #region Parameter
    const float PATROL_SPEED = 4.0f;
    const float CHASE_SPEED = 12.0f;
    const float ATTACK_RADIUS = 3.5f;
    const float CHASE_RADIUS = 18.0f;
    const float MAX_IDLETIME = 4.0f;
    // the time keep chasing even out attack_radius 
    const float MAX_OUTRADIUS_CHASETIME = 5.0f;

    const float ROTATION_SPEED = 5.0f;

    float idleTime = MAX_IDLETIME;
    float chaseTime = MAX_OUTRADIUS_CHASETIME;

    const float MAX_VIEWDISTANCE = 2*CHASE_RADIUS;
    const float VIEW_ANGLE = 120.0f;

    const int DAMAGE = 10;
    #endregion

    GameObject m_player;
    FPSplayerLogic m_playerLogic;
    Animator m_animator;

    NavMeshAgent m_navMeshAgent;
    Collider m_collider;

    CloseEnemyState m_enemyState;

    [SerializeField]
    CloseEnemyType m_closeEnemyType;
    [SerializeField]
    Transform rayCastPoint;

    int m_type;
    [SerializeField]
    int m_health;
    bool isAlert;
    bool isDead = false;

    void Awake()
    {
        switch (m_closeEnemyType)
        {
            case (CloseEnemyType.type1):
                m_type = 1;
                break;
            case (CloseEnemyType.type2):
                m_type = 2;
                break;
            case (CloseEnemyType.type3):
                m_type = 3;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<FPSplayerLogic>();
        m_animator = GetComponent<Animator>();
        m_animator.SetInteger("State", 1);
        m_animator.SetInteger("Type", m_type);
        m_enemyState = CloseEnemyState.Idle;
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        Debug.Log("state: " + m_enemyState);
        switch (m_enemyState)
        {
            case (CloseEnemyState.Idle):
                UpdateIdleState();
                break;
            case (CloseEnemyState.Patrol):
                UpdatePatrolState();
                break;
            case (CloseEnemyState.Chase):
                UpdateChaseState();
                break;
            case (CloseEnemyState.Attack):
                UpdateAttackState();
                break;
        }
        EfficientDetectPlayer();
        Debug.Log("IsAlert: " + isAlert);
    }
    #region Update State
    void UpdateIdleState()
    {
        if (isAlert)
        {
            m_enemyState = CloseEnemyState.Chase;
            m_navMeshAgent.speed = CHASE_SPEED;
            idleTime = MAX_IDLETIME;
            return;
        }
        if (idleTime <= 0.0f)
        {
            m_enemyState = CloseEnemyState.Patrol;
            idleTime = MAX_IDLETIME;
            float posX = Random.Range(-20.0f, 20.0f);
            float poxZ = Random.Range(-20.0f, 20.0f);
            m_navMeshAgent.SetDestination(new Vector3(posX + transform.position.x, transform.position.y, poxZ + transform.position.z));
            m_animator.SetInteger("State", 2);
            idleTime = MAX_IDLETIME;
        }
        else
        {
            idleTime -= Time.deltaTime;
            m_animator.SetInteger("State", 1);
        }
    }
    void UpdatePatrolState()
    {
        if (isAlert)
        {
            m_enemyState = CloseEnemyState.Chase;
            m_navMeshAgent.speed = CHASE_SPEED;
            return;
        }
        if (m_navMeshAgent.isStopped || m_navMeshAgent.remainingDistance < 1.0f)
        {
            m_enemyState = CloseEnemyState.Idle;
            m_animator.SetInteger("State", 1);
            m_navMeshAgent.SetDestination(transform.position);
        }
    }
    void UpdateChaseState()
    {
        if (!m_player)
        {
            m_enemyState = CloseEnemyState.Idle;
            m_navMeshAgent.speed = PATROL_SPEED;
            return;
        }
        if(!isAlert)
        {
            if(chaseTime >= 0)
            {
                chaseTime -= Time.deltaTime;
                m_navMeshAgent.SetDestination(m_player.transform.position);
            }
            else
            {
                // to long lose player, turn to idle state
                m_enemyState = CloseEnemyState.Idle;
                m_animator.SetInteger("State", 1);
                m_navMeshAgent.SetDestination(transform.position);
                m_navMeshAgent.speed = PATROL_SPEED;
                chaseTime = MAX_OUTRADIUS_CHASETIME;
            }
        }
        else if(Vector3.Distance(transform.position,m_player.transform.position)<=ATTACK_RADIUS)
        {
            m_enemyState = CloseEnemyState.Attack;
            m_navMeshAgent.SetDestination(transform.position);
        }
        else
        {
            chaseTime = MAX_OUTRADIUS_CHASETIME;
            m_animator.SetInteger("State", 3);
            m_navMeshAgent.SetDestination(m_player.transform.position);
            LookAtPlayer();
        }
    }
    void UpdateAttackState()
    {
        if (!m_player)
        {
            m_enemyState = CloseEnemyState.Idle;
            return;
        }
        if (!isAlert)
        {
            m_enemyState = CloseEnemyState.Chase;
            return;
        }
        
        m_animator.SetInteger("State", 4);
        LookAtPlayer();
        if (Vector3.Distance(transform.position, m_player.transform.position) > ATTACK_RADIUS+1.0f)
        {
            m_enemyState = CloseEnemyState.Chase;
        }
    }
    #endregion
    #region Deteck player
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        //Gizmos.DrawSphere(transform.position, CHASE_RADIUS);
    }
    void EfficientDetectPlayer()
    {
        if (!m_player)
        {
            isAlert = false;
            return;
        }
        isAlert = false;
        if (Vector3.Distance(transform.position, m_player.transform.position) > CHASE_RADIUS)
        {
            return;
        }
        Vector3 dir = m_player.transform.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        float toPlayerAngle = Vector3.Angle(forward, dir);
        Vector3 normal = Vector3.Cross(forward, dir);
        toPlayerAngle = Mathf.Abs(toPlayerAngle * Mathf.Sign(Vector3.Dot(normal, transform.up)));

        Ray ray = new Ray(rayCastPoint.position, m_player.transform.position - rayCastPoint.position);
        RaycastHit hit;
        if ((Physics.Raycast(ray, out hit, MAX_VIEWDISTANCE) || Vector3.Distance(transform.position,m_player.transform.position)<ATTACK_RADIUS) && toPlayerAngle <= VIEW_ANGLE / 2)
        {
            Debug.DrawLine(rayCastPoint.position, hit.transform.position, Color.red);
            if (hit.transform.gameObject.tag == "Player")
            {
                isAlert = true;
            }
        }
        Debug.DrawLine(transform.position, m_player.transform.position);
    }
    #endregion
    public void TakeDamage(int damage)
    {
        if (m_health > 0)
        {
            isAlert = true;
            m_enemyState = CloseEnemyState.Chase;
            m_health -= damage;
        }
        else
        {
            m_navMeshAgent.enabled = false;
            m_collider.enabled = false;
            m_animator.SetTrigger("Dead");
        }
    }
    public void Attack()
    {
        m_playerLogic.TakeDamage(DAMAGE);
    }
    public void LookAtPlayer()
    {
        Debug.Log("looked");
        Vector3 look = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
        transform.LookAt(look);
    }


    public void Save(int index)
    {
        PlayerPrefs.SetFloat("EnemyPosX" + index, transform.position.x);
        PlayerPrefs.SetFloat("EnemyPosY" + index, transform.position.y);
        PlayerPrefs.SetFloat("EnemyPosZ" + index, transform.position.z);

        PlayerPrefs.SetFloat("EnemyRotX" + index, transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("EnemyRotY" + index, transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("EnemyRotZ" + index, transform.rotation.eulerAngles.z);


        PlayerPrefs.SetInt("EnemyHealth" + index, m_health);
    }

    public void Load(int index)
    {
        float playerPosX = PlayerPrefs.GetFloat("EnemyPosX" + index);
        float playerPosY = PlayerPrefs.GetFloat("EnemyPosY" + index);
        float playerPosZ = PlayerPrefs.GetFloat("EnemyPosZ" + index);

        float playerRotX = PlayerPrefs.GetFloat("EnemyRotX" + index);
        float playerRotY = PlayerPrefs.GetFloat("EnemyRotY" + index);
        float playerRotZ = PlayerPrefs.GetFloat("EnemyRotZ" + index);

        m_health = PlayerPrefs.GetInt("EnemyHealth" + index);

        m_navMeshAgent.enabled = false;
        transform.position = new Vector3(playerPosX, playerPosY, playerPosZ);
        transform.rotation = Quaternion.Euler(playerRotX, playerRotY, playerRotZ);
        m_enemyState = CloseEnemyState.Idle;
        m_animator.SetInteger("State", 1);
        isDead = false;
        m_navMeshAgent.enabled = true;
    }
}
