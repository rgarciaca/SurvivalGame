using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryState : BaseState
{
    public override void EnterState(AgentController controller)
    {
        base.EnterState(controller);
        controllerReference.inventorySystem.ToggleInventory();
        Time.timeScale = 0;
         Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void HandleInventoryInput()
    {
        base.HandleInventoryInput();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controllerReference.inventorySystem.ToggleInventory();
        controllerReference.TransitionToState(controllerReference.movementState);
    }

}
