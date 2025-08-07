using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerController : IMonoBehaviourController {
}

public class PlayerController : MonoBehaviourController, IPlayerController {
    private readonly IPlayerView _view;
    private readonly IInputHandler _inputHandler;

    private Vector3 _moveDirection = Vector3.zero;

    public PlayerController(IPlayerView view, IInputHandler inputHandler) : base(view) {
        _view = view;
        _inputHandler = inputHandler;
    }

    public override void OnStart() {
        AddListeners();
    }

    private void AddListeners() {
        _inputHandler.OnMoveInput += UpdateMovementVector;
    }

    private void RemoveListeners() {
        _inputHandler.OnMoveInput -= UpdateMovementVector;
    }

    private void UpdateMovementVector(Vector2 move) {
        _moveDirection = new Vector3(move.x, 0f, 0f);
    }

    public override void OnUpdate() {
        Vector3 position = _view.Transform.position;
        position += _moveDirection * _view.GetMoveSpeed() * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -_view.GetScreenBorder(), _view.GetScreenBorder());
        _view.Transform.position = position;
    }

    public override void OnDestroy() {
        RemoveListeners();
    }
}