using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    #region Constants
    const float MIN_X = -50.0f;
    const float MAX_X = 50.0f;
    #endregion

    #region Fields
    CharacterController m_characterController;

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
    #endregion

    #region Fields Serialized

    [SerializeField]
    GameObject m_cameraObject = null;
    
    #endregion

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        m_camera = m_cameraObject.GetComponent<Camera>();
        m_cameraLogic = m_cameraObject.GetComponent<FPCameraLogic>();
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

        // Jump input
        if(Input.GetButtonDown("Jump") && m_characterController.isGrounded && !m_crouch)
        {
            m_jump = true;
        }

        if(Input.GetButtonDown("Crouch") && m_characterController.isGrounded)
        {
            float targetHeight = m_isCrouching ? m_standHeight : m_crouchHeight;
            StartCoroutine(doCrouch(targetHeight));
            m_isCrouching = !m_isCrouching;
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
        while(Mathf.Abs(m_characterController.height - target) > 0.1f)
        {
            yield return null;
            m_characterController.height = 
                Mathf.SmoothDamp(m_characterController.height,target,ref currentHeight, Time.deltaTime * 2.0f);
        }
    }
    #endregion
}
