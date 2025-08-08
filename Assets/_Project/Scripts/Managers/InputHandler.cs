using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Defines the interface for an input handler.
/// Translates raw input events into game actions.
/// </summary>
public interface IInputHandler {
    /// <summary>
    /// Triggered when a movement input is detected.
    /// Provides a Vector2 with directional input data.
    /// </summary>
    event Action<Vector2> OnMoveInput;

    /// <summary>
    /// Triggered when a shoot action is detected.
    /// </summary>
    event Action OnShootInput;

    /// <summary>
    /// Removes all registered input listeners.
    /// Should be called when input handling is no longer needed.
    /// </summary>
    void RemoveListeners();
}

/// <summary>
/// Handles player input using Unity's new Input System.
/// Maps movement and attack input actions to game events.
/// </summary>
public class InputHandler : IInputHandler {
    public event Action<Vector2> OnMoveInput;
    public event Action OnShootInput;

    private readonly InputSystem_Actions _inputSystemActions;

    /// <summary>
    /// Creates a new input handler and immediately enables input listening.
    /// </summary>
    /// <param name="inputSystemActions">Reference to the generated Input System actions asset.</param>
    public InputHandler(InputSystem_Actions inputSystemActions) {
        _inputSystemActions = inputSystemActions;
        _inputSystemActions.Enable();
        AddListeners();
    }

    /// <summary>
    /// Subscribes to movement and attack events from the Input System.
    /// </summary>
    private void AddListeners() {
        _inputSystemActions.Player.Move.performed += OnMove;
        _inputSystemActions.Player.Move.canceled += OnMove;
        _inputSystemActions.Player.Attack.performed += OnShoot;
    }

    /// <summary>
    /// Handles movement input and forwards it to subscribers.
    /// </summary>
    /// <param name="context">Callback context containing the movement Vector2 value.</param>
    private void OnMove(InputAction.CallbackContext context) {
        Vector2 moveValue = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(moveValue);
    }

    /// <summary>
    /// Handles shoot input and forwards the event to subscribers.
    /// </summary>
    /// <param name="context">Callback context (value is unused for shooting).</param>
    private void OnShoot(InputAction.CallbackContext context) {
        OnShootInput?.Invoke();
    }

    /// <summary>
    /// Removes all listeners from the Input System.
    /// Prevents memory leaks and unwanted input when disabled.
    /// </summary>
    public void RemoveListeners() {
        _inputSystemActions.Player.Move.performed -= OnMove;
        _inputSystemActions.Player.Move.canceled -= OnMove;
        _inputSystemActions.Player.Attack.performed -= OnShoot;
    }
}