using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputHandler {
    event Action<Vector2> OnMoveInput;
    event Action OnShootInput;
    void RemoveListeners();
}

public class InputHandler : IInputHandler {
    public event Action<Vector2> OnMoveInput;
    public event Action OnShootInput;

    private readonly InputSystem_Actions _inputSystemActions;

    public InputHandler(InputSystem_Actions inputSystemActions) {
        _inputSystemActions = inputSystemActions;
        _inputSystemActions.Enable();
        AddListeners();
    }

    private void AddListeners() {
        _inputSystemActions.Player.Move.performed += OnMove;
        _inputSystemActions.Player.Move.canceled += OnMove;
        _inputSystemActions.Player.Attack.performed += OnShoot;
    }

    private void OnMove(InputAction.CallbackContext context) {
        Vector2 moveValue = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(moveValue);
    }

    private void OnShoot(InputAction.CallbackContext context) {
        OnShootInput?.Invoke();
    }

    public void RemoveListeners() {
        _inputSystemActions.Player.Move.performed -= OnMove;
        _inputSystemActions.Player.Move.canceled -= OnMove;
        _inputSystemActions.Player.Attack.performed -= OnShoot;
    }
}