using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRobGunLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    Transform m_bulletSpawnPoint;
    [SerializeField]
    GameObject m_emptyBulletPrefab;
    [SerializeField]
    Transform m_emptySpawnPoint;

    GameObject m_player;
    Animator m_animator;

    const float MAX_FIRE_COOLDOWN = 1.0f;
    float m_fire_cooldown = 0.0f;
    const int MAX_BULLET_NUM = 20;
    int m_bullet_num = MAX_BULLET_NUM;
    const float MAX_RELOAD_TIME = 2.66f;
    float m_reloadTime = MAX_RELOAD_TIME;

    const float EMPTYBULLET_SPAWNOUT_SPEED = 1.0f;

    bool m_isReloading = false;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_animator = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fire_cooldown > 0)
        {
            m_fire_cooldown -= Time.deltaTime;
        }
        if (m_bullet_num <= 0)
        {
            m_isReloading = true;
            m_animator.SetBool("IsReloading", m_isReloading);
        }
        if (m_isReloading)
        {
            m_reloadTime -= Time.deltaTime;
            if (m_reloadTime < 0.0f)
            {
                m_isReloading = false;
                m_reloadTime = MAX_RELOAD_TIME;
                m_animator.SetBool("IsReloading", m_isReloading);
                m_bullet_num = MAX_BULLET_NUM;
            }
        }
    }

    public void Fire()
    {
        if (m_fire_cooldown <= 0.0f && m_bullet_num > 0 && !m_isReloading)
        {
            m_animator.SetTrigger("Shoot");
            m_fire_cooldown = MAX_FIRE_COOLDOWN;
        }
    }

    public void SpawnBullet()
    {
        GameObject bullet = Instantiate(m_bulletPrefab, m_bulletSpawnPoint.position, Quaternion.LookRotation(m_player.transform.position - transform.position));
        m_bullet_num -= 1;
        Debug.Log(m_bulletSpawnPoint.transform.forward);
        Debug.Log((m_player.transform.position - m_bulletSpawnPoint.position).normalized);
        bullet.transform.forward = -m_bulletSpawnPoint.transform.forward;
        GameObject emptyBullet = Instantiate(m_emptyBulletPrefab, m_emptySpawnPoint.position, Quaternion.LookRotation(m_player.transform.position - transform.position));
        emptyBullet.transform.forward = -m_emptySpawnPoint.transform.forward;
        emptyBullet.GetComponent<Rigidbody>().velocity = (m_emptySpawnPoint.transform.up + m_emptySpawnPoint.transform.right) * EMPTYBULLET_SPAWNOUT_SPEED;
        Destroy(emptyBullet, 5.0f);
    }
}
