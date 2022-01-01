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
    FastRun3,
    FastAttack4,
    Attack5,
    Invisible,
    JumpAttack,
    SkillAttack
}

public class BossLogic : MonoBehaviour
{
    #region Parameter
    const float WALK_SPEED = 2.5f;
    const float FASTRUN_SPEED = 10.0f;
    const float ATTACK_RADIUS = 3.2f;
    const float MAX_WALKTIME = 4.0f;

    float walkTime = MAX_WALKTIME;

    const int ATTACK_DAMAGE = 7;
    const int FAST_ATTACK_DAMAGE = 13;
    const int JUMP_ATTACK_DAMAGE = 20;

    const float SKILL_COOLDOWN = 15.0f;
    // first time cool down
    float skillCooldown = 1.5f*SKILL_COOLDOWN;

    const float JUMPATTACK_SPEED = 2.0f;
    const float JUMPATTACK_DISTANCE = 3.7f * JUMPATTACK_SPEED;
    const float JUMPATTACK_BIAS = 0.7f;

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
    int m_health=500;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (!isAlert)
        {
            if (Vector3.Distance(m_player.transform.position,RoomCenter.transform.position)<20.0f)
            {
                isAlert = true;
            }
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
            m_bossState = BossState.SkillAttack;
            float skillType = Random.Range(0.0f, 1.0f);
            if (skillType > 0.5f)
            {
                //Roar
                m_animator.SetTrigger("Roar");
                m_navMeshAgent.SetDestination(transform.position);
                isAttacking = true;
            }
            else
            {
                //Invisible
                TurnInvisible(false);
                m_animator.SetTrigger("Invisible");
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
            case (BossState.SkillAttack):
                break;
            case (BossState.Idle1):
                UpdateIdle();
                break;
            case (BossState.Walk2):
                UpdateWalk();
                break;
            case (BossState.FastRun3):
                UpdateFastRun();
                break;
            case (BossState.Attack5):
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
        if (walkTime < 0.0f)
        {
            m_bossState = BossState.FastRun3;
            walkTime = MAX_WALKTIME;
            m_navMeshAgent.speed = FASTRUN_SPEED;
            return;
        }
        else if (ToPlayerDistance() <= ATTACK_RADIUS)
        {
            m_bossState = BossState.Attack5;
            m_navMeshAgent.SetDestination(transform.position);
            return;
        }
        walkTime -= Time.deltaTime;
        LookAtPlayer();
        m_animator.SetInteger("State", 2);
        m_navMeshAgent.speed = WALK_SPEED;
        m_navMeshAgent.SetDestination(m_player.transform.position);
    }
    void UpdateFastRun()
    {
        if (ToPlayerDistance() > JUMPATTACK_DISTANCE + JUMPATTACK_BIAS && ToPlayerDistance() < JUMPATTACK_DISTANCE +JUMPATTACK_BIAS+0.1f)
        {
            isAttacking = true;
            m_bossState = BossState.JumpAttack;
            m_animator.SetTrigger("JumpAttack");
            m_navMeshAgent.speed = JUMPATTACK_SPEED;
            m_navMeshAgent.destination = m_player.transform.position;
            return;
        }
        if (ToPlayerDistance() < ATTACK_RADIUS)
        {
            m_bossState = BossState.Attack5;
            m_navMeshAgent.SetDestination(transform.position);
            return;
        }
        m_animator.SetInteger("State", 3);
        m_navMeshAgent.SetDestination(m_player.transform.position);
        LookAtPlayer();
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
        m_bossState = BossState.Walk2;
        m_animator.SetInteger("State", 2);
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
        m_playerLogic.TakeDamage(JUMP_ATTACK_DAMAGE);
    }
    public void TakeDamage(int damage)
    {
        if (m_health > 0)
        {
            m_health -= damage;
        }
        else
        {
            m_navMeshAgent.enabled = false;
            transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            m_animator.SetTrigger("Dead");
            Destroy(gameObject, 10.0f);
        }
    }
}
