using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
