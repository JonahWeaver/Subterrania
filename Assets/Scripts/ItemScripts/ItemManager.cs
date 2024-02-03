using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemManager : MonoBehaviour
{

    void Awake()
    {
        BakeItemRepository();
        DropItemOnGround(new Vector2(16005f, 16001f), itemRepository["Red Block"]);
    }

    [SerializeField]
    private static Dictionary<string, BaseItem> itemRepository = new Dictionary<string, BaseItem>(); //name to item mapping


    private static Dictionary<int, BaseItem> itemsInOverworld = new Dictionary<int, BaseItem>(); //name to item mapping
    int instanceId = 0;

    [MenuItem("Bake/Item Repository")]
    public static void BakeItemRepository()
    {
        itemRepository = new Dictionary<string, BaseItem>();
        Object[] items = Resources.LoadAll("Items", typeof(BaseItem));

        foreach(Object item in items)
        {
            BaseItem baseItem = (BaseItem)(GameObject.Instantiate(item));
            //Debug.Log(baseItem.name);
            itemRepository[baseItem.name] = baseItem;
        }
    }

    public BaseItem GetItem(string name)
    {
        if(itemRepository.ContainsKey(name))
        {
            BaseItem item = GameObject.Instantiate(itemRepository[name]);
            return item;
        }
        return null;
    }

    public void DropItemOnGround(Vector2 pos, BaseItem item)
    {
        item.amount = 1;
        GameObject obj = new GameObject(item.name);
        OverworldItem itemComponent = obj.AddComponent<OverworldItem>();
        itemComponent.item = item;
        obj.transform.position = pos;
    }

    public void RemoveItemFromGround()
    {

    }

    public void ClearItemsFromGround()
    {

    }
}
