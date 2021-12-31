using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    AR,
    handgun
}

public class Gun
{
    public int MAX_AMMO;
    public float MAX_COOL_DOWN;
    public int ammo;
    public int mag;
}

public class WeaponLogic : MonoBehaviour
{
    #region Constants

    const float originFOV = 56.0f;

    #endregion

    #region Fields

    FPCameraLogic m_FPCameraLogic;
    Animator m_animator;
    AudioSource m_audioSource;
    FPSplayerLogic m_FPSplayerLogic;
    SaveManager m_saveManager;

    Weapon currentWeapon = Weapon.AR;

    float MAX_SHOT_COOLDOWN = 0.15f;
    int MAX_AMMO = 30;

    float m_shotCooldown = 0;
    int m_ammo;
    int m_mag;
    public int m_healthPack;

    bool m_isReloading = false;
    bool m_isAiming = false;

    float ARAimFOV = 30.0f;
    float ARScopeAimFOV = 25.0f;
    float HandgunAimFOV = 30.0f;

    public bool m_hasScope = false;
    bool m_isUsingScope = false;

    bool m_enableFire = false;
    bool m_isRunning = false;
    bool m_isKnifeAttacking = false;
    float m_knifeCoolDown = 0.0f;

    public bool m_hasKey = false;

    public Gun m_AR;
    public Gun m_Handgun;
    Gun currentGun;

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

    [SerializeField]
    AudioClip m_ARShootSound;

    [SerializeField]
    AudioClip m_HandgunShootSound;

    [SerializeField]
    AudioClip m_ARReloadSound;

    [SerializeField]
    AudioClip m_HandgunReloadSound;

    [SerializeField]
    AudioClip m_shootEmptySound;

    [SerializeField]
    AudioClip m_aimIn;

    [SerializeField]
    AudioClip m_aimOut;

    [SerializeField]
    AudioClip m_knifeAttackSound;

    #endregion

    #region Unity
    void Start()
    {
        m_FPCameraLogic = FindObjectOfType<FPCameraLogic>();
        m_animator = GetComponentInParent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_FPSplayerLogic = FindObjectOfType<FPSplayerLogic>();
        m_saveManager = FindObjectOfType<SaveManager>();

        m_AR = new Gun();
        m_Handgun = new Gun();

        m_AR.MAX_AMMO = 30;
        m_AR.MAX_COOL_DOWN = 0.15f;
        m_AR.ammo = 30;
        m_AR.mag = 5;

        m_Handgun.MAX_AMMO = 10;
        m_Handgun.MAX_COOL_DOWN = 0.35f;
        m_Handgun.ammo = 10;
        m_Handgun.mag = 3;

        currentGun = m_AR;

        m_ammo = currentGun.ammo;
        m_mag = currentGun.mag;
        MAX_SHOT_COOLDOWN = currentGun.MAX_COOL_DOWN;
        MAX_AMMO = currentGun.MAX_AMMO;

        m_healthPack = 3;

        
        m_ARscope.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.State != UIState.Game) {
            return;
        }

		// Shoot logics
		if (!m_FPSplayerLogic.m_IsAlive)
		{
            return;
		}
        
        if (Input.GetButton("Fire1") && m_enableFire)
        {
            if (m_shotCooldown <= 0.0f)
            {
                if (m_ammo > 0)
                {
                    if (!m_isUsingScope || !m_isAiming) {
                        m_animator.SetTrigger("Shoot");
                    }
                    

                    Shoot();

                    --m_ammo;
                    --currentGun.ammo;

                    UIManager.Instance.setAmmoNumber(currentWeapon, m_ammo, m_mag);

                    PlayShootSound(0.3f);
                }
                else
                {
                    // Debug.Log("empty gun");
                    // Play Empty Clip Sound
                    PlaySound(m_shootEmptySound);
                }

                m_shotCooldown = MAX_SHOT_COOLDOWN;
            }
        }

        if(m_knifeCoolDown >= 0.0f)
        {
            m_knifeCoolDown -= Time.deltaTime;
        }

        // Update Shot Cooldown
        if (m_shotCooldown > 0.0f)
        {
            m_shotCooldown -= Time.deltaTime;
        }

        // Reload
        if (Input.GetButtonDown("Reload") && !m_isReloading)
        {
            Reload();
            
        }

        // Aim
        if (Input.GetButtonDown("Fire2") && !m_isReloading)
        {
            m_isAiming = !m_isAiming;
            m_animator.SetBool("isAiming", m_isAiming);
        }

