using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerController : IMonoBehaviourController {
}

public class PlayerController : MonoBehaviourController, IPlayerController {
    private readonly IPlayerView _view;
    private readonly IInputHandler _inputHandler;
    private readonly IProjectilePoolManager _projectilePoolManager;

    private Vector3 _moveDirection = Vector3.zero;
    private bool _isMoving;

    public PlayerController(IPlayerView view, IInputHandler inputHandler,
        IProjectilePoolManager projectilePoolManager) : base(view) {
        _view = view;
        _inputHandler = inputHandler;
        _projectilePoolManager = projectilePoolManager;
    }

    public override void OnStart() {
        AddListeners();
    }

    private void AddListeners() {
        _inputHandler.OnMoveInput += UpdateMovementVector;
        _inputHandler.OnShootInput += OnShoot;
    }

    private void RemoveListeners() {
        _inputHandler.OnMoveInput -= UpdateMovementVector;
        _inputHandler.OnShootInput -= OnShoot;
    }

    private void UpdateMovementVector(Vector2 move) {
        _moveDirection = new Vector3(move.x, 0f, 0f);
        _isMoving = _moveDirection != Vector3.zero;
    }

    private void OnShoot() {
        ProjectileView projectile = _projectilePoolManager.GetPlayerBullet();
        projectile.transform.position = _view.Transform.position + Vector3.up * 1f;
        projectile.StartMoving(Vector2.up);
    }

    public override void OnUpdate() {
        if (_isMoving) PlayerMovement();
    }

    private void PlayerMovement() {
        Vector3 position = _view.Transform.position;
        position += _moveDirection * _view.GetMoveSpeed() * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -_view.GetScreenBorder(), _view.GetScreenBorder());
        _view.Transform.position = position;
    }

    public override void OnDestroy() {
        RemoveListeners();
    }
}