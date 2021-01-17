using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS.InventorySystem
{
    public class Storage
    {
        private int storageLimit;
        public int StorageLimit
        {
            get { return storageLimit; }
            set { storageLimit = value; }
        }
        
        List<StorageItem> storageItems;

        public Storage(int storageLimit)
        {
            StorageLimit = storageLimit;
            storageItems = new List<StorageItem>();
            for (int i = 0; i < StorageLimit; i++)
            {
                storageItems.Add(null);
            }
        }

        /// <summary>
        /// Returns 0 if successfuly added. Else return amoun that is left.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        public int AddItem(IInventoryItem inventoryItem)
        {
            int remainingAmount = TryAddingToAnExistingItem(inventoryItem.ID, inventoryItem.Count);
            if (remainingAmount == 0 || CheckIfStorageIsFull())
            {
                return remainingAmount;
            }

            StorageItem newStorageItem = new StorageItem(inventoryItem.ID, remainingAmount, inventoryItem.IsStackable, inventoryItem.StackLimit);

            int nullItemIndex = storageItems.FindIndex(x => x == null);
            
            if (nullItemIndex == -1)
            {
                
                storageItems.Add(newStorageItem);
            }
            else
            {
                storageItems[nullItemIndex] = newStorageItem;
            }
            return 0;
        }

        /// <summary>
        /// Returns 0 if successfuly added. Else return amoun that is left.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        public int AddItem(string ID, int count, bool isStackable = true, int stackLimit = 100)
        {
            int remainingAmount = TryAddingToAnExistingItem(ID, count);
            if (remainingAmount == 0 || CheckIfStorageIsFull())
            {
                return remainingAmount;
            }
            StorageItem newStorageItem = new StorageItem(ID, remainingAmount, isStackable, stackLimit);

            int nullItemIndex = storageItems.FindIndex(x => x == null);

            if (nullItemIndex == -1)
            {

                storageItems.Add(newStorageItem);
            }
            else
            {
                storageItems[nullItemIndex] = newStorageItem;
            }

            return 0;
        }

        /// <summary>
        /// Swaps item with Index to provided InventoryItem data
        /// </summary>
        /// <param name="index"></param>
        /// <param name="inventoryItemData"></param>
        public void SwapItemWithIndexFor(int index, IInventoryItem inventoryItemData)
        {
            storageItems[index] = null;
            StorageItem newStorageItem = new StorageItem(inventoryItemData.ID, inventoryItemData.Count, inventoryItemData.IsStackable, inventoryItemData.StackLimit);
            storageItems[index] = newStorageItem;
        }

        private bool CheckIfStorageIsFull()
        {
            return storageItems.Any(x => x == null) == false;
        }

        private int TryAddingToAnExistingItem(string ID, int itemCount)
        {
            if (itemCount == 0)
            {
                return 0;
            }
            if (CheckIfStorageContains(ID))
            {
                foreach (var item in storageItems)
                {
                    if (item != null && item.ID == ID && !item.IsFull)
                    {
                        itemCount = item.AddToItem(itemCount);
                    }
                    if (itemCount == 0)
                        return 0;
                }
            }

            return itemCount;
        }

        public bool CheckIfStorageContains(string ID)
        {
            return storageItems.Any(x => x != null && x.ID == ID);
        }

        public bool TakeItemFromStorageIfContainsEnough(string ID, int quantity)
        {
            if (!CheckIfStorageHasEnoughOfItemWith(ID, quantity))
            {
                return false;
            }
            for (int i = storageItems.Count; i >= 0; i--)
            {
                if (storageItems[i] == null)
                {
                    continue;
                }
                if (storageItems[i].ID == ID)
                {
                    quantity -= storageItems[i].TakeFromItem(quantity);
                }
                if (storageItems[i].IsEmpty)
                {
                    storageItems[i] = null;
                }
                if (quantity <= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIfStorageHasEnoughOfItemWith(string ID, int quantity)
        {
            int amount = 0;
            foreach (var item in storageItems)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.ID == ID)
                {
                    amount += item.Count;
                }
                if (amount >= quantity)
                {
                    return true;
                }
            }

            return false;
        }

        public int TakeFromItemWith(int index, int amountToTake)
        {
            if (storageItems[index] == null)
            {
                return amountToTake;
            }
            if (storageItems[index].Count < amountToTake)
            {
                var temp = storageItems[index].Count;
                storageItems[index] = null;
                return temp;
            }
            storageItems[index].TakeFromItem(amountToTake);
            if (storageItems[index].IsEmpty)
            {
                storageItems[index] = null;
            }

            return amountToTake;
        }

        public void RemoveItemOfIndex(int index)
        {
            storageItems[index] = null;
        }

        public void UpgradeStorage(int capacity)
        {
            StorageLimit += capacity;
            for (int i = 0; i < capacity; i++)
            {
                storageItems.Add(null);
            }
        }

        public string GetIDOfItemWithIndex(int index)
        {
            if (storageItems[index] == null)
            {
                return null;
            }

            return storageItems[index].ID;
        }

        public int GetCountOfItemWithIndex(int index)
        {
            if (storageItems[index] == null)
            {
                return -1;
            }

            return storageItems[index].Count;
        }

        public bool CheckIfItemIsEmpty(int index)
        {
            return storageItems[index] == null;
        }

        public List<ItemData> GetDataToSave()
        {
            List<ItemData> valueToReturn = new List<ItemData>();
            for (int i = 0; i < this.StorageLimit; i++)
            {
                if (storageItems[i] != null)
                {
                    valueToReturn.Add(new ItemData(i, storageItems[i].Count, storageItems[i].ID,
                            storageItems[i].IsStackable, storageItems[i].StackLimit));
                }
                else
                {
                    valueToReturn.Add(new ItemData(true));
                }
            }

            return valueToReturn;
        }

        /// <summary>
        /// Returns copy of data for all the storage items.
        /// </summary>
        /// <returns>List of ItemData for all items inside storage</returns>
        public List<ItemData> GetItemsData()
        {
            List<ItemData> valueToReturn = new List<ItemData>();
            for (int i = 0; i < this.StorageLimit; i++)
            {
                if (storageItems[i] != null)
                {
                    valueToReturn.Add(new ItemData(i, storageItems[i].Count, storageItems[i].ID, storageItems[i].IsStackable, storageItems[i].StackLimit));
                }
                else
                {
                    valueToReturn.Add(new ItemData(true));
                }
            }

            return valueToReturn;
        }

        public ItemData GetItemData(int index)
        {
            if (storageItems[index] == null)
            {
                return new ItemData(true);
            }
            else
            {
                return new ItemData(index, storageItems[index].Count, storageItems[index].ID,
                            storageItems[index].IsStackable, storageItems[index].StackLimit);
            }
        }
    }

    [Serializable]
    public struct ItemData : IInventoryItem
    {
        [SerializeField] private bool isNull;
        [SerializeField] private int storageIndex;
        [SerializeField] private string id;
        [SerializeField] private int count;
        [SerializeField] private bool isStackable;
        [SerializeField] private int stackLimit;

        public string ID 
        {
            get { return id; }
            private set { id = value; }
        }

        public bool IsStackable
        {
            get { return isStackable; }
            private set { isStackable = value; }
        }

        public int StackLimit
        {
            get { return stackLimit; }
            private set { stackLimit = value; }
        }

        public int Count
        {
            get { return count; }
            private set { count  = value; }
        }

        public bool IsNull
        {
            get { return isNull; }
            private set { isNull = value; }
        }

        public int StorageIndex
        {
            get { return storageIndex; }
            private set { storageIndex = value; }
        }

        public ItemData(int storageIndex, int count, string id, bool isStackable, int stackLimit)
        {
            this.id = id;
            this.count = count;
            this.storageIndex = storageIndex;
            this.isStackable = isStackable;
            this.stackLimit = stackLimit;
            this.isNull = false;
        }

        public ItemData (bool isNull = true)
        {
            this.id = string.Empty;
            this.count = -1;
            this.storageIndex = -1;
            this.isStackable = false;
            this.stackLimit = -1;
            this.isNull = isNull;
        }
    }
}
