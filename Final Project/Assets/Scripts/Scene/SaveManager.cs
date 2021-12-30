using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance => m_instance;
    static SaveManager m_instance;

    FPSplayerLogic m_playerLogic;
    FireRobLogic[] m_fireRobLogic;
    CloseEnemyLogic[] m_closeEnemyLogic;

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
        m_fireRobLogic = FindObjectsOfType<FireRobLogic>();
        m_closeEnemyLogic = FindObjectsOfType<CloseEnemyLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        // Save
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
            Debug.Log("Save");
        }

        // Load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
            Debug.Log("Load");
        }
    }

    public void Save()
    {
        m_playerLogic.Save();

        for (int index = 0; index < m_fireRobLogic.Length; ++index)
        {
            m_fireRobLogic[index].Save(index);
        }
        for (int index = 0; index < m_closeEnemyLogic.Length; ++index)
        {
            m_closeEnemyLogic[index].Save(index);
        }

        PlayerPrefs.Save();
    }

    public void Load()
    {
        m_playerLogic.Load();

        for (int index = 0; index < m_fireRobLogic.Length; ++index)
        {
            m_fireRobLogic[index].Load(index);
        }
        for (int index = 0; index < m_closeEnemyLogic.Length; ++index)
        {
            m_closeEnemyLogic[index].Load(index);
        }
    }
}
