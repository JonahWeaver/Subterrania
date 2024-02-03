using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    public ItemManager itemManager;
    public InventoryManager inventoryManager;

    void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();
        inventory.itemManager = itemManager;
        inventory.inventoryManager = inventoryManager;
        inventoryManager.inventory = inventory;
    }

    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "OverworldItem")
        {
            OverworldItem item = other.GetComponent<OverworldItem>();
            if(!item.gettingPickedUp)
            {
                inventory.AddItem(item.GetPickedUp());
            }
        }
    }

}
