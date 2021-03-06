using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum CloseEnemyState
{
    Idle, // 1
    Patrol, //2
    Chase, //3
    Attack, //4
    Walk //5
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
    const float PATROL_SPEED = 3.0f;
    public const float CHASE_SPEED = 5.0f;
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

    const float SETALERT_RADUIS = CHASE_RADIUS * 0.4f;

    const float MAX_WALKTIME=5.0f;
    float walkTime = MAX_WALKTIME;
    const float MAX_PATROL_TIME = 5.0f;
    float patrolTime = MAX_PATROL_TIME;
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
    int m_maxHealth;
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
        m_maxHealth = m_health;

        bool requireLoad = PlayerPrefs.GetInt("Load") == 1;
        if (requireLoad) {
            SaveManager.Instance.Load();
        } else {
            SaveManager.Instance.Save();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        EfficientDetectPlayer();
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
            case (CloseEnemyState.Walk):
                UpdateWalkState();
                break;
        }
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
        if (m_navMeshAgent.isStopped || Vector3.Distance(transform.position,m_navMeshAgent.destination) < 1.0f || patrolTime < 0.0f)
        {
            m_enemyState = CloseEnemyState.Idle;
            m_animator.SetInteger("State", 1);
            m_navMeshAgent.SetDestination(transform.position);
            patrolTime = MAX_PATROL_TIME;
        }
        patrolTime -= Time.deltaTime;
        m_animator.SetInteger("State", 2);
    }
    void UpdateChaseState()
    {
        if(!isAlert)
        {
            if(chaseTime >= 0 && m_playerLogic.m_isAlive)
            {
                chaseTime -= Time.deltaTime;
                m_navMeshAgent.SetDestination(m_player.transform.position);
            }
            else
            {
                // to long lose player, turn to idle state
                m_enemyState = CloseEnemyState.Idle;
                m_animator.SetInteger("State", 1);
                idleTime = 0.0f;
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
        if (!isAlert)
        {
            m_enemyState = CloseEnemyState.Idle;
            m_animator.SetInteger("State", 3);
            return;
        }
        m_animator.SetInteger("State", 4);
        LookAtPlayer();
        if (Vector3.Distance(transform.position, m_player.transform.position) > ATTACK_RADIUS+1.0f)
        {
            m_enemyState = CloseEnemyState.Walk;
            m_navMeshAgent.speed = PATROL_SPEED;
        }
    }
    void UpdateWalkState()
    {
        if (!isAlert)
        {
            m_enemyState = CloseEnemyState.Idle;
            return;
        }
        if (Vector3.Distance(transform.position, m_player.transform.position) < ATTACK_RADIUS)
        {
            walkTime = MAX_WALKTIME;
            m_enemyState = CloseEnemyState.Attack;
            m_animator.SetInteger("State", 4);
            return;
        }
        if (walkTime < 0.0f)
        {
            walkTime = MAX_WALKTIME;
            m_navMeshAgent.speed = CHASE_SPEED;
            m_enemyState = CloseEnemyState.Chase;
            m_animator.SetInteger("State",3);
            return;
        }
        else
        {
            LookAtPlayer();
            m_navMeshAgent.SetDestination(m_player.transform.position);
            m_animator.SetInteger("State", 5);
            walkTime -= Time.deltaTime;
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
        if (!m_player.GetComponent<FPSplayerLogic>().m_isAlive)
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
            m_health = Mathf.Clamp(m_health, 0, m_maxHealth);
            UIManager.Instance.displayFeedbackCrosshair();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy02");
            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) <= SETALERT_RADUIS)
                {
                    enemy.GetComponent<CloseEnemyLogic>().SetAlert();
                }
            }
        }
        else
        {
            m_navMeshAgent.enabled = false;
            m_collider.enabled = false;
            m_animator.SetTrigger("Dead");
            isDead = true;
        }
    }
    public void SetAlert()
    {
        isAlert = true;
    }
    public void Attack()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) < ATTACK_RADIUS + 0.6f)
        {
            m_playerLogic.TakeDamage(DAMAGE);
        }
    }
    public void LookAtPlayer()
    {
        Vector3 look = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
        transform.LookAt(look);
    }


    public void Save(int index)
    {
        PlayerPrefs.SetFloat("CloseEnemyPosX" + index, transform.position.x);
        PlayerPrefs.SetFloat("CloseEnemyPosY" + index, transform.position.y);
        PlayerPrefs.SetFloat("CloseEnemyPosZ" + index, transform.position.z);

        PlayerPrefs.SetFloat("CloseEnemyRotX" + index, transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("CloseEnemyRotY" + index, transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("CloseEnemyRotZ" + index, transform.rotation.eulerAngles.z);


        PlayerPrefs.SetInt("CloseEnemyHealth" + index, m_health);
    }

    public void Load(int index)
    {
        float closeEnemyPosX = PlayerPrefs.GetFloat("CloseEnemyPosX" + index);
        float closeEnemyPosY = PlayerPrefs.GetFloat("CloseEnemyPosY" + index);
        float closeEnemyPosZ = PlayerPrefs.GetFloat("CloseEnemyPosZ" + index);

        float closeEnemyRotX = PlayerPrefs.GetFloat("CloseEnemyRotX" + index);
        float closeEnemyRotY = PlayerPrefs.GetFloat("CloseEnemyRotY" + index);
        float closeEnemyRotZ = PlayerPrefs.GetFloat("CloseEnemyRotZ" + index);

        m_health = PlayerPrefs.GetInt("CloseEnemyHealth" + index);

        m_navMeshAgent.enabled = false;
        transform.position = new Vector3(closeEnemyPosX, closeEnemyPosY, closeEnemyPosZ);
        transform.rotation = Quaternion.Euler(closeEnemyRotX, closeEnemyRotY, closeEnemyRotZ);
        m_navMeshAgent.enabled = true;
        m_enemyState = CloseEnemyState.Idle;
        m_animator.SetInteger("State", 1);
        m_animator.SetTrigger("Load");
        m_navMeshAgent.SetDestination(transform.position);
        patrolTime = MAX_PATROL_TIME;
        idleTime = MAX_IDLETIME;
        isAlert=false;
        isDead = false;
        m_collider.enabled = true;
    }
}