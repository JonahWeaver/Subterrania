using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //static ItemManager itemManager = ItemManager.Instance();;
    public BaseItem[,] items;
    public List<BaseItem> tempItems;
    public ItemManager itemManager;
    public InventoryManager inventoryManager;
    [Range(0, 20)]
    public int m_inventoryDimX;
    [Range(0, 20)]
    public int m_inventoryDimY;

    void Awake()
    {
        //itemManager = ItemManager.Instance();
        tempItems = new List<BaseItem>();
        m_inventoryDimX = 10;
        m_inventoryDimY = 10;
        items = new BaseItem[m_inventoryDimX, m_inventoryDimY];
    }

    public BaseItem RetrieveItem(string name)
    {
        return itemManager.GetItem(name);
    }

    public void AddItem(string name)
    {
        BaseItem item = RetrieveItem(name);

        for (int j = 0; j < m_inventoryDimY; j++)
        {
            for (int i = 0; i < m_inventoryDimX; i++)
            {
                if (j == 0 && i == 0) continue;
                if (items[i, j] != null && items[i, j].name == item.name)
                {
                    items[i, j].amount += item.amount;
                    Debug.Log(items[i, j].amount);
                    return;
                }
            }
        }

        for (int j = 0; j < m_inventoryDimY; j++)
        {
            for (int i = 0; i < m_inventoryDimX; i++)
            {
                if (j == 0  && i == 0) continue;
                if (items[i, j] == null)
                {
                    items[i, j] = item;
                    //Debug.Log(items[i,j].name);
                    inventoryManager.UpdateItemSprite(i, j, item.icon);
                    return;
                }
            }
        }


    }

    public void RemoveItem(int i, int j, int amount)
    {
        Debug.Log(items[i, j].amount);
        if (items[i,j].amount <= amount)
        {
            ClearItem(i, j);
        }
        else
        {
            items[i,j].amount -= amount;
        }
    }

    public void ClearItem(int i, int j)
    {
        items[i,j] = null;
        inventoryManager.UpdateItemSprite(i, j, null);
    }

    public void SwitchItems(int i1, int j1, int i2, int j2)
    {
        
    }

    public void OrganizeItems()
    {
        
    }

    public void ItemMoveOntoSpace(int i1, int j1, int i2, int j2)
    {
        BaseItem item1 = items[i1, j1];
        BaseItem item2 = items[i2, j2];
        //Debug.Log(i1.ToString() + "!" + j1.ToString());
        //Debug.Log(i2.ToString() + "?" + j2.ToString());

        if (item2 != null && item2.name == item1.name)
        {
            item2.amount += item1.amount;
            if (item2.amount > item2.maxAmount)
            {
                item1.amount = item2.amount - item2.maxAmount;
                item2.amount = item2.maxAmount;
            }
        }
        else
        {
            items[i1, j1] = item2;
            items[i2, j2] = item1;
        }

    }

    public BaseItem GetFirstItem()
    {
        for (int j = 0; j < m_inventoryDimY; j++)
        {
            for (int i = 0; i < m_inventoryDimX; i++)
            {
                if (j == 0 && i == 0) continue;
                if (items[i, j] != null)
                {
                    //Debug.Log(i.ToString() + " " + j.ToString());
                    BaseItem item = items[i, j];
                    RemoveItem(i, j, 1);
                    return item;
                }
            }
        }
        return null;
    }
}
