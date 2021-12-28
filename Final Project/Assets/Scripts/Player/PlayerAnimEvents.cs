using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    WeaponLogic m_weaponLogic;
    //FPSplayerLogic m_playerLogic;
    //FPCameraLogic m_cameraLogic;

    private void Start()
    {
        m_weaponLogic = GetComponentInChildren<WeaponLogic>();
        //m_playerLogic = GetComponentInParent<FPSplayerLogic>();
        //m_cameraLogic = GetComponent<FPCameraLogic>();
    }

    #region Events Function
    public void endReload()
    {
        m_weaponLogic.endReload();
    }

    public void endHolster()
    {
        m_weaponLogic.setController();
    }

    public void endAim()
    {
        m_weaponLogic.Aim();
    }

    public void endTakeOut()
    {
        m_weaponLogic.endTakeOut();
    }
    #endregion
}
