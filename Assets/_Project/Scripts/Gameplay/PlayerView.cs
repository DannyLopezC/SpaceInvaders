using System;
using UnityEngine;

/// <summary>
/// Defines the public interface for the player view.
/// Provides access to movement speed, screen limits, shooting cooldown, and health management.
/// </summary>
public interface IPlayerView : IMonoBehaviourView {
    /// <summary>
    /// Gets the player's horizontal movement speed.
    /// </summary>
    float GetMoveSpeed();

    /// <summary>
    /// Gets the horizontal limit of the player's movement.
    /// </summary>
    float GetScreenBorder();

    /// <summary>
    /// Gets the cooldown time (in seconds) between shots.
    /// </summary>
    float GetShootCooldown();

    /// <summary>
    /// Increases the player's health by one.
    /// </summary>
    void AddHealth();
}

/// <summary>
/// Represents the visual and Unity-specific behaviour of the player.
/// Handles serialization of movement speed, screen limits, and cooldowns.
/// Delegates logic to the associated <see cref="IPlayerController"/>.
/// </summary>
public class PlayerView : MonoBehaviourView, IPlayerView {
    private IPlayerController _controller;

    [SerializeField] private float moveSpeed; // Horizontal movement speed of the player
    [SerializeField] private float screenBorders; // Maximum X distance the player can move
    [SerializeField] private float shootCooldown; // Time between consecutive shots

    private const string PROJECTILE_TAG = "Projectile";

    /// <summary>
    /// Returns the associated controller for this view.
    /// </summary>
    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    /// <summary>
    /// Creates and registers the player controller, linking it with required services.
    /// Disables the player GameObject by default until needed.
    /// </summary>
    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new PlayerController(this,
            serviceLocator.GetService<IInputHandler>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            serviceLocator.GetService<IUpdateManager>(),
            GameManager.Instance);

        serviceLocator.AddFactory<IPlayerController>(_ => _controller);

        // Start with player inactive, will be activated when the game starts
        gameObject.SetActive(false);
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }

    public float GetScreenBorder() {
        return screenBorders;
    }

    public float GetShootCooldown() {
        return shootCooldown;
    }

    public void AddHealth() {
        _controller.AddHealth();
    }

    /// <summary>
    /// Detects collisions with projectiles and reduces player health accordingly.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PROJECTILE_TAG)) {
            _controller.RemoveHealth();
        }
    }
}