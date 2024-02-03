using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //create data structure for tracking UI focus and prioritizing UI with the highest/lowest focus value
    public InventoryManager inventoryManager;
    public Dictionary<ManagerType, UIBase> typeToManager;
    void Start()
    {
        typeToManager = new Dictionary<ManagerType, UIBase>();
        typeToManager[ManagerType.Inventory] = inventoryManager;
    }
    public enum ManagerType
    {
        NA,
        Inventory
    }
    
    public enum Command
    {
        OpenClose
    }

    public void ProcessCommand(Command cmd, ManagerType mt = ManagerType.NA)
    {
        UIBase manager = typeToManager[mt];
        if(mt == ManagerType.NA)
        {
            //retrieve highest focus ui's manager
        }

        switch(cmd)
        {
            case Command.OpenClose:
                manager.gameObject.SetActive(!inventoryManager.gameObject.active);
                break;
            default:
                break;
        }
    }

    public bool mouseClickOverUI()
    {
        bool ret = false;
        ret |= inventoryManager.GetComponent<UIBase>().mouseInside;
        return ret;
    }
}
