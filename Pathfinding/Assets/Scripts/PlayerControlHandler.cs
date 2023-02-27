using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerControlHandler : MonoBehaviour {
    private Player _player;
    private PlayerControls _playerInput;
    
    
    public PlayerControls PlayerInput { 
        get {
            if (_playerInput == null){
                _playerInput = new PlayerControls();
            }
            return _playerInput;
        }
    }
    private void OnEnable() => PlayerInput.Enable();
    private void OnDisable() => PlayerInput.Disable();
    private void Awake(){
        _player = GetComponent<Player>();
    }

    private void Start(){
    //     PlayerInput.Player.MoveTarget.performed += ctx => MouseMoveEvent(ctx);
    //     PlayerInput.Player.Move.performed += ctx => WASDEvent(ctx);
    //     PlayerInput.Player.Move.canceled += ctx => WASDEvent(ctx);
    //     PlayerInput.Player.Fire.performed += ctx => PathEvent(ctx);
    //     // PlayerInput.Player.AltFire.performed += ctx =>  CommandEvent(ctx);
    //     // PlayerInput.Player.Move.performed += ctx => MoveEvent(ctx);
    //     // PlayerInput.Player.Dash.performed += ctx => DashEvent(ctx, true);
    //     // PlayerInput.Player.Dash.canceled += ctx => DashEvent(ctx, false);
    }

    // // private void TrackingEvent(InputAction.CallbackContext ctx) => _playerControl.IssueCommand();
    // // private void CommandEvent(InputAction.CallbackContext ctx) => _playerControl.IssueCommand();

    // private void MouseMoveEvent(InputAction.CallbackContext ctx) => _pathHandler.MouseAxis = ctx.ReadValue<Vector2>();
    // private void WASDEvent(InputAction.CallbackContext ctx) => _player.InputAxis = ctx.ReadValue<Vector2>();
    // private void PathEvent(InputAction.CallbackContext ctx) => _pathHandler.SpawnPathNode();
}