using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEnemyPlayerLogic : MonoBehaviour
{
    CharacterController m_characterController;

    const float SPEED = 10.0f;

    float m_horzontalInput;
    float m_verticalInput;

    Vector3 m_movementInput;
    Vector3 m_movement;

    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    Transform m_spawnPoint;

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

        if ((Input.GetAxis("Fire1") != 0))
        {
            GameObject bullet = Instantiate(m_bulletPrefab, m_spawnPoint.position, transform.rotation * m_bulletPrefab.transform.rotation);
            bullet.transform.forward = m_spawnPoint.transform.forward;
        }
    }

    void FixedUpdate()
    {
        m_movement = m_movementInput * Time.deltaTime * SPEED;

        m_characterController.Move(m_movement);
    }

    public void TakeDamage(int d)
    {
        return;
    }
}
