using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance => m_instance;
    static SaveManager m_instance;

    FPSplayerLogic m_playerLogic;
    CloseEnemyLogic[] m_closeEnemynemyLogic;
    FireRobLogic[] m_fireRobLogic;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerLogic = FindObjectOfType<FPSplayerLogic>();
        m_closeEnemynemyLogic = FindObjectsOfType<CloseEnemyLogic>();
        m_fireRobLogic = FindObjectsOfType<FireRobLogic>();
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        // Save
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }

        // Load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {
        m_playerLogic.Save();

        for (int index = 0; index < m_closeEnemynemyLogic.Length; ++index)
        {
            m_closeEnemynemyLogic[index].Save(index);
        }
        for (int index = 0; index < m_fireRobLogic.Length; ++index)
        {
            m_fireRobLogic[index].Save(index);
        }

        PlayerPrefs.Save();
    }

    public void Load()
    {
        m_playerLogic.Load();

        for (int index = 0; index < m_closeEnemynemyLogic.Length; ++index)
        {
            m_closeEnemynemyLogic[index].Load(index);
        }
        for (int index = 0; index < m_fireRobLogic.Length; ++index)
        {
            m_fireRobLogic[index].Load(index);
        }
    }
}
