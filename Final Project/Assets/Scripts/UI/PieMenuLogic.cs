using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieMenuLogic : MonoBehaviour
{
    Text m_ARAmmoText;
    Text m_handGunAmmoText;
    Text m_healthPackText;

    void OnEnable() {
        // Collect data
        WeaponLogic weaponLogic = FindObjectOfType<WeaponLogic>();
        if (weaponLogic) {
            setAmmoNumber(Weapon.AR, weaponLogic.m_AR.ammo, weaponLogic.m_AR.mag);
            setAmmoNumber(Weapon.handgun, weaponLogic.m_Handgun.ammo, weaponLogic.m_Handgun.mag);
            setHealthPackNumber(weaponLogic.m_healthPack);
        }
    }

    void Awake()
    {
        m_ARAmmoText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        m_handGunAmmoText = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        m_healthPackText = transform.GetChild(2).GetChild(1).GetComponent<Text>();
    }

    // Display functions
    public void setAmmoNumber(Weapon weapon, int ammoNumber, int magNumber) {
        Text ammoText = (weapon == Weapon.AR ? m_ARAmmoText : m_handGunAmmoText);
        ammoText.text = ammoNumber + "/" + magNumber;
    }

    public void setHealthPackNumber(int healthPackNumber) {
        m_healthPackText.text = "" + healthPackNumber;
    }

    // Button functions

    public void SwitchToAR() {
        WeaponLogic weaponLogic = FindObjectOfType<WeaponLogic>();
        if (weaponLogic) {
            weaponLogic.changeWeapon(Weapon.AR);
        }
        //UIManager.Instance.switchState(UIState.Game);
    }

    public void SwitchToHandGun() {
        WeaponLogic weaponLogic = FindObjectOfType<WeaponLogic>();
        if (weaponLogic) {
            weaponLogic.changeWeapon(Weapon.handgun);
        }
        //UIManager.Instance.switchState(UIState.Game);
    }

    public void UseHealthPack() {
        WeaponLogic weaponLogic = FindObjectOfType<WeaponLogic>();
        if (weaponLogic) {
            weaponLogic.useHealthPack();
            setHealthPackNumber(weaponLogic.m_healthPack);
        }
    }


}
