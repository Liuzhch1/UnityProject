using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    WeaponLogic m_weaponLogic;

    private void Start()
    {
        m_weaponLogic = GetComponentInChildren<WeaponLogic>();
    }

    #region Events Function
    public void endReload()
    {
        m_weaponLogic.endReload();
    }
    #endregion
}
