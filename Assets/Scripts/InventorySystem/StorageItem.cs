using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SVS.InventorySystem
{
    public class StorageItem : IInventoryItem
    {
        private string id;
        public string ID 
        {
            get { return id; }
            private set { id = value; }
        }

        private bool isStackable;
        public bool IsStackable 
        {
            get { return isStackable; }            
            private set 
            {
                isStackable = value;
                if (!isStackable)
                {
                    isFull = true;
                }
            }
        }

        private bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            private set 
            { 
                isEmpty = value; 
                if (isEmpty)
                {
                    isFull = false;
                }
            }
        }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            private set 
            { 
                isFull = value; 
                if (isFull)
                {
                    isEmpty = false;
                }
            }
        }

        private int stackLimit;
        public int StackLimit 
            {
            get { return stackLimit; }            
            private set { stackLimit = value; }
        }

        private int count;
        public int Count 
        {
            get { return count; }            
            private set 
            { 
                count = value;
                if (IsStackable)
                {
                    if (count >= StackLimit)
                    {
                        count = StackLimit;
                        IsFull = true;
                        IsEmpty = false;
                    }
                    else
                    {
                        IsEmpty = false;
                        IsFull = false;
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        IsEmpty = false;
                        IsFull = true;
                    }
                }
                if (count <= 0)
                {
                    count = 0;
                    IsEmpty = true;
                    IsFull = false;
                }
           }
        }

        public StorageItem(string id, int count, bool isStackable = true, int stackLimit = 100)
        {
            ID = id;
            IsStackable= isStackable;
            StackLimit = stackLimit;
            Count = count;
        }

        public virtual int AddToItem(int quantityToAdd)
        {
            if (!IsStackable || IsFull)
            {
                return quantityToAdd;
            }
            int availableStorage = StackLimit - Count;
            if (availableStorage - quantityToAdd < 0)
            {
                Count = StackLimit;
                return quantityToAdd - availableStorage;
            }

            Count += quantityToAdd;
            return 0;
        }

        public virtual int TakeFromItem(int quantity)
        {
            if (quantity < Count)
            {
                Count -= quantity;
                return quantity;
            }
            else
            {
                var temp = Count;
                Count = 0;
                return temp;
            }
        }

        public virtual void ChangeStackLimit(int newLimit)
        {
            if(newLimit <= 0)
            {
                return;
            }
            StackLimit = newLimit;
            if (Count >= StackLimit)
            {
                Count = StackLimit;
                IsFull = true;
            }
            else
            {
                IsFull = false;
            }
        }
    }
}
