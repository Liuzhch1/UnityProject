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

    #endregion

    #region Fields Serialized

    #endregion

    #region Unity
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float MouseX_Input = Input.GetAxis("Mouse X");
        float MouseY_Input = Input.GetAxis("Mouse Y");

        m_rotationY += MouseX_Input * MouseSensitivity;
        m_rotationX -= MouseY_Input * MouseSensitivity;
        m_rotationX = Mathf.Clamp(m_rotationX, MIN_X, MAX_X);

    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);
        //m_playerLogic.transform.rotation = Quaternion.Euler(0, m_rotationY, 0);
    }
    #endregion

    #region Help Methods
    public float GetRotationY()
    {
        return m_rotationY;
    }
    #endregion
}
