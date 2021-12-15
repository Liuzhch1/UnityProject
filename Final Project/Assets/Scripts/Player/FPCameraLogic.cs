using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraLogic : MonoBehaviour
{
    // Start is called before the first frame update
    #region Constants

    const float MIN_X = -50.0f;
    const float MAX_X = 50.0f;

    #endregion

    #region Fields

    float m_rotationX;
    float m_rotationY;
    public float MouseSensitivity;

    bool m_recoilAnim = false;
    float m_recoilProgress = 0.0f;
    float m_startRotationX = 0.0f;
    float m_targetRotationX = 0.0f;

    #endregion

    #region Fields Serialized

    #endregion

    #region Unity
    void Start()
    {
        // Disable mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float MouseX_Input = Input.GetAxis("Mouse X");
        float MouseY_Input = Input.GetAxis("Mouse Y");

        m_rotationY += MouseX_Input * MouseSensitivity;
        m_rotationX -= MouseY_Input * MouseSensitivity;
        m_rotationX = Mathf.Clamp(m_rotationX, MIN_X, MAX_X);

        // If player moves the mouse stop auto-retarget
        if (MouseX_Input != 0.0f || MouseY_Input != 0.0f)
        {
            m_recoilAnim = false;
        }

        // Animate aim position back to original position
        if (m_recoilAnim)
        {
            m_recoilProgress += Time.deltaTime;
            m_rotationX = Mathf.Lerp(m_startRotationX, m_targetRotationX, m_recoilProgress);

            // Last part of Lerp can snap to target destination
            if (Mathf.Abs(m_rotationX - m_targetRotationX) < 0.1f)
            {
                m_rotationX = m_targetRotationX;
                m_recoilAnim = false;
                m_recoilProgress = 0.0f;
            }
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);
    }
    #endregion

    #region Help Methods
    public float GetRotationY()
    {
        return m_rotationY;
    }

    public void AddRecoil()
    {
        if (!m_recoilAnim)
        {
            m_targetRotationX = m_rotationX;
        }

        m_recoilProgress = 0.0f;
        m_rotationX -= 4.0f;
        m_rotationY += Random.Range(-1.0f, 1.0f);
        m_startRotationX = m_rotationX;

        m_recoilAnim = true;
    }
    #endregion
}
