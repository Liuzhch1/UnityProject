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
    JumpAttack6,
    SkillAttack
}

public class BossLogic : MonoBehaviour
{
    #region Parameter
    const float WALK_SPEED = 4.0f;
    const float FASTRUN_SPEED = 18.0f;
    const float ATTACK_RADIUS = 3.5f;
    const float MAX_WALKTIME = 4.0f;

    float walkTime = MAX_WALKTIME;

    const int ATTACK_DAMAGE = 7;
    const int FAST_ATTACK_DAMAGE = 13;
    const int JUMP_ATTACK_DAMAGE = 20;

    const float SKILL_COOLDOWN = 7.0f;
    // first time cool down
    float skillCooldown = 1.5f*SKILL_COOLDOWN;

    const float JUMPATTACK_SPEED = 2.0f;
    const float JUMPATTACK_DISTANCE = 3.7f * JUMPATTACK_SPEED;

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
        if (skillCooldown > 0.0f)
        {
            skillCooldown -= Time.deltaTime;
        }
        else
        {
            m_bossState = BossState.SkillAttack;
            float skillType = Random.Range(0.0f, 1.0f);
            if (skillType > 0.5f)
            {
                //Roar
                RoarAttack();
            }
            else
            {
                //Invisible
                InvisibleAttack();
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
            case (BossState.FastAttack4):
                UpdateFastAttack();
                break;
            case (BossState.Attack5):
                UpdateAttack();
                break;
            case (BossState.JumpAttack6):
                UpdateJumpAttack();
                break;
        }
    }
    #region Update State
    void UpdateIdle()
    {
        if (isAlert)
        {
            m_bossState = BossState.Walk2;
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
            return;
        }
        else if (Vector3.Distance(transform.position, m_player.transform.position) <= ATTACK_RADIUS)
        {
            m_bossState = BossState.Attack5;
            m_navMeshAgent.SetDestination(transform.position);
            return;
        }
        LookAtPlayer();
        m_animator.SetInteger("State", 2);
        m_navMeshAgent.speed = WALK_SPEED;
        m_navMeshAgent.SetDestination(m_player.transform.position);
    }
    void UpdateFastRun()
    {
        float distance = Vector3.Distance(transform.position, m_player.transform.position);
        if (distance > JUMPATTACK_DISTANCE - 0.1f && distance < JUMPATTACK_DISTANCE + 0.1f)
        {
            m_bossState = BossState.JumpAttack6;
            m_animator.SetTrigger("JumpAttack");
            m_navMeshAgent.speed = JUMPATTACK_SPEED;
            return;
        }
        if (distance < ATTACK_RADIUS)
        {
            m_bossState = BossState.Attack5;
            m_navMeshAgent.SetDestination(transform.position);
            return;
        }
        LookAtPlayer();
        m_animator.SetInteger("State", 3);
    }
    void UpdateFastAttack()
    {

    }
    void UpdateAttack()
    {

    }
    void UpdateJumpAttack()
    {

    }
    void UpdateInvisiable()
    {

    }
    void UpdateRoar()
    {

    }
    #endregion
    void RoarAttack()
    {

    }
    void InvisibleAttack()
    {

    }
    public void LookAtPlayer()
    {
        Vector3 look = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
        transform.LookAt(look);
    }
    public void FinishSkillAttack()
    {
        m_bossState = BossState.Walk2;
    }
}
