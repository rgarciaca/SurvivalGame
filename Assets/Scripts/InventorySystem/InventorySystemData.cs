using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SVS.InventorySystem;

namespace Inventory
{
    public class InventorySystemData
    {
        public Action updateHotbarCallback;

        private Storage storagePlayer, storageHotbar;
        List<int> inventoryUIElementIdList = new List<int>();
        List<int> hotbarUIElementIdList = new List<int>();

        public int selectedItemUIID = -1;

        public InventorySystemData(int playerStorageSize, int hotbarStorageSize)
        {
            storagePlayer = new Storage(playerStorageSize);
            storageHotbar = new Storage(hotbarStorageSize);
        }

        public int PlayerStorageLimit { get => storagePlayer.StorageLimit; }

        public void SetSelectedItemTo(int ui_id)
        {
            selectedItemUIID = ui_id;
        }

        public void ResetSelectedItem()
        {
            selectedItemUIID = -1;
        }

        public void AddHotbarUIElement(int ui_id)
        {
            hotbarUIElementIdList.Add(ui_id);
        }

        public void AddInventoryUIElement(int ui_id)
        {
            inventoryUIElementIdList.Add(ui_id);
        }

        public void ClearInventoryUIElements()
        {
            inventoryUIElementIdList.Clear();
        }

        public int AddToStorage(IInventoryItem item)
        {
            int countLeft = item.Count;
            if (storageHotbar.CheckIfStorageContains(item.ID))
            {
                countLeft = storageHotbar.AddItem(item);
                if (countLeft == 0)
                {
                    updateHotbarCallback.Invoke();
                    return countLeft;
                }
            }
            countLeft = storagePlayer.AddItem(item.ID, countLeft, item.IsStackable, item.StackLimit);
            if (countLeft > 0)
            {
                countLeft = storageHotbar.AddItem(item.ID, countLeft, item.IsStackable, item.StackLimit);
                if (countLeft == 0)
                {
                    updateHotbarCallback.Invoke();
                    return countLeft;
                }
            }
            return countLeft;
        }

        internal List<ItemData> GetItemsDataFromInventory()
        {
            return storagePlayer.GetItemsData();
        }

        internal void SwapStorageItemsInsideInventory(int droppedItemID, int draggedItemID)
        {
            var storage_IdDroppedItem = inventoryUIElementIdList.IndexOf(droppedItemID);
            var storage_IdDraggedItem = inventoryUIElementIdList.IndexOf(draggedItemID);
            var storagedata_IdDraggedItem = storagePlayer.GetItemData(storage_IdDraggedItem);

            if (CheckItemForUIStorageNotEmpty(droppedItemID))
            {
                var storagedata_IdDroppedItem = storagePlayer.GetItemData(storage_IdDroppedItem);

                storagePlayer.SwapItemWithIndexFor(storage_IdDraggedItem, storagedata_IdDroppedItem);
                storagePlayer.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
            }
            else
            {
                storagePlayer.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
                storagePlayer.RemoveItemOfIndex(storage_IdDraggedItem);
            }
        }

        private bool CheckItemForUIStorageNotEmpty(int ui_id)
        {
            return inventoryUIElementIdList.Contains(ui_id) && !storagePlayer.CheckIfItemIsEmpty(inventoryUIElementIdList.IndexOf(ui_id));
        }

        internal void SwapStorageItemsInsideHotbar(int droppedItemID, int draggedItemID)
        {
            var storage_IdDroppedItem = hotbarUIElementIdList.IndexOf(droppedItemID);
            var storage_IdDraggedItem = hotbarUIElementIdList.IndexOf(draggedItemID);
            var storagedata_IdDraggedItem = storageHotbar.GetItemData(storage_IdDraggedItem);

            if (CheckItemForHotbarStorageNotEmpty(droppedItemID))
            {
                var storagedata_IdDroppedItem = storageHotbar.GetItemData(storage_IdDroppedItem);

                storageHotbar.SwapItemWithIndexFor(storage_IdDraggedItem, storagedata_IdDroppedItem);
                storageHotbar.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
            }
            else
            {
                storageHotbar.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
                storageHotbar.RemoveItemOfIndex(storage_IdDraggedItem);
            }
        }

        private bool CheckItemForHotbarStorageNotEmpty(int ui_id)
        {
            return !storageHotbar.CheckIfItemIsEmpty(hotbarUIElementIdList.IndexOf(ui_id));
        }

        internal void SwapStorageHotbarToInventory(int droppedItemID, int draggedItemID)
        {
            var storage_IdDroppedItem = inventoryUIElementIdList.IndexOf(droppedItemID);
            var storage_IdDraggedItem = hotbarUIElementIdList.IndexOf(draggedItemID);
            var storagedata_IdDraggedItem = storageHotbar.GetItemData(storage_IdDraggedItem);

            if (CheckItemForUIStorageNotEmpty(droppedItemID))
            {
                var storagedata_IdDroppedItem = storagePlayer.GetItemData(storage_IdDroppedItem);

                storageHotbar.SwapItemWithIndexFor(storage_IdDraggedItem, storagedata_IdDroppedItem);
                storagePlayer.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
            }
            else
            {
                storagePlayer.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
                storageHotbar.RemoveItemOfIndex(storage_IdDraggedItem);
            }
        }

        internal void SwapStorageInventoryToHotbar(int droppedItemID, int draggedItemID)
        {
            var storage_IdDroppedItem = hotbarUIElementIdList.IndexOf(droppedItemID);
            var storage_IdDraggedItem = inventoryUIElementIdList.IndexOf(draggedItemID);
            var storagedata_IdDraggedItem = storagePlayer.GetItemData(storage_IdDraggedItem);

            if (CheckItemForHotbarStorageNotEmpty(droppedItemID))
            {
                var storagedata_IdDroppedItem = storageHotbar.GetItemData(storage_IdDroppedItem);

                storagePlayer.SwapItemWithIndexFor(storage_IdDraggedItem, storagedata_IdDroppedItem);
                storageHotbar.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
            }
            else
            {
                storageHotbar.SwapItemWithIndexFor(storage_IdDroppedItem, storagedata_IdDraggedItem);
                storagePlayer.RemoveItemOfIndex(storage_IdDraggedItem);
            }
        }
    }
}
