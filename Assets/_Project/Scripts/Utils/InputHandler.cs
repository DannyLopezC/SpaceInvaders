using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputHandler {
    public event Action OnPlayerClickInteraction;
}

public class InputHandler : IInputHandler {
    public event Action OnPlayerClickInteraction;

    private readonly InputSystem_Actions _inputSystemActions;

    public InputHandler(InputSystem_Actions inputSystemActions) {
        _inputSystemActions = inputSystemActions;
        _inputSystemActions.Enable();
        AddListeners();
    }

    private void AddListeners() {
        _inputSystemActions.Player.Attack.performed += OnPlayerClick;
    }

    private void OnPlayerClick(InputAction.CallbackContext callbackContext) {
        OnPlayerClickInteraction?.Invoke();
    }
}