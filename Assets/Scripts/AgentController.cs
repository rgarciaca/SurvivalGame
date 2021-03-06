using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public AgentMovement movement;
    public PlayerInput input;
    public HumanoidAnimations agentAnimations;

    public InventorySystem inventorySystem;
    
    BaseState currentState;
    public readonly BaseState movementState = new MovemenState();
    public readonly BaseState jumpState = new JumpState();
    public readonly BaseState fallingState = new FallingState();
    public readonly BaseState inventoryState = new InventoryState();

    private void OnEnable()
    {
        movement = GetComponent<AgentMovement>();
        input = GetComponent<PlayerInput>();
        agentAnimations = GetComponent<HumanoidAnimations>();

        currentState = movementState;
        currentState.EnterState(this);
        AssignInputListeners();
    }

    private void AssignInputListeners()
    {
        input.OnJump += HandleJump;
        input.OnHotbarKey += HandleHotbatInput;
        input.OnToggleInventory += HandleInventoryInput;
    }

    private void Update() {
        currentState.Update();
    }

    private void OnDisable() {
        //input.OnJump -= HandleJump;
        input.OnJump -= currentState.HandleJumpInput;
    }

    private void HandleJump()
    {
        currentState.HandleJumpInput();
    }

    private void HandleInventoryInput()
    {
        currentState.HandleInventoryInput();
    }

    private void HandleHotbatInput(int hotbarkey)
    {
        currentState.HandleHotbarInput(hotbarkey);
    }

    public void TransitionToState(BaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
