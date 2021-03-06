using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairLogic : MonoBehaviour
{
    CharacterController m_playerController;
    RectTransform m_reticle;
    float m_size;
    [SerializeField]
    float m_idleSize = 100f;

    [SerializeField]
    float m_shootSize = 150f;

    [SerializeField]
    float m_movingSize = 150f;

    [SerializeField]
    float m_movingThreshold = 3f;

    float m_shootingTimer = 0;
    float m_feedbackTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_playerController = FindObjectOfType<CharacterController>();
        m_reticle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        m_shootingTimer -= (m_shootingTimer > 0) ? Time.deltaTime : 0;
        if (m_feedbackTimer > 0) {
            m_feedbackTimer -= Time.deltaTime;
            transform.GetChild(1).gameObject.SetActive(!((m_feedbackTimer <= 0.14f && m_feedbackTimer > 0.07f) || (m_feedbackTimer < 0f)));
        }
        //Debug.Log("Moving@" + m_playerController.velocity.magnitude);
        bool isShooting = m_shootingTimer > 0;
        bool isMoving = m_playerController.velocity.magnitude > m_movingThreshold;
        if (isShooting) {
            m_size = Mathf.Lerp(m_size, m_shootSize, Time.deltaTime * 5f);
        } else if (isMoving) {
            m_size = Mathf.Lerp(m_size, m_movingSize, Time.deltaTime * 5f);
        } else {
            m_size = Mathf.Lerp(m_size, m_idleSize, Time.deltaTime * 5f);
        }
        m_reticle.sizeDelta = new Vector2(m_size, m_size);
    }

    public void SetShooting() {
        m_shootingTimer = 0.2f;
    }

    public void SetFeedback() {
        m_feedbackTimer = 0.21f;
        transform.GetChild(1).gameObject.SetActive(true);
    }

}
