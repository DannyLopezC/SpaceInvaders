using System;
using UnityEngine;

/// <summary>
/// Interface for the projectile view.
/// Exposes methods to start movement and retrieve projectile data like speed, lifetime, and explosion prefab.
/// </summary>
public interface IProjectileView : IMonoBehaviourView {
    /// <summary>
    /// Starts moving the projectile in the given direction.
    /// </summary>
    /// <param name="direction">The normalized direction vector (only Y is typically used).</param>
    void StartMoving(Vector2 direction);

    /// <summary>
    /// Gets the projectile's movement speed.
    /// </summary>
    float GetProjectileSpeed();

    /// <summary>
    /// Gets the lifetime of the projectile before it is automatically disabled.
    /// </summary>
    float GetProjectileLifeTime();

    /// <summary>
    /// Gets the explosion prefab associated with the projectile.
    /// </summary>
    GameObject GetExplosionPrefab();
}

/// <summary>
/// Unity MonoBehaviour that represents the projectile in the scene.
/// Handles collision detection, sound effects, and connects to the projectile controller.
/// </summary>
public class ProjectileView : MonoBehaviourView, IProjectileView {
    private IProjectileController _controller;

    [Header("Projectile Settings")] [SerializeField]
    private float projectileSpeed; // Movement speed in units per second

    [SerializeField] private float projectileLifeTime; // Time before auto-disable
    [SerializeField] private bool playerProjectile; // Whether this projectile was fired by the player
    [SerializeField] private GameObject explosionPrefab; // Explosion effect prefab

    // Tags used for collision detection
    private const string ENEMY_TAG = "Enemy";
    private const string PLAYER_TAG = "Player";
    private const string OBSTACLE_TAG = "Obstacle";

    /// <summary>
    /// Returns the linked controller instance.
    /// </summary>
    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    /// <summary>
    /// Creates the controller and injects required dependencies.
    /// </summary>
    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new ProjectileController(
            this,
            serviceLocator.GetService<IUpdateManager>()
        );
    }

    /// <inheritdoc/>
    public void StartMoving(Vector2 direction) {
        _controller.StartMoving(direction);

        // Play firing sound effect
        SoundManager.Instance.PlaySoundEffect("Projectile");
    }

    /// <inheritdoc/>
    public float GetProjectileSpeed() {
        return projectileSpeed;
    }

    /// <inheritdoc/>
    public float GetProjectileLifeTime() {
        return projectileLifeTime;
    }

    /// <inheritdoc/>
    public GameObject GetExplosionPrefab() {
        return explosionPrefab;
    }

    /// <summary>
    /// Called by Unity when this projectile's collider enters another collider.
    /// Handles collision logic depending on the type of object hit.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other) {
        // Player-fired projectile hits an enemy
        if (other.CompareTag(ENEMY_TAG) && playerProjectile) {
            _controller.OnProjectileCollided();
            SoundManager.Instance.PlaySoundEffect("Explosion", 0.8f);
        }

        // Enemy-fired projectile hits the player
        if (other.CompareTag(PLAYER_TAG) && !playerProjectile) {
            _controller.OnProjectileCollided();
        }

        // Any projectile hitting an obstacle
        if (other.CompareTag(OBSTACLE_TAG)) {
            _controller.OnProjectileCollided();
            SoundManager.Instance.PlaySoundEffect("Explosion", 0.8f);
        }
    }
}