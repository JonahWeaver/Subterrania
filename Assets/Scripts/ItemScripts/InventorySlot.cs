using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    // Start is called before the first frame update
    public int x;
    public int y;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        int x2 = draggableItem.parentAfterDrag.GetComponent<InventorySlot>().x;
        int y2 = draggableItem.parentAfterDrag.GetComponent<InventorySlot>().y;
        transform.GetChild(0).parent = draggableItem.parentAfterDrag;
        draggableItem.parentAfterDrag = transform;
        inventory.ItemMoveOntoSpace(x2, y2, x, y);
    }
}
