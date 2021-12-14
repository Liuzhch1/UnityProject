using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorLogic : MonoBehaviour
{
    #region Fields
    Animator m_animator;

    float m_horizontalMovementInput;
    float m_verticalMovementInput;
    #endregion

    #region Unity
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement input
        m_horizontalMovementInput = Input.GetAxis("Horizontal");
        m_verticalMovementInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        // Set Animation States
        Debug.Log(m_animator);
        Debug.Log(m_verticalMovementInput);
        m_animator.SetFloat("VerticalInput", m_verticalMovementInput);
    }
    #endregion
}
