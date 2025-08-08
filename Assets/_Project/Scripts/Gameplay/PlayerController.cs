using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerController : IMonoBehaviourController {
    void RemoveHealth();
    void AddHealth();
    event Action OnRemoveHealthEvent;
    void ToggleView(bool toggle);
    int GetHealth();
}

public class PlayerController : MonoBehaviourController, IPlayerController {
    private readonly IPlayerView _view;
    private readonly IInputHandler _inputHandler;
    private readonly IProjectilePoolManager _projectilePoolManager;
    private readonly IUpdateManager _updateManager;
    private readonly IGameManager _gameManager;

    private Vector3 _moveDirection = Vector3.zero;
    private bool _isMoving;
    private bool _canShoot = true;
    private int _currentHealth = 3;

    public event Action OnRemoveHealthEvent;

    public PlayerController(IPlayerView view, IInputHandler inputHandler,
        IProjectilePoolManager projectilePoolManager, IUpdateManager updateManager,
        IGameManager gameManager) : base(view) {
        _view = view;
        _inputHandler = inputHandler;
        _projectilePoolManager = projectilePoolManager;
        _updateManager = updateManager;
        _gameManager = gameManager;
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
        if (!_canShoot || !_gameManager.GetIsPlaying()) return;

        ProjectileView projectile = _projectilePoolManager.GetPlayerBullet();
        projectile.transform.position = _view.Transform.position + Vector3.up * 1f;
        projectile.StartMoving(Vector2.up);
        _canShoot = false;
        _updateManager.StartCoroutine(ShootingCooldown());
    }

    private IEnumerator ShootingCooldown() {
        yield return new WaitForSeconds(_view.GetShootCooldown());
        _canShoot = true;
    }

    public override void OnUpdate() {
        if (_isMoving && _gameManager.GetIsPlaying()) PlayerMovement();
    }

    private void PlayerMovement() {
        Vector3 position = _view.Transform.position;
        position += _moveDirection * _view.GetMoveSpeed() * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -_view.GetScreenBorder(), _view.GetScreenBorder());
        _view.Transform.position = position;
    }

    public void AddHealth() {
        _currentHealth++;
    }

    public void RemoveHealth() {
        _currentHealth--;

        OnRemoveHealth();
        ToggleView(false);
    }

    private void OnRemoveHealth() {
        OnRemoveHealthEvent?.Invoke();
    }

    public void ToggleView(bool toggle) {
        _view.GameObject.SetActive(toggle);
    }

    public int GetHealth() {
        return _currentHealth;
    }

    public override void OnDestroy() {
        RemoveListeners();
    }
}