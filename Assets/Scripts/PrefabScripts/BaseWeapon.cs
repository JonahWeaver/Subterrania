using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class BaseWeapon : BaseItem
{
    public int atk;
    public int mAtk;
    public int acc;
    public int weight;
    public int range;

    public bool equipped;
    public void Awake()
    {
        type = ItemType.WEAPON;
        equipped = false;
    }

    //public void Equip(BaseCharacter c)
    //{
    //    c.Attack.AddModifier(new StatModifier(atk, this));
    //    equipped = true;
    //}

    //public void Unequip(BaseCharacter c)
    //{
    //    c.Attack.RemoveAllModifiersFromSource(this);
    //    equipped = false;
    //}
}