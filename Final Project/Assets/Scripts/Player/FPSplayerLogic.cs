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
    Vector3 m_movementInput;

    Vector3 m_verticalMovement;
    Vector3 m_horizontalMovement;
    Vector3 m_heightMovement;

    float m_forwardMovementSpeed = 7.0f;
    float m_backwardMovementSpeed = 2.5f;
    float m_strafeMovementSpeed = 2.0f;
    float m_crouchingSpeed = 2.0f;

    float m_rotationY;

    bool m_crouch = false;
    bool m_isCrouching = false;
    bool m_jump = false;

    float m_gravity = 0.8f;

    float m_jumpHeight = 0.2f;
    float m_crouchHeight = 1.0f;
    float m_standHeight = 2.4f;

    Camera m_camera;
    FPCameraLogic m_cameraLogic;

    int currentWeapon = 0;
    #endregion

    #region Fields Serialized

    //[SerializeField]
    //List<AudioClip> m_runStoneSounds = new List<AudioClip>();

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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Movement input
        m_horizontalMovementInput = Input.GetAxis("Horizontal");
        m_verticalMovementInput = Input.GetAxis("Vertical");
        m_movementInput = new Vector3(m_horizontalMovementInput, 0, m_verticalMovementInput);

        //Apply camera rotation
        m_rotationY += Input.GetAxis("Mouse X") * m_cameraLogic.MouseSensitivity;
        transform.rotation = Quaternion.Euler(0, m_rotationY, 0);

        // input
        if ( m_characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && !m_crouch)
            {
                m_jump = true;
            }

            if (Input.GetButtonDown("Crouch"))
            {
                float targetHeight = m_isCrouching ? m_standHeight : m_crouchHeight;
                StartCoroutine(doCrouch(targetHeight));
                m_isCrouching = !m_isCrouching;
            }

            if (Input.GetButtonDown("Weapon"))
            {
                // change current weapon: 0 = hand, 1 = gun, 2 = handgun
                currentWeapon += 1;
                currentWeapon %= 3;
            }
        }
        
    }

    private void FixedUpdate()
    {

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

    #region Help Methods
    float GetMovementSpeed()
    {
        if (m_isCrouching)
        {
            return m_crouchingSpeed;
        }
        else if (m_verticalMovementInput >= 0.1f)
        {
            return m_forwardMovementSpeed;
        }
        else
        {
            return m_backwardMovementSpeed;
        }
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

    
}
