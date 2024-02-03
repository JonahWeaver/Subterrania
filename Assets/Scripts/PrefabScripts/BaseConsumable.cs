using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Basic Consumable")]
public class BaseConsumable : BaseItem
{
    public int restoreHealth;
    public int restoreMana;
    public void Awake()
    {
        type = BaseItem.ItemType.CONSUMABLE;
    }
}
