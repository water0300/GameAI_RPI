using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AgentManager))]
public class PlayerInputHandler : MonoBehaviour {
    private AgentManager _playerControl;
    private PlayerInputActions _playerInput;
    public PlayerInputActions PlayerInput { 
        get {
            if (_playerInput == null){
                _playerInput = new PlayerInputActions();
            }
            return _playerInput;
        }
    }
    private void OnEnable() => PlayerInput.Enable();
    private void OnDisable() => PlayerInput.Disable();
    private void Awake(){
        _playerControl = GetComponent<AgentManager>();
    }

    private void Start(){
        PlayerInput.Player.MoveTarget.performed += ctx => MouseMoveEvent(ctx);
        // PlayerInput.Player.AltFire.performed += ctx =>  CommandEvent(ctx);
        // PlayerInput.Player.Move.performed += ctx => MoveEvent(ctx);
        // PlayerInput.Player.Dash.performed += ctx => DashEvent(ctx, true);
        // PlayerInput.Player.Dash.canceled += ctx => DashEvent(ctx, false);
    }

    // private void TrackingEvent(InputAction.CallbackContext ctx) => _playerControl.IssueCommand();
    // private void CommandEvent(InputAction.CallbackContext ctx) => _playerControl.IssueCommand();

    private void MouseMoveEvent(InputAction.CallbackContext ctx) => _playerControl.OnMouseMoved(ctx.ReadValue<Vector2>());


}