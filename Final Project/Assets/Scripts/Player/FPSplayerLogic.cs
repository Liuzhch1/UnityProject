using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FPSplayerLogic : MonoBehaviour
{
    #region Constants
    const float MIN_X = -50.0f;
    const float MAX_X = 50.0f;
    #endregion

    #region Fields
    CharacterController m_characterController;
    Animator m_animator;
    AudioSource m_audioSource;

    float m_horizontalMovementInput;
    float m_verticalMovementInput;

    Vector3 m_verticalMovement;
    Vector3 m_horizontalMovement;
    Vector3 m_heightMovement;

    public float m_forwardMovementSpeed = 10.0f;
    public float m_backwardMovementSpeed = 8.0f;
    public float m_strafeMovementSpeed = 8.0f;
    public float m_crouchingSpeed = 2.0f;

    float m_rotationY;

    bool m_crouch = false;
    bool m_isCrouching = false;
    bool m_jump = false;

    float m_gravity = 0.8f;

    float m_jumpHeight = 0.2f;
    float m_crouchHeight = 1.0f;
    float m_standHeight = 2.4f;

    public int m_health = 100;
    int m_healthPack = 0;
    public bool m_isAlive = true;

    Camera m_camera;
    FPCameraLogic m_cameraLogic;
    WeaponLogic m_weaponLogic;

    #endregion

    #region Fields Serialized

    [SerializeField]
    AudioClip m_moveSound;

    #endregion

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponentInChildren<Animator>();
        m_audioSource = GetComponent<AudioSource>();

        m_camera = GetComponentInChildren<Camera>();
        m_cameraLogic = GetComponentInChildren<FPCameraLogic>();
        m_weaponLogic = FindObjectOfType<WeaponLogic>();

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
        if (Input.GetKeyDown(KeyCode.X)) { // for test
            Debug.Log("Test Death!");
            m_isAlive = false;
            m_health = 0;
            DisplayDeath();
        } else if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Test Respawn!");
            m_isAlive = true;
            m_health = 100;
            DisplayRespawn();
        }
        
        if (UIManager.Instance.State != UIState.Game) {
            m_horizontalMovementInput = Mathf.Lerp(m_horizontalMovementInput, 0, Time.deltaTime * 5f);
            m_verticalMovementInput = Mathf.Lerp(m_verticalMovementInput, 0, Time.deltaTime * 5f);
            return;
        }

		if (!m_isAlive)
		{
            return;
		}

        // Movement input
        m_horizontalMovementInput = Input.GetAxis("Horizontal");
        m_verticalMovementInput = Input.GetAxis("Vertical");

        //Apply camera rotation
        m_rotationY += Input.GetAxis("Mouse X") * m_cameraLogic.MouseSensitivity;
        transform.rotation = Quaternion.Euler(0, m_rotationY, 0);

        // Input
        if ( m_characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && !m_crouch)
            {
                m_jump = true;
                m_audioSource.PlayOneShot(m_moveSound);
            }

            // Crouch or Stand up
            if (Input.GetButtonDown("Crouch")) 
            {
                float targetHeight = m_isCrouching ? m_standHeight : m_crouchHeight;
                StartCoroutine(doCrouch(targetHeight));
                m_isCrouching = !m_isCrouching;
                m_audioSource.PlayOneShot(m_moveSound);
            }

            // Change Weapon
            if (Input.GetButtonDown("Weapon"))
            {
                if(m_weaponLogic.GetCurrentWeapon() == Weapon.AR)
                {
                    m_weaponLogic.changeWeapon(Weapon.handgun);
                }
                else
                {
                    m_weaponLogic.changeWeapon(Weapon.AR);
                }
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (!m_isAlive) return;

        if( m_verticalMovementInput == 1)
        {
            m_weaponLogic.disanbleRunFire();
        }
        else
        {
            m_weaponLogic.enableRunFire();
        }

        // Apply jump
        if (m_jump)
        {
            m_heightMovement.y = m_jumpHeight;
            m_jump = false;
        }

        // Apply Gravity
        m_heightMovement.y -= m_gravity * Time.deltaTime;

        // Calculate movement
        m_verticalMovement = transform.forward * m_verticalMovementInput * GetMovementSpeed() * Time.deltaTime;
        m_horizontalMovement = m_camera.transform.right * m_horizontalMovementInput * m_strafeMovementSpeed * Time.deltaTime;

        // Apply Movement
        m_characterController.Move(m_horizontalMovement + m_verticalMovement + m_heightMovement);

        // Stop Height movement & Jump animation if grounded
        if (m_characterController.isGrounded)
        {
            m_heightMovement.y = 0.0f;
        }

        // Set animator parameters
        m_animator.SetFloat("VerticalInput", m_verticalMovementInput);
        m_animator.SetFloat("HorizontalInput", m_horizontalMovementInput);
    }


    #endregion

    #region Movement Methods
    float GetMovementSpeed()
    {
        float v = 0.0f;
        if (m_isCrouching)
        {
            v = m_crouchingSpeed;
        }
        else if (m_verticalMovementInput >= 0.1f)
        {
            v = m_forwardMovementSpeed;
        }
        else
        {
            v = m_backwardMovementSpeed;
        }

        if (m_weaponLogic.isAiming())
        {
            v /= 2.0f;
        }

        return v;
    }

    IEnumerator doCrouch(float target)
    {
        float currentHeight = 0.0f;
        while (Mathf.Abs(m_characterController.height - target) > 0.1f)
        {
            yield return null;
            m_characterController.height =
                Mathf.SmoothDamp(m_characterController.height, target, ref currentHeight, Time.deltaTime * 2.0f);
        }
    }


    public void TakeDamage(int damage)
    {
        UIManager.Instance.displayHurt();
        m_health -= damage;
        m_health = Mathf.Clamp(m_health, 0, 100);
        UIManager.Instance.setHealth(m_health);
        // Debug.Log(m_health);
        if (m_health <= 0 && m_isAlive)
        {
            m_isAlive = false;
            gameObject.GetComponent<CapsuleCollider>().enabled=false;
            DisplayDeath();
        }
    }

    public void DisplayDeath() {
        UIManager.Instance.displayDeath();
        // Disable character controller
        m_characterController.enabled = false;
        // Disable arms
        Transform playerArms = transform.GetChild(0).GetChild(0).GetChild(0);
        playerArms.gameObject.SetActive(false);
        // enable gun rigidbody
        Transform deathCameraHolder = transform.GetChild(0).GetChild(0).GetChild(2);
        deathCameraHolder.gameObject.SetActive(true);
        deathCameraHolder.GetComponent<Rigidbody>().AddForce((Vector3.up - transform.forward) * 80f);
        Transform camera = transform.GetChild(0).GetChild(0).GetChild(3);
        camera.parent = deathCameraHolder;
    }

    public void DisplayRespawn() {
        UIManager.Instance.displayRespawn();
        // Enable character controller
        m_characterController.enabled = true;
        Transform playerArms = transform.GetChild(0).GetChild(0).GetChild(0);
        playerArms.gameObject.SetActive(true);
        playerArms.GetComponent<Animator>().SetTrigger("Holster");
        // disable gun rigidbody
        Transform deathCameraHolder = transform.GetChild(0).GetChild(0).GetChild(2);
        deathCameraHolder.gameObject.SetActive(false);
        deathCameraHolder.localPosition = Vector3.zero;
        deathCameraHolder.localRotation = Quaternion.Euler(0, 0, -28.62f);
        Transform camera = deathCameraHolder.GetChild(0);
        camera.parent = transform.GetChild(0).GetChild(0);
        camera.localPosition = new Vector3(0, 0.03900003f, -0.099f);
        camera.localRotation = Quaternion.Euler(0, 0, 0);
    }


    public void RecoverHealth(int heal)
	{
        m_health += heal;
        m_health = Mathf.Clamp(m_health, 0, 100);
        UIManager.Instance.setHealth(m_health);
        Debug.Log(m_health);
    }
    #endregion

    #region Sounds Methods
    //public void PlayFootStepSound(int index)
    //{
    //    if (index == 0)
    //    {
    //        RayCastTerrain(m_leftFoot.position);
    //    }
    //    else if(index == 1)
    //    {
    //        RayCastTerrain(m_rightFoot.position);
    //    }
    //}

    //void RayCastTerrain(Vector3 position)
    //{
    //    Ray ray = new Ray(position, Vector3.down);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        string hitTag = hit.collider.gameObject.tag;
    //        if (hitTag == "stone")
    //        {
    //            PlayRandomSound(m_runStoneSound);
    //        }
    //    }
    //}
    void PlayRandomSound(List<AudioClip> sounds)
    {
        PlaySound(sounds[Random.Range(0, sounds.Count - 1)]);
    }

    void PlaySound(AudioClip sound, float volume = 100)
    {
        if (m_audioSource && sound)
        {
            m_audioSource.volume = volume;
            m_audioSource.PlayOneShot(sound);
        }
    }
    #endregion

    #region ForEnemy
    public bool BackFree()
    {
        return true;
    }
    public bool LeftFree()
    {
        return true;
    }
    public bool RightFree()
    {
        return true;
    }
    #endregion

    #region Load and Save
    public void Save()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);

        PlayerPrefs.SetFloat("PlayerRotY", m_rotationY);

    }

    public void Load()
    {
        float playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
        float playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
        float playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ");



        m_health = 100;
        UIManager.Instance.setHealth(m_health);

        m_isAlive = true;
        m_characterController.enabled = false;

        transform.position = new Vector3(playerPosX, playerPosY, playerPosZ);
        m_rotationY = PlayerPrefs.GetFloat("PlayerRotY");

        m_characterController.enabled = true;
        gameObject.GetComponent<CapsuleCollider>().enabled=true;
    }
    #endregion

}
