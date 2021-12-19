using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Health,
    Weapon,
    Scope,
    Ammo
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    [TextArea]
    public string description = "";

    public bool isStackable;

    [Header("Weapon")]
    public GameObject weaponPrefab;
    

}
