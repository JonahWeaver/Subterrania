using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Items/Block")]
public class BaseBlock : BaseItem
{
    public bool gravity; //(?)
    public void Awake()
    {
        type = ItemType.BLOCK;
    }
}