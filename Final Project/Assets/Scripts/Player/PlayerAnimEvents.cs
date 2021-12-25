using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    WeaponLogic m_weaponLogic;
    FPSplayerLogic m_playerLogic;

    private void Start()
    {
        m_weaponLogic = GetComponentInChildren<WeaponLogic>();
        m_playerLogic = GetComponentInParent<FPSplayerLogic>();
    }

    #region Events Function
    public void endReload()
    {
        m_weaponLogic.endReload();
    }

    public void endHolster()
    {
        m_playerLogic.setController();
    }
    #endregion
}
