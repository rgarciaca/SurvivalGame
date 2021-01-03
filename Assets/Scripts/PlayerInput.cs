using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MovementInputVector { get; private set; } 
    public Vector3 MovementDirectionVector { get; private set; }
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        GetMovementInput();
        GetMovementDirection();
    }

    private void GetMovementInput()
    {
        var cameraForwardDirection = mainCamera.transform.forward;
        Debug.DrawRay(mainCamera.transform.position, cameraForwardDirection * 10, Color.red);
        MovementDirectionVector = Vector3.Scale(cameraForwardDirection, (Vector3.right + Vector3.forward));
        Debug.DrawRay(mainCamera.transform.position, MovementDirectionVector * 10, Color.green);
    }

    private void GetMovementDirection()
    {
        MovementInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    } 
}
