using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS.InventorySystem
{
    public interface IInventoryItem
    {
        string ID { get; }
        bool IsStackable { get; }
        int StackLimit { get; }
        int Count { get; }
    }
}
