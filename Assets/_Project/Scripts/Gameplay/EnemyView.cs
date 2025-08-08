using System;
using UnityEngine;

/// <summary>
/// Interface representing the enemy's visual and interactive component.
/// </summary>
public interface IEnemyView : IMonoBehaviourView {
    /// <summary>
    /// Whether the enemy is dead.
    /// </summary>
    bool IsDead { get; }

    /// <summary>
    /// Makes the enemy shoot a projectile.
    /// </summary>
    void Shoot();

    /// <summary>
    /// Checks if moving in the given direction will make the enemy hit a boundary.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <returns>True if a collision with a boundary will occur, false otherwise.</returns>
    bool WillHitBoundary(Vector2 direction);

    /// <summary>
    /// Moves the enemy in the given direction.
    /// </summary>
    /// <param name="direction">The movement direction.</param>
    void Move(Vector2 direction);

    /// <summary>
    /// Gets the base movement speed of the enemy.
    /// </summary>
    float GetSpeed();

    /// <summary>
    /// Gets the speed increment applied per game level.
    /// </summary>
    float GetMoveSpeedIncrement();

    /// <summary>
    /// Changes the enemy's visual sprite.
    /// </summary>
    /// <param name="sprite">The new sprite to display.</param>
    void SetSprite(Sprite sprite);
}

/// <summary>
/// Handles the enemy's interactions in the scene, including movement, shooting,
/// collision detection, and communication with its controller.
/// </summary>
public class EnemyView : MonoBehaviourView, IEnemyView {
    private IEnemyController _controller;

    private const string PLAYER_PROJECTILE_TAG = "PlayerProjectile";
    private const string ENEMIES_LIMIT_TAG = "EnemiesLimit";

    /// <inheritdoc/>
    public bool IsDead => _controller.IsDead();

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedIncrement;
    [SerializeField] private SpriteRenderer spriteRenderer;

    /// <inheritdoc/>
    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    /// <inheritdoc/>
    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new EnemyController(
            this,
            serviceLocator.GetService<IEnemyManager>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            GameManager.Instance
        );
    }

    /// <inheritdoc/>
    public void Shoot() {
        _controller.Shoot();
    }

    /// <inheritdoc/>
    public bool WillHitBoundary(Vector2 direction) {
        return _controller.WillHitBoundary(direction);
    }

    /// <inheritdoc/>
    public void Move(Vector2 direction) {
        _controller.Move(direction);
    }

    /// <inheritdoc/>
    public float GetSpeed() {
        return moveSpeed;
    }

    /// <inheritdoc/>
    public float GetMoveSpeedIncrement() {
        return moveSpeedIncrement;
    }

    /// <inheritdoc/>
    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// Handles collisions with player projectiles and scene boundaries.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger.</param>
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PLAYER_PROJECTILE_TAG)) {
            _controller.Die();
        }

        if (other.CompareTag(ENEMIES_LIMIT_TAG)) {
            _controller.KillPlayer();
        }
    }
}