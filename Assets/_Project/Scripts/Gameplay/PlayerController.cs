using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Defines the public interface for controlling the player entity.
/// </summary>
public interface IPlayerController : IMonoBehaviourController {
    /// <summary>
    /// Decreases the player's health by one and triggers related events.
    /// </summary>
    void RemoveHealth();

    /// <summary>
    /// Increases the player's health by one.
    /// </summary>
    void AddHealth();

    /// <summary>
    /// Event triggered when the player's health is reduced.
    /// </summary>
    event Action OnRemoveHealthEvent;

    /// <summary>
    /// Enables or disables the player view (e.g., for respawn or death).
    /// </summary>
    void ToggleView(bool toggle);

    /// <summary>
    /// Gets the current amount of health the player has.
    /// </summary>
    int GetHealth();

    /// <summary>
    /// Resets the player's health to its starting value.
    /// </summary>
    void RestartHealth();
}

/// <summary>
/// Handles player movement, shooting, health management, and event subscriptions.
/// </summary>
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

    /// <summary>
    /// Called when the controller starts. Subscribes to input events.
    /// </summary>
    public override void OnStart() {
        AddListeners();
    }

    /// <summary>
    /// Subscribes to input handler events for movement and shooting.
    /// </summary>
    private void AddListeners() {
        _inputHandler.OnMoveInput += UpdateMovementVector;
        _inputHandler.OnShootInput += OnShoot;
    }

    /// <summary>
    /// Unsubscribes from input handler events to prevent memory leaks.
    /// </summary>
    private void RemoveListeners() {
        _inputHandler.OnMoveInput -= UpdateMovementVector;
        _inputHandler.OnShootInput -= OnShoot;
    }

    /// <summary>
    /// Updates the player's movement vector based on input.
    /// </summary>
    private void UpdateMovementVector(Vector2 move) {
        _moveDirection = new Vector3(move.x, 0f, 0f);
        _isMoving = _moveDirection != Vector3.zero;
    }

    /// <summary>
    /// Attempts to shoot a projectile if allowed and the game is in play.
    /// </summary>
    private void OnShoot() {
        if (!_canShoot || !_gameManager.GetIsPlaying()) return;

        ProjectileView projectile = _projectilePoolManager.GetPlayerBullet();
        projectile.transform.position = _view.Transform.position + Vector3.up * 1f;
        projectile.StartMoving(Vector2.up);

        _canShoot = false;
        _updateManager.StartCoroutine(ShootingCooldown());
    }

    /// <summary>
    /// Prevents the player from shooting again until the cooldown finishes.
    /// </summary>
    private IEnumerator ShootingCooldown() {
        yield return new WaitForSeconds(_view.GetShootCooldown());
        _canShoot = true;
    }

    /// <summary>
    /// Called every frame. Moves the player if input is active and the game is running.
    /// </summary>
    public override void OnUpdate() {
        if (_isMoving && _gameManager.GetIsPlaying()) PlayerMovement();
    }

    /// <summary>
    /// Moves the player horizontally, clamping position within the screen borders.
    /// </summary>
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

    public void RestartHealth() {
        _currentHealth = 3;
    }

    /// <summary>
    /// Called when the object is destroyed. Removes all event subscriptions.
    /// </summary>
    public override void OnDestroy() {
        RemoveListeners();
    }
}