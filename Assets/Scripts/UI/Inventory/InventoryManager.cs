using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : UIBase
{
    // Start is called before the first frame update
    private GameObject inventoryPanel;
    public Inventory inventory;
    public Sprite panelSprite;

    public int defaultDim = 250;
    public int itemPanelsDim = 40;
    public int itemsDim = 20;
    public GameObject[,] itemPanels;
    private int itemPanelsX;
    private int itemPanelsY;
    public int spacing = 5;
    void Start()
    {
        inventoryPanel = transform.GetChild(0).gameObject;
        inventoryPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultDim);
        inventoryPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultDim);
        itemPanelsX = defaultDim / (itemPanelsDim + spacing);
        itemPanelsY = defaultDim / (itemPanelsDim + spacing);
        itemPanels = new GameObject[itemPanelsX, itemPanelsY];

        for (int i = 0; i < itemPanelsX; i++)
        {
            int j2 = 0;
            for (int j = itemPanelsY - 1; j >= 0; j--)
            {
                if (i == 0 && j2 == 0)
                {
                    j2++;
                    continue;
                }
                itemPanels[i, j2] = new GameObject("Slot " + (j2 + i * itemPanelsX + 1).ToString());
                itemPanels[i, j2].transform.parent = inventoryPanel.transform;
                itemPanels[i, j2].transform.localPosition = new Vector3((spacing * i + itemPanelsDim / 2) + i * itemPanelsDim - (defaultDim/2), (spacing * j + itemPanelsDim / 2) + j * itemPanelsDim - (defaultDim / 2), 0);
                RectTransform rt = itemPanels[i, j2].AddComponent<RectTransform>();
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemPanelsDim);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemPanelsDim);
                Image image = itemPanels[i, j2].AddComponent<Image>();

                GameObject itemSprite = new GameObject();
                RectTransform rt2 = itemSprite.AddComponent<RectTransform>();
                rt2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemsDim);
                rt2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemsDim);
                itemSprite.transform.parent = itemPanels[i, j2].transform;
                itemSprite.transform.localPosition = new Vector3(0, 0, 0);

                GridLayoutGroup grid = itemPanels[i, j2].AddComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(itemsDim, itemsDim);
                grid.childAlignment = TextAnchor.MiddleCenter;
                InventorySlot slot = itemPanels[i, j2].AddComponent<InventorySlot>();
                slot.x = i;
                slot.y = j2;
                slot.inventory = inventory;

                Image itemImage = itemSprite.AddComponent<Image>();
                DraggableItem dragItem = itemSprite.AddComponent<DraggableItem>();
                dragItem.image = itemImage;
                dragItem.enabled = false;

                image.sprite = panelSprite;
                j2++;

            }
        }
        this.gameObject.SetActive(false);
    }

    public void UpdateItemSprite(int x, int y, Sprite sprite)
    {
        bool ogActiveVal = gameObject.active;
        this.gameObject.SetActive(true);
        itemPanels[x, y].transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        itemPanels[x, y].transform.GetChild(0).GetComponent<DraggableItem>().enabled = true;
        this.gameObject.SetActive(ogActiveVal);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