		if (Input.GetKeyDown(KeyCode.E))
		{
            pick();
		}

        if(Input.GetButtonDown("KnifeAttack") && !m_isReloading && m_knifeCoolDown <= 0.0f)
        {
            if (m_isAiming)
            {
                m_isAiming = !m_isAiming;
                m_animator.SetBool("isAiming", m_isAiming);
            }

            m_knifeCoolDown = 0.5f;

            m_animator.SetTrigger("KnifeAttack");
        }
    }

    private void FixedUpdate()
    {
        if (m_isKnifeAttacking)
        {
            KnifeAttack();
        }
    }
    #endregion

    #region Help Methods
    void Shoot()
    {
        UIManager.Instance.setShootingCrosshair();
        Ray ray = new Ray(m_FPCameraLogic.gameObject.transform.position, m_FPCameraLogic.gameObject.transform.forward);
        RaycastHit rayHit;

        // Log which object we hit
        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            string hitTag = rayHit.collider.gameObject.tag;
            Debug.Log("Bullet Hit Object: " + hitTag);
            if (hitTag == "Enemy01")
            {
                rayHit.collider.gameObject.GetComponent<FireRobLogic>().TakeDamage(10);
            }

            // Spawn Bullet Impact VFX
            GameObject.Instantiate(m_bulletImpactObj, rayHit.point, Quaternion.FromToRotation(Vector3.up, rayHit.normal) * Quaternion.Euler(-90, 0, 0));

        }

        // Play Muzzle VFX & Turn light on
        //m_muzzleFlash.Play(true);
        //m_muzzleFlashLight.enabled = true;

        // Add recoil to Camera
        if(m_isAiming)
        {
            if (currentWeapon == Weapon.AR && m_isUsingScope)
            {
                m_FPCameraLogic.AddRecoil(0.5f); 
            }
            else
            {
                m_FPCameraLogic.AddRecoil(1.0f); 
            }
        }
        else
        {
            m_FPCameraLogic.AddRecoil(2.0f); ;
        }
    }

    void Reload()
    {
        if (m_mag > 0 && m_ammo < MAX_AMMO)
        {
            QuitAim();

            m_isReloading = true;
            m_enableFire = false;

            m_animator.SetTrigger("Reload");

            PlayReloadSound();
        }
        else
        {
            // reload empty
        }
    }

    public void QuitAim() {
        if (m_isAiming)
        {
            m_isAiming = false;
            m_animator.SetBool("isAiming", m_isAiming);
        }
    }

    public void endReload()
    {
        m_ammo = MAX_AMMO;
        currentGun.ammo = MAX_AMMO;
        m_mag -= 1;
        currentGun.mag -= 1;
        UIManager.Instance.setAmmoNumber(currentWeapon, m_ammo, m_mag);
        m_isReloading = false;
        m_enableFire = true;
    }

    public void Aim()
    {
        if (m_isAiming)
        {
            PlaySound(m_aimIn);
            if (currentWeapon == Weapon.AR)
            {
                UIManager.Instance.hideCrosshair();
                m_FPCameraLogic.changeFOVto(m_isUsingScope ? ARScopeAimFOV : ARAimFOV);
                if (m_isUsingScope) {
                    //Debug.Log("Scope aim!");
                    UIManager.Instance.displayScopeCrosshair();
                    m_FPCameraLogic.changePositionTo(ARAimPoint.position);
                }
                
            }
            else if (currentWeapon == Weapon.handgun)
            {
                UIManager.Instance.hideCrosshair();
                m_FPCameraLogic.changeFOVto(HandgunAimFOV);
                m_FPCameraLogic.changePositionTo(HandgunAimPoint.position);
            }
        }
        else
        {
            UIManager.Instance.displayCrosshair();
            UIManager.Instance.hideScopeCrosshair();
            PlaySound(m_aimOut);
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
            if (hitTag == "mag" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f)
            {
                Destroy(rayHit.collider.gameObject);
                m_mag += 1;
                currentGun.mag += 1;
                UIManager.Instance.setAmmoNumber(currentWeapon, m_ammo, m_mag);
            }
            else if (hitTag == "scope" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f)
            {
                List<GameObject> childList = new List<GameObject>();
                int childCount = rayHit.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    GameObject child = rayHit.transform.GetChild(i).gameObject;
                    childList.Add(child);
                }
                for (int i = 0; i < childCount; i++)
                {
                    DestroyImmediate(childList[i]);
                }
                Destroy(rayHit.collider.gameObject);
                //m_ARscope.SetActive(true);
                m_hasScope = true;
                useScope();
            }
            else if (hitTag == "healthPack" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f)
            {
                Destroy(rayHit.collider.gameObject);
                m_healthPack += 1;
            }
            else if (hitTag == "radiationCloudController" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f)
            {
                List<GameObject> childList = new List<GameObject>();
                int childCount = rayHit.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    GameObject child = rayHit.transform.GetChild(i).gameObject;
                    childList.Add(child);
                }
                for (int i = 0; i < childCount; i++)
                {
                    DestroyImmediate(childList[i]);
                }
            }
            else if(hitTag == "CheckPoint" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f){
                m_saveManager.Save();
            }
            else if(hitTag == "key" && Vector3.Distance(transform.position, rayHit.transform.position) < 5.0f){
                Destroy(rayHit.collider.gameObject);
                m_hasKey = true;
            }
        }

    }

    public void useHealthPack()
	{
        if (m_healthPack > 0 && m_FPSplayerLogic.m_health < 100)
		{
            m_healthPack -= 1;
            m_FPSplayerLogic.RecoverHealth(30);
            UIManager.Instance.displayHeal();
		}
	}

    public void useScope()
    {
        if (!m_hasScope) return;
        if (currentWeapon == Weapon.AR)
        {
            m_animator.SetTrigger("Holster");
            m_enableFire = false;

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

    public void changeWeapon(Weapon type)
    {
        UIManager.Instance.displayWeapon(type);
        if (currentWeapon != type)
        {
            m_enableFire = false;

            // currentGun.ammo = m_ammo;
            // currentGun.mag = m_mag;

            QuitAim();

            m_animator.SetTrigger("Holster");

            currentWeapon = type;

            if(type == Weapon.AR)
            {
                currentGun = m_AR;
            }
            else if(type == Weapon.handgun)
            {
                currentGun = m_Handgun;
            }

            m_ammo = currentGun.ammo;
            m_mag = currentGun.mag;
            MAX_AMMO = currentGun.MAX_AMMO;
            MAX_SHOT_COOLDOWN = currentGun.MAX_COOL_DOWN;
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void endTakeOut()
    {
        m_enableFire = true;
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

    public void disanbleRunFire()
    {
        m_enableFire = false;
        m_isRunning = true;
    }

    public void enableRunFire()
    {
        if (m_isRunning)
        {
            m_enableFire = true;
            m_isRunning = false;
        }
    }

    public void enableKnife()
    {
        m_isKnifeAttacking = true;
        
        PlaySound(m_knifeAttackSound);
    }

    public void disableKnife()
    {
        m_isKnifeAttacking = false;
    }

    void KnifeAttack()
    {
        Ray ray = new Ray(m_FPCameraLogic.gameObject.transform.position, m_FPCameraLogic.gameObject.transform.forward);
        RaycastHit rayHit;

        // Log which object we hit
        if (Physics.Raycast(ray, out rayHit, 2.0f))
        {
            string hitTag = rayHit.collider.gameObject.tag;
            Debug.Log("Knife Hit Object: " + hitTag);
            if (hitTag == "Enemy01")
            {
                rayHit.collider.gameObject.GetComponent<FireRobLogic>().TakeDamage(5);
                disableKnife();
            }

        }

        
    }
    #endregion

    #region Sounds Methods
    void PlayShootSound(float volume = 1.0f)
    {
        m_audioSource.volume = volume;
        if (currentWeapon == Weapon.AR)
        {
            m_audioSource.PlayOneShot(m_ARShootSound);
        }
        else if(currentWeapon == Weapon.handgun)
        {
            m_audioSource.PlayOneShot(m_HandgunShootSound);
        }
    }

    void PlayReloadSound(float volume = 1.0f)
    {
        m_audioSource.volume = volume;
        if (currentWeapon == Weapon.AR)
        {
            m_audioSource.PlayOneShot(m_ARReloadSound);
        }
        else if (currentWeapon == Weapon.handgun)
        {
            m_audioSource.PlayOneShot(m_HandgunReloadSound);
        }
    }

    void PlaySound(AudioClip tmp, float volume = 1.0f)
    {
        m_audioSource.volume = volume;
        m_audioSource.PlayOneShot(tmp);
    }
    #endregion
}