using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour {
    private Player _player;
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
        _player = GetComponent<Player>();
    }

    private void Start(){
        // PlayerInput.Player.MoveTarget.performed += ctx => MouseMoveEvent(ctx);
        PlayerInput.Player.WASD.performed += ctx => WASDEvent(ctx);
        PlayerInput.Player.WASD.canceled += ctx => WASDEvent(ctx);
        PlayerInput.Player.QE.performed += ctx => QEEvent(ctx);
        PlayerInput.Player.QE.canceled += ctx => QEEvent(ctx);

    }

    // private void MouseMoveEvent(InputAction.CallbackContext ctx) => _pathHandler.MouseAxis = ctx.ReadValue<Vector2>();
    private void WASDEvent(InputAction.CallbackContext ctx) => _player.InputAxis = ctx.ReadValue<Vector2>();
    private void QEEvent(InputAction.CallbackContext ctx) => _player.RotationAxis = ctx.ReadValue<float>() * -1f;
    // private void PathEvent(InputAction.CallbackContext ctx) => _pathHandler.SpawnPathNode();

}