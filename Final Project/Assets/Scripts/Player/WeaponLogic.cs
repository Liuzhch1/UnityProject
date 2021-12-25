using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    #region Constants
    const float MAX_SHOT_COOLDOWN = 0.15f;
    const int MAX_AMMO = 30;

    #endregion

    #region Fields

    FPCameraLogic m_FPCameraLogic;
    Animator m_animator;

    float m_shotCooldown = 0;
    int m_ammo = MAX_AMMO;
    int m_mag = 4;

    bool m_isReloading = false;
    bool m_isAiming = false;

    float not_aimY;
    float aimY = 1.0f;

    #endregion

    #region Serialized Fields
    [SerializeField]
    GameObject m_bulletImpactObj;

    [SerializeField]
    GameObject m_arms;
    #endregion

    #region Unity
    void Start()
    {
        m_FPCameraLogic = FindObjectOfType<FPCameraLogic>();
        m_animator = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Shoot logics
        if (Input.GetButton("Fire1"))
        {
            if (m_shotCooldown <= 0.0f)
            {
                m_animator.SetTrigger("Shoot");

                if (m_ammo > 0)
                {
                    Shoot();

                    --m_ammo;

                    // SetAmmmoText(m_ammo);
                }
                else
                {
                    Debug.Log("empty gun");
                    // Play Empty Clip Sound
                    // PlaySound(m_emptyClipSound);
                }

                m_shotCooldown = MAX_SHOT_COOLDOWN;
            }
        }

        // Update Shot Cooldown
        if (m_shotCooldown > 0.0f)
        {
            m_shotCooldown -= Time.deltaTime;
        }

        // Reload
        if (Input.GetButtonDown("Reload") && !m_isReloading)
        {
            if (m_mag > 0)
            {
                m_isReloading = true;
                m_animator.SetTrigger("Reload");
                // Play Reload sound
                //PlaySound(m_reloadSound, 0.35f);
            }
            else
            {
                // reload empty
            }
        }

        // Aim
        if (Input.GetButtonDown("Fire2")&& !m_isReloading)
        {
            m_isAiming = !m_isAiming;
            m_animator.SetBool("isAiming", m_isAiming);
        }
    }

    #endregion

    #region Help Methods
    void Shoot()
    {
        Ray ray = new Ray(m_FPCameraLogic.gameObject.transform.position, m_FPCameraLogic.gameObject.transform.forward);
        RaycastHit rayHit;

        // Log which object we hit
        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            Debug.Log("Bullet Hit Object: " + rayHit.collider.gameObject.name);
            Debug.Log(m_bulletImpactObj);

            // Spawn Bullet Impact VFX
            Debug.Log("shoot");
            GameObject.Instantiate(m_bulletImpactObj, rayHit.point, Quaternion.FromToRotation(Vector3.up, rayHit.normal) * Quaternion.Euler(-90, 0, 0));
        }

        // Play Shoot Sound
        //PlaySound(m_shootSound);

        // Play Muzzle VFX & Turn light on
        //m_muzzleFlash.Play(true);
        //m_muzzleFlashLight.enabled = true;

        // Add recoil to Camera
        m_FPCameraLogic.AddRecoil();
    }

    public void endReload()
    {
        m_ammo = MAX_AMMO;
        m_mag -= 1;
        m_isReloading = false;
    }
    #endregion
}
