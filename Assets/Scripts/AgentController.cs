using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public AgentMovement movement;
    public PlayerInput input;
    public HumanoidAnimations agentAnimations;

    BaseState currentState;
    public readonly BaseState movementState = new MovemenState();
    public readonly BaseState jumpState = new JumpState();
    public readonly BaseState fallingState = new FallingState();

    private void OnEnable()
    {
        movement = GetComponent<AgentMovement>();
        input = GetComponent<PlayerInput>();
        agentAnimations = GetComponent<HumanoidAnimations>();

        currentState = movementState;
        currentState.EnterState(this);
        AssignMovementInputListeners();
    }

    private void AssignMovementInputListeners()
    {
        input.OnJump += HandleJump;
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

    public void TransitionToState(BaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
