using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FireRobState
{
    Idle,
    Ready,
    Run,
    Stand,
    Back,
    Dead
}

public class FireRobLogic : MonoBehaviour
{
    const float MINI_FIRE_RADIUS = 1.5f;
    const float READY_RADIUS = 10.0f;
    const float RUN_RADIUS = READY_RADIUS - 2.0f;
    const float STAND_RADIUS = 2 * READY_RADIUS / 3;
    const float BACK_RADIUS = READY_RADIUS / 3;
    const float SETALERT_RADUIS = 1.1f * READY_RADIUS;

    // ?????н??????????н??????
    const float FIRE_ANGLE = 6.0f;

    const float WALKBACK_SPEED = 2.5f;
    //the speed Rob turn to player
    const float ROTATION_SPEED = 3.0f;

    #region View Parameter
    //???Player?????
    //???????????????
    const float MAX_VIEWDISTANCE = 3*READY_RADIUS;
    //?????
    const float VIEW_ANGLE = 140.0f;
    #endregion

    [SerializeField]
    Transform rayCastPoint;

    NavMeshAgent m_navMeshAgent;
    GameObject m_player;
    Animator m_animator;
    FireRobGunLogic m_gunLogic;
    Collider m_collider;

    FireRobState m_fireRobState;

    [SerializeField]
    int m_health = 100;
    int m_maxHealth;
    bool isAlert = false;
    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gunLogic = GetComponentInChildren<FireRobGunLogic>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_collider = GetComponent<Collider>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_animator = GetComponent<Animator>();
        m_animator.SetFloat("State", 0.0f);
        m_fireRobState = FireRobState.Idle;
        m_maxHealth = m_health;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAlert)
        {
            EfficientDetectPlayer();
        }
        if (!m_player || isDead)
        {
            //Debug.Log("This FireRob is dead!");
            return;
        }
        switch (m_fireRobState)
        {
            case (FireRobState.Idle):
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
    }

    #region UpdateState
    void UpdateIdelState()
    {
        if (isAlert)
        {
            m_fireRobState = FireRobState.Run;
            return;
        }
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
        if (isAlert)
        {
            m_fireRobState = FireRobState.Run;
            return;
        }
        if (Vector3.Distance(transform.position, m_player.transform.position) > READY_RADIUS)
        {
            m_fireRobState = FireRobState.Idle;
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
        Vector3 playerDirPlane = new Vector3(playerDir.x, 0, playerDir.z);
        //slowly trun to player
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(playerDir), ROTATION_SPEED * Time.deltaTime);

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Vector3.Angle(transform.forward, playerDirPlane) <= FIRE_ANGLE || m_fireRobState == FireRobState.Back)
        {
            m_gunLogic.Fire();
        }
    }
    public void SpawnBullet()
    {
        m_gunLogic.SpawnBullet();
    }


    #region Detect
    void EfficientDetectPlayer()
    {
        if (isAlert)
        {
            return;
        }
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
        if((Physics.Raycast(ray,out hit, MAX_VIEWDISTANCE) || Vector3.Distance(transform.position, m_player.transform.position) < MINI_FIRE_RADIUS) && toPlayerAngle <= VIEW_ANGLE / 2)
        {
            //Debug.DrawLine(rayCastPoint.position, hit.transform.position,Color.red);
            if (hit.transform.gameObject.tag == "Player")
            {
                isAlert = true;
            }
        }
        //Debug.DrawLine(transform.position, m_player.transform.position);
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
        m_health = Mathf.Clamp(m_health, 0, m_maxHealth);
        UIManager.Instance.displayFeedbackCrosshair();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy01");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= SETALERT_RADUIS)
            {
                enemy.GetComponent<FireRobLogic>().SetAlert();
            }
        }
        if (m_health <= 0)
        {
            isDead = true;
            m_animator.SetTrigger("Die");
            m_collider.enabled = false;
            m_navMeshAgent.enabled = false;
        }
    }
    public void SetAlert()
    {
        Debug.Log("succ setalert");
        isAlert = true;
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
        m_fireRobState = FireRobState.Idle;
        m_animator.SetFloat("State", 0.0f);
        isDead = false;
        m_collider.enabled = true;
        m_navMeshAgent.enabled = true;
    }
}
