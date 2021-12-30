using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieMenuLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Display functions
    public void setAmmoNumber(Weapon weapon, int ammoNumber, int magNumber) {
        int id = (weapon == Weapon.AR) ? 0 : 1;
        Text ammoText = transform.GetChild(id).GetChild(1).GetComponent<Text>();
        ammoText.text = ammoNumber + "/" + magNumber;
    }

    public void setHealthPackNumber(int healthPackNumber) {
        Text healthPackText = transform.GetChild(2).GetChild(1).GetComponent<Text>();
        healthPackText.text = "" + healthPackNumber;
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


}
