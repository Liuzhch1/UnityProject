using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum BossState
{
    // the number means animator State
    // Invisiable and Roar are skill attack
    Idle1,
    Walk2,
    FastAttack3,
    Attack4,
    Invisible,//5
    JumpAttack,//6
    RoarAttack//7
}

public class BossLogic : MonoBehaviour
{
    #region Parameter
    const float WALK_SPEED = 1.6f;
    const float FASTRUN_SPEED = 7.0f;
    const float ATTACK_RADIUS = 3.2f;

    const int ATTACK_DAMAGE = 7;
    const int FAST_ATTACK_DAMAGE = 13;
    const int JUMP_ATTACK_DAMAGE = 20;

    const float SKILL_COOLDOWN = 15.0f;
    // first time cool down
    float skillCooldown = 1.5f*SKILL_COOLDOWN;

    const float JUMPATTACK_SPEED = 3.0f;
    const float JUMPATTACK_DISTANCE = 3.7f * JUMPATTACK_SPEED;
    const float JUMPATTACK_BIAS = 0.1f;
    const float JUMP_ATTACKRADUIS = 5.0f;

    const float MAX_INVISIBLE_TIME = 5.0f;
    float invisibleTime = MAX_INVISIBLE_TIME;

    GameObject m_player;
    FPSplayerLogic m_playerLogic;
    Animator m_animator;
    NavMeshAgent m_navMeshAgent;

    BossState m_bossState;
    [SerializeField]
    Transform rayCastPoint;

    [SerializeField]
    int m_health=1000;
    int m_maxHealth;
    bool isDead = false;
    bool isAlert = false;
    bool invisible = false;
    bool isAttacking = false;

