using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    public enum ItemType
    {
        CONSUMABLE,
        ENHANCER,
        WEAPON,
        ARMOR,
        BLOCK
    }
    //public GameObject prefab;
    public ItemType type;
    [TextArea(15, 20)]
    public string name;
    [TextArea(15, 20)]
    public string description;

    public string id;
    public string instanceId;
    public Sprite icon;
    public int hRestore;
    public int mRestore;
    public int amount;
    public int maxAmount;
    public byte tileTypeIndex;
}
