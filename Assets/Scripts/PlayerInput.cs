using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MovementInputVector { get; private set; } 
    public Vector3 MovementDirectionVector { get; set; }

    public Action OnJump { get; set; }
    public Action OnToggleInventory { get; set; }
    public Action<int> OnHotbarKey { get; set; }

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        GetMovementInput();
        GetMovementDirection();
        GetJumpInput();
        GetInventoryInput();
        GetHotbarInput();
    }

    private void GetHotbarInput()
    {
        char hotbar0 = '0';
        for (int i = 0; i < 9; i++)
        {
            KeyCode keycode = (KeyCode)((int)hotbar0 + i);
            if (Input.GetKeyDown(keycode))
            {
                OnHotbarKey?.Invoke(i);
                return;
            }
        }
    }

    private void GetInventoryInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnToggleInventory?.Invoke();
        }
    }

    private void GetJumpInput()
    {
        if (Input.GetAxisRaw("Jump") > 0)
        {
            OnJump?.Invoke(); 
        }
    }

    private void GetMovementInput()
    {
        var cameraForwardDirection = mainCamera.transform.forward;
        //Debug.DrawRay(mainCamera.transform.position, cameraForwardDirection * 10, Color.red);
        MovementDirectionVector = Vector3.Scale(cameraForwardDirection, (Vector3.right + Vector3.forward));
        //Debug.DrawRay(mainCamera.transform.position, MovementDirectionVector * 10, Color.green);
    }

    private void GetMovementDirection()
    {
        MovementInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    } 
}
