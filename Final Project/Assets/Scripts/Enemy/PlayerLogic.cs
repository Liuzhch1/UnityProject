using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    CharacterController m_characterController;

    const float SPEED = 5.0f;

    float m_horzontalInput;
    float m_verticalInput;

    Vector3 m_movementInput;
    Vector3 m_movement;

    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_horzontalInput = Input.GetAxisRaw("Horizontal");
        m_verticalInput = Input.GetAxisRaw("Vertical");

        m_movementInput = new Vector3(m_horzontalInput, 0, m_verticalInput);
    }

    void FixedUpdate()
    {
        m_movement = m_movementInput * Time.deltaTime * SPEED;

        m_characterController.Move(m_movement);
    }
}