    [SerializeField]
    GameObject Type1Prefab;
    [SerializeField]
    GameObject Type2Prefab;
    [SerializeField]
    GameObject Type3Prefab;
    [SerializeField]
    Transform spawnPoint1;
    [SerializeField]
    Transform spawnPoint2;
    [SerializeField]
    Transform spawnPoint3;
    [SerializeField]
    GameObject mainBody;
    [SerializeField]
    GameObject tail;
    [SerializeField]
    Transform RoomCenter;
    //there will be no this type when Roar
    int noType = 1;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<FPSplayerLogic>();
        m_animator = GetComponent<Animator>();
        m_animator.SetInteger("State", 1);
        m_bossState = BossState.Idle1;
        m_maxHealth = m_health;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Time: " + invisibleTime);
        if (isDead)
        {
            return;
        }
        if (!isAlert)
        {
            if (Vector3.Distance(m_player.transform.position, RoomCenter.transform.position) < 20.0f)
            {
                isAlert = true;
            }
            return;
        }
        if (skillCooldown > 0.0f)
        {
            if (!isAttacking)
            {
                skillCooldown -= Time.deltaTime;
            }
        }
        else
        {
            float skillType = Random.Range(0.0f, 1.0f);
            if (skillType > 0.4f)
            {
                //Roar
                m_bossState = BossState.RoarAttack;
                m_animator.SetTrigger("Roar");
                m_animator.SetInteger("State", 7);
                m_navMeshAgent.SetDestination(transform.position);
                isAttacking = true;
            }
            else
            {
                //Invisible
                TurnInvisible(false);
                m_animator.SetTrigger("Invisible");
                m_animator.SetInteger("State", 5);
                m_bossState = BossState.Invisible;
                invisible = true;
                m_navMeshAgent.SetDestination(transform.position);
                m_navMeshAgent.speed = WALK_SPEED;
            }
            skillCooldown = SKILL_COOLDOWN;
            return;
        }
        switch (m_bossState)
        {
            case (BossState.RoarAttack):
                break;
            case (BossState.Idle1):
                UpdateIdle();
                break;
            case (BossState.Walk2):
                UpdateWalk();
                break;
            case (BossState.Attack4):
                UpdateAttack();
                break;
            case (BossState.JumpAttack):
                break;
            case (BossState.Invisible):
                UpdateInvisiable();
                break;
        }
        Debug.Log("State: " + m_bossState);
    }
    #region Update State
    void UpdateIdle()
    {
        if (isAlert)
        {
            m_bossState = BossState.Walk2;
            m_navMeshAgent.speed = WALK_SPEED;
            return;
        }
        m_animator.SetInteger("State", 1);
    }
    void UpdateWalk()
    {
        if (ToPlayerDistance() <= ATTACK_RADIUS)
        {
            m_bossState = BossState.Attack4;
            m_navMeshAgent.SetDestination(transform.position);
            return;
        }
        if (ToPlayerDistance() > JUMPATTACK_DISTANCE + JUMPATTACK_BIAS && ToPlayerDistance() < JUMPATTACK_DISTANCE + JUMPATTACK_BIAS + 0.05f)
        {
            isAttacking = true;
            m_bossState = BossState.JumpAttack;
            m_animator.SetTrigger("JumpAttack");
            m_animator.SetInteger("State", 6);
            m_navMeshAgent.speed = JUMPATTACK_SPEED;
            m_navMeshAgent.destination = m_player.transform.position;
            return;
        }
        LookAtPlayer();
        m_animator.SetInteger("State", 2);
        m_navMeshAgent.speed = WALK_SPEED;
        m_navMeshAgent.SetDestination(m_player.transform.position);
    }
    void UpdateAttack()
    {
        isAttacking = true;
        if (!m_player)
        {
            m_bossState = BossState.Idle1;
            m_animator.SetTrigger("PlayerDead");
            isAlert = false;
            isAttacking = false;
            return;
        }
        if (ToPlayerDistance() > ATTACK_RADIUS + 1.0f)
        {
            m_bossState = BossState.Walk2;
            m_navMeshAgent.speed = WALK_SPEED;
            isAttacking = false;
            return;
        }
        m_animator.SetInteger("State", 5);
        LookAtPlayer();
    }
    void UpdateInvisiable()
    {
        if (!invisible)
        {
            return;
        }
        if (invisibleTime > 0.0f)
        {
            invisibleTime -= Time.deltaTime;
            return;
        }
        Vector3 playerPos = m_player.transform.position;
        if (m_playerLogic.BackFree())
        {
            Vector3 back = -3 * m_player.transform.forward;
            transform.position = new Vector3(playerPos.x + back.x, transform.position.y, playerPos.z + back.z);
        }
        else if (m_playerLogic.LeftFree())
        {

        }
        else if (m_playerLogic.RightFree())
        {

        }
        else
        {

        }
        m_animator.SetTrigger("FastAttack");
        LookAtPlayer();
        m_navMeshAgent.SetDestination(transform.position);
        TurnInvisible(true);
        invisibleTime = MAX_INVISIBLE_TIME;
        invisible = false;
    }
    #endregion
    public void RoarSpawnEnemy()
    {
        if (noType != 1)
        {
            GameObject enemy1 = Instantiate(Type1Prefab, spawnPoint1.position, Quaternion.LookRotation(m_player.transform.position - transform.position));
            enemy1.transform.forward = transform.forward;
        }
        if (noType != 2)
        {
            GameObject enemy2 = Instantiate(Type2Prefab, spawnPoint2.position, Quaternion.LookRotation(m_player.transform.position - transform.position));
            enemy2.transform.forward = transform.forward;
        }
        if (noType != 3)
        {
            GameObject enemy3 = Instantiate(Type3Prefab, spawnPoint3.position, Quaternion.LookRotation(m_player.transform.position - transform.position));
            enemy3.transform.forward = transform.forward;
        }
        switch (noType)
        {
            case (1):
                noType = 2;
                break;
            case (2):
                noType = 3;
                break;
            case (3):
                noType = 1;
                break;
        }
    }
    void TurnInvisible(bool turn)
    {
        mainBody.GetComponent<SkinnedMeshRenderer>().enabled = turn;
        tail.GetComponent<SkinnedMeshRenderer>().enabled = turn;
    }
    public void LookAtPlayer()
    {
        Vector3 look = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
        transform.LookAt(look);
    }
    public void FinishSkillAttack()
    {
        m_bossState = BossState.Walk2;
        m_animator.SetInteger("State", 2);
        isAttacking = false;
    }
    public void FinishJumpAttack()
    {
        m_bossState = BossState.Walk2;
        m_animator.SetInteger("State", 2);
        isAttacking = false;
    }
    public void FinishRoarAttack()
    {
        m_bossState = BossState.Walk2;
        m_animator.SetInteger("State", 2);
        isAttacking = false;
    }
    public void FinishFastAttack()
    {
        Debug.Log("FUCK!!!");
        m_bossState = BossState.Walk2;
        m_animator.SetInteger("State", 2);
        invisibleTime = MAX_INVISIBLE_TIME;
        isAttacking = false;
    }
    float ToPlayerDistance()
    {
        return Vector3.Distance(transform.position, m_player.transform.position);
    }
    public void Attack()
    {
        m_playerLogic.TakeDamage(ATTACK_DAMAGE);
    }
    public void JumpAttack()
    {
        if (ToPlayerDistance() < JUMP_ATTACKRADUIS)
        {
            m_playerLogic.TakeDamage(JUMP_ATTACK_DAMAGE);
        }
    }
    public void TakeDamage(int damage)
    {
        if (m_health > 0)
        {
            m_health -= damage;
            m_health = Mathf.Clamp(m_health, 0, m_maxHealth);
            UIManager.Instance.displayFeedbackCrosshair();
        }
        else
        {
            m_navMeshAgent.enabled = false;
            transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            m_animator.SetTrigger("Dead");
            UIManager.Instance.displayDialogue(Speaker.Agent, "CompleteDialogue1", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Commander, "CompleteDialogue2", 3.0f);
            Destroy(gameObject, 10.0f);
        }
    }
}
