using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour
{
    public GameObject inventoryGeneralPanel;

    public bool IsInventoryVisible { get => inventoryGeneralPanel.activeSelf; }
    public int HotbarElementsCount { get =>hotbarUiItems.Count;}
    public RectTransform Draggableitem { get => draggableitem; }
    public ItemPanelHelper DraggableItemPanel { get => draggableItemPanel; }

    public Dictionary<int, ItemPanelHelper> inventoryUiItems = new Dictionary<int, ItemPanelHelper>();
    public Dictionary<int, ItemPanelHelper> hotbarUiItems = new Dictionary<int, ItemPanelHelper>();

    private List<int> listOfHotbarElementsID = new List<int>();

    public List<ItemPanelHelper> GetUiElementsForHotbar()
    {
        return hotbarUiItems.Values.ToList();
    }

    public GameObject hotbarPanel, storagePanel;

    public GameObject storagePrefab;

    public Canvas canvas;

    private RectTransform draggableitem;
    private ItemPanelHelper draggableItemPanel;

    private void Awake()
    {
        inventoryGeneralPanel.SetActive(false);
        foreach (Transform child in hotbarPanel.transform)
        {
            ItemPanelHelper helper = child.GetComponent<ItemPanelHelper>();
            if (helper != null)
            {
                hotbarUiItems.Add(helper.GetInstanceID(), helper);
                helper.isHotbarItem = true;
            }
        }
        listOfHotbarElementsID = hotbarUiItems.Keys.ToList();
    }

    public void ToggleUI()
    {
        if(inventoryGeneralPanel.activeSelf == false)
        {
            inventoryGeneralPanel.SetActive(true);
        }
        else
        {
            inventoryGeneralPanel.SetActive(false);
            DestroyDraggedObject();
        }
    }

    public void PrepareInventoryItems(int playerStorageLimit)
    {
        for (int i = 0; i < playerStorageLimit; i++)
        {
            foreach (Transform child in storagePanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
        inventoryUiItems.Clear();
        for (int i = 0; i < playerStorageLimit; i++)
        {
            var element = Instantiate(storagePrefab, Vector3.zero, Quaternion.identity, storagePanel.transform);
            var itemHelper = element.GetComponent<ItemPanelHelper>();
            inventoryUiItems.Add(itemHelper.GetInstanceID(), itemHelper);
        }
    }

    public List<ItemPanelHelper> GetUiElementsForInventory()
    {
        return inventoryUiItems.Values.ToList();
    }

    public void DestroyDraggedObject()
    {
        if (Draggableitem != null)
        {
            Destroy(Draggableitem.gameObject);
            draggableItemPanel = null;
            draggableitem = null;
        }
    }

    public void CreateDraggableItem(int ui_id)
    {
        if (CheckItemInInventory(ui_id))
        {
            draggableItemPanel = inventoryUiItems[ui_id];
        }
        else
        {
            draggableItemPanel = hotbarUiItems[ui_id];
        }

        Image itemImage = DraggableItemPanel.itemImage;
        var imageObject = Instantiate(itemImage, itemImage.transform.position, Quaternion.identity, canvas.transform);
        imageObject.raycastTarget = false;
        imageObject.sprite = itemImage.sprite;

        draggableitem = imageObject.GetComponent<RectTransform>();
        draggableitem.sizeDelta = new Vector2(60, 60);
    }

    public bool CheckItemInInventory(int ui_id)
    {
        return inventoryUiItems.ContainsKey(ui_id);
    }

    public void MoveDraggableItem(PointerEventData eventData)
    {
        var valueToAdd = eventData.delta / canvas.scaleFactor;
        Draggableitem.anchoredPosition += valueToAdd;
    }

    internal void SwapUIItemInventoryToInventory(int droppedItemID, int draggedItemID)
    {
        var tempName = inventoryUiItems[draggedItemID].itemName;
        var tempSprite = inventoryUiItems[draggedItemID].itemImage.sprite;
        var tempCount = inventoryUiItems[draggedItemID].itemCount;
        var tempIsEmpty = inventoryUiItems[draggedItemID].isEmpty;

        var droppedItemData = inventoryUiItems[droppedItemID];
        inventoryUiItems[draggedItemID].SwapWithData(droppedItemData.itemName, droppedItemData.itemCount,
                        droppedItemData.itemImage.sprite, droppedItemData.isEmpty);

        inventoryUiItems[droppedItemID].SwapWithData(tempName, tempCount, tempSprite, tempIsEmpty);

        DestroyDraggedObject();
    }

    internal bool CheckItemInHotbar(int ui_id)
    {
        return hotbarUiItems.ContainsKey(ui_id);
    }

    internal void SwapUIItemHotbarToHotbar(int droppedItemID, int draggedItemID)
    {
        var tempName = hotbarUiItems[draggedItemID].itemName;
        var tempSprite = hotbarUiItems[draggedItemID].itemImage.sprite;
        var tempCount = hotbarUiItems[draggedItemID].itemCount;
        var tempIsEmpty = hotbarUiItems[draggedItemID].isEmpty;

        var droppedItemData = hotbarUiItems[droppedItemID];
        hotbarUiItems[draggedItemID].SwapWithData(droppedItemData.itemName, droppedItemData.itemCount,
                        droppedItemData.itemImage.sprite, droppedItemData.isEmpty);

        hotbarUiItems[droppedItemID].SwapWithData(tempName, tempCount, tempSprite, tempIsEmpty);

        DestroyDraggedObject();
    }

    internal void SwapUIItemHotbarToInventory(int droppedItemID, int draggedItemID)
    {
        var tempName = hotbarUiItems[draggedItemID].itemName;
        var tempSprite = hotbarUiItems[draggedItemID].itemImage.sprite;
        var tempCount = hotbarUiItems[draggedItemID].itemCount;
        var tempIsEmpty = hotbarUiItems[draggedItemID].isEmpty;

        var droppedItemData = inventoryUiItems[droppedItemID];
        hotbarUiItems[draggedItemID].SwapWithData(droppedItemData.itemName, droppedItemData.itemCount,
                        droppedItemData.itemImage.sprite, droppedItemData.isEmpty);

        inventoryUiItems[droppedItemID].SwapWithData(tempName, tempCount, tempSprite, tempIsEmpty);

        DestroyDraggedObject();
    }

    internal void SwapUIItemInventoryToHotbar(int droppedItemID, int draggedItemID)
    {
        var tempName = inventoryUiItems[draggedItemID].itemName;
        var tempSprite = inventoryUiItems[draggedItemID].itemImage.sprite;
        var tempCount = inventoryUiItems[draggedItemID].itemCount;
        var tempIsEmpty = inventoryUiItems[draggedItemID].isEmpty;

        var droppedItemData = hotbarUiItems[droppedItemID];
        inventoryUiItems[draggedItemID].SwapWithData(droppedItemData.itemName, droppedItemData.itemCount,
                        droppedItemData.itemImage.sprite, droppedItemData.isEmpty);

        hotbarUiItems[droppedItemID].SwapWithData(tempName, tempCount, tempSprite, tempIsEmpty);

        DestroyDraggedObject();
    }
}