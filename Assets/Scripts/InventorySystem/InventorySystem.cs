using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Inventory;
using SVS.InventorySystem;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
    private UIInventory uiInventory;

    private InventorySystemData inventoryData;

    public int playerStorageSize = 20;

    private void Awake()
    {
        uiInventory = GetComponent<UIInventory>(); 
    }

    private void Start()
    {
        inventoryData = new InventorySystemData(playerStorageSize, uiInventory.HotbarElementsCount);
        inventoryData.updateHotbarCallback += UpdateHotbarHandler;
        ItemData artificialDataItem = new ItemData(0, 20, "8c801dfe0159443d83d0de746c5df9f7", true, 100);
        ItemData artificialDataItem1 = new ItemData(0, 90, "8c801dfe0159443d83d0de746c5df9f7", true, 100);
        ItemData artificialDataItem2 = new ItemData(0, 1, "bdaf5b6d7e674d4890d0a8aa59b8b525", false, 1);
        AddToStorage(artificialDataItem);
        AddToStorage(artificialDataItem1);
        AddToStorage(artificialDataItem2);
        var hotbarUiElementsList = uiInventory.GetUiElementsForHotbar();

        for (int i = 0; i < hotbarUiElementsList.Count; i++)
        {
            inventoryData.AddHotbarUIElement(hotbarUiElementsList[i].GetInstanceID());
            hotbarUiElementsList[i].OnClickEvent += UseHotbarItemHandler;
            hotbarUiElementsList[i].DragContinueCallBack += DraggingHandler;
            hotbarUiElementsList[i].DragStartCallBack += DragStartHandler;
            hotbarUiElementsList[i].DragStopCallBack += DragStopHandler;
            hotbarUiElementsList[i].DropCallBack += DropHandler;
        }
    }

    private void UpdateHotbarHandler()
    {
        throw new NotImplementedException();
    }

    private void UseHotbarItemHandler(int ui_id, bool isEmpty)
    {
        Debug.Log("Using hotbar item");
        if (isEmpty)
            return;
        //throw new NotImplementedException();
        
    }

    public void ToggleInventory()
    {
        if(uiInventory.IsInventoryVisible == false)
        {
            DeselectCurrentItem();
            inventoryData.ClearInventoryUIElements();
            PrepareUI();
            PutDataInUI();
        }
        uiInventory.ToggleUI();
    }

    private void PutDataInUI()
    {
        var uiElementsList = uiInventory.GetUiElementsForInventory();
        var inventoryItemsList = inventoryData.GetItemsDataFromInventory();
        for (int i = 0; i <uiElementsList.Count; i++)
        {
            var uiItemElement = uiElementsList[i];
            var itemData = inventoryItemsList[i];
            if (!itemData.IsNull)
            {
                var itemName = ItemDataManager.instance.GetItemName(itemData.ID);
                var itemSprite = ItemDataManager.instance.GetItemSprite(itemData.ID);
                uiItemElement.SetInventoryUiElement(itemName, itemData.Count, itemSprite);
            }
            inventoryData.AddInventoryUIElement(uiItemElement.GetInstanceID());
        }
    }

    private void PrepareUI()
    {
        uiInventory.PrepareInventoryItems(inventoryData.PlayerStorageLimit);
        AddEventHandlersToInventoryUiElements();
    }

    private void AddEventHandlersToInventoryUiElements()
    {
        foreach (var uiItemElement in uiInventory.GetUiElementsForInventory())
        {
            uiItemElement.OnClickEvent += UiElementSelectedHandler;
            uiItemElement.DragContinueCallBack += DraggingHandler;
            uiItemElement.DragStartCallBack += DragStartHandler;
            uiItemElement.DragStopCallBack += DragStopHandler;
            uiItemElement.DropCallBack += DropHandler;
        }
    }

    private void DropHandler(PointerEventData eventData, int droppedItemID)
    {
        if (uiInventory.Draggableitem != null)
        {
            var draggedItemID = uiInventory.DraggableItemPanel.GetInstanceID();
            if (draggedItemID == droppedItemID)
            {
                return;
            }

            DeselectCurrentItem();
            if (uiInventory.CheckItemInInventory(draggedItemID))
            {
                if (uiInventory.CheckItemInInventory(droppedItemID))
                {
                    DroppingItemsInventoryToInventory(droppedItemID, draggedItemID);
                }
                else 
                {
                    DroppingItemsInventoryToHotbar(droppedItemID, draggedItemID);
                }
            }
            else
            {
                if (uiInventory.CheckItemInInventory(droppedItemID))
                {
                    DroppingItemsHotbarToInventory(droppedItemID, draggedItemID);
                }
                else 
                {
                    DroppingItemsHotbarToHotbar(droppedItemID, draggedItemID);
                }
            }
        }
    }

    private void DroppingItemsHotbarToHotbar(int droppedItemID, int draggedItemID)
    {
        uiInventory.SwapUIItemHotbarToHotbar(droppedItemID, draggedItemID);
        inventoryData.SwapStorageItemsInsideHotbar(droppedItemID, draggedItemID);
    }

    private void DroppingItemsHotbarToInventory(int droppedItemID, int draggedItemID)
    {
        uiInventory.SwapUIItemHotbarToInventory(droppedItemID, draggedItemID);
        inventoryData.SwapStorageHotbarToInventory(droppedItemID, draggedItemID);
    }

    private void DroppingItemsInventoryToHotbar(int droppedItemID, int draggedItemID)
    {
        uiInventory.SwapUIItemInventoryToHotbar(droppedItemID, draggedItemID);
        inventoryData.SwapStorageInventoryToHotbar(droppedItemID, draggedItemID);
    }

    private void DroppingItemsInventoryToInventory(int droppedItemID, int draggedItemID)
    {
        uiInventory.SwapUIItemInventoryToInventory(droppedItemID, draggedItemID);
        inventoryData.SwapStorageItemsInsideInventory(droppedItemID, draggedItemID);
    }

    private void DeselectCurrentItem()
    {
        inventoryData.ResetSelectedItem();
    }

    private void DragStopHandler(PointerEventData eventData)
    {
        uiInventory.DestroyDraggedObject();
    }

    private void DragStartHandler(PointerEventData eventData, int ui_id)
    {
        uiInventory.DestroyDraggedObject();
        uiInventory.CreateDraggableItem(ui_id);
    }

    private void DraggingHandler(PointerEventData eventData)
    {
        uiInventory.MoveDraggableItem(eventData);
    }

    public int AddToStorage(IInventoryItem item)
    {
        int val = inventoryData.AddToStorage(item);
        return val;
    }

    private void UiElementSelectedHandler(int ui_id, bool isEmpty)
    {
        Debug.Log("Selecting inventory item");
        if (isEmpty) return;
        
        DeselectCurrentItem();
        inventoryData.SetSelectedItemTo(ui_id);
        
    }
}
