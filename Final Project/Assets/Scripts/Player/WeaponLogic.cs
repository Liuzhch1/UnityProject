using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    AR,
    handgun
}
public class WeaponLogic : MonoBehaviour
{
    #region Constants
    const float MAX_SHOT_COOLDOWN = 0.15f;
    const int MAX_AMMO = 30;
    const int MAX_MAG = 5;

    const float originFOV = 56.0f;

    #endregion

    #region Fields

    FPCameraLogic m_FPCameraLogic;
    Animator m_animator;

    Weapon currentWeapon = Weapon.AR;

    float m_shotCooldown = 0;
    int m_ammo = MAX_AMMO;
    int m_mag = 4;

    bool m_isReloading = false;
    bool m_isAiming = false;

    //float not_aimY;
    //float aimY = 1.0f;
    float ARAimFOV = 23.0f;
    float HandgunAimFOV = 23.0f;

    bool m_hasScope = false;
    bool m_isUsingScope = false;

    #endregion

    #region Serialized Fields
    [SerializeField]
    GameObject Wp_AR;

    [SerializeField]
    GameObject Wp_handgun;

    [SerializeField]
    RuntimeAnimatorController ARController;

    [SerializeField]
    RuntimeAnimatorController handGunController;

    [SerializeField]
    GameObject m_bulletImpactObj;

    [SerializeField]
    GameObject m_ironSight;

    [SerializeField]
    GameObject m_ARscope;

    [SerializeField]
    Transform ARAimPoint;

    [SerializeField]
    Transform OriginAimPoint;

    [SerializeField]
    Transform HandgunAimPoint;
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            useScope();
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

    public void Aim()
    {
        if (m_isAiming)
        {
            if(currentWeapon == Weapon.AR)
            {
                m_FPCameraLogic.changeFOVto(ARAimFOV);
                m_FPCameraLogic.changePositionTo(ARAimPoint.position);
            }
            else if(currentWeapon == Weapon.handgun)
            {
                m_FPCameraLogic.changeFOVto(HandgunAimFOV);
                m_FPCameraLogic.changePositionTo(HandgunAimPoint.position);
            }
        }
        else
        {
            m_FPCameraLogic.changeFOVto(originFOV);
            m_FPCameraLogic.changePositionTo(OriginAimPoint.position);
        }
    }

    public void pick()
    {
        Ray ray = new Ray(m_FPCameraLogic.gameObject.transform.position, m_FPCameraLogic.gameObject.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            string hitTag = rayHit.collider.gameObject.tag;
            Debug.Log("Try to pick: " + hitTag);
            if (hitTag == "mag") // ЕЏМа
            {
                m_mag += MAX_MAG;
            }
            else if (hitTag == "scope")
            {
                m_hasScope = true;
            }
        }

    }

    public void useScope()
    {
        //if (!m_hasScope) return;
        if(currentWeapon == Weapon.AR)
        {
            m_animator.SetTrigger("Holster");
            if (m_isUsingScope)
            {
                m_ARscope.SetActive(false);
                m_ironSight.SetActive(true);
            }
            else
            {
                m_ARscope.SetActive(true);
                m_ironSight.SetActive(false);
            }
            m_isUsingScope = !m_isUsingScope;
        }
        else
        {
            Debug.Log("You cannot use this scope for this weapon.");
        }
    }

    public void setController() //change animator controller
    {
        if (currentWeapon == Weapon.AR)
        {
            Wp_AR.SetActive(true);
            Wp_handgun.SetActive(false);
            m_animator.runtimeAnimatorController = ARController;
        }
        else if (currentWeapon == Weapon.handgun)
        {
            Wp_AR.SetActive(false);
            Wp_handgun.SetActive(true);
            m_animator.runtimeAnimatorController = handGunController;

        }
    }
    public void changeWeapon(int type)
    {
        //type 0 => AR, type 1 => handgun
        if (type == 0)
        {
            currentWeapon = Weapon.AR;

            m_animator.SetTrigger("Holster");

        }
        else if (type == 1)
        {
            currentWeapon = Weapon.handgun;

            m_animator.SetTrigger("Holster");
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    #endregion
}
