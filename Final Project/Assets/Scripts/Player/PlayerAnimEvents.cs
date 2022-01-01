using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    WeaponLogic m_weaponLogic;
    //FPSplayerLogic m_playerLogic;
    //FPCameraLogic m_cameraLogic;
    AudioSource m_audioSource;

    [SerializeField]
    AudioClip m_takeOutAudio;

    [SerializeField]
    AudioClip m_loadAmmo;

    [SerializeField]
    AudioClip m_runningSound;
    
    private void Start()
    {
        m_weaponLogic = GetComponentInChildren<WeaponLogic>();
        //m_playerLogic = GetComponentInParent<FPSplayerLogic>();
        //m_cameraLogic = GetComponent<FPCameraLogic>();
        m_audioSource = GetComponent<AudioSource>();
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

    public void playTakeOutAudio()
    {
        m_audioSource.PlayOneShot(m_loadAmmo);
        m_audioSource.PlayOneShot(m_takeOutAudio);
    }

    public void playRunAudio()
    {
        m_audioSource.volume = 1;
        m_audioSource.PlayOneShot(m_runningSound);
    }

    public void doKnifeAttack()
    {
        m_weaponLogic.enableKnife();
    }

    public void endKnifeAttack()
    {
        m_weaponLogic.disableKnife();
    }

    public void ThrowGrenade()
    {
        m_weaponLogic.ThrowGrenade();
    }
    #endregion
}
