using UnityEngine;

/// <summary>
/// Interface for controlling enemy behavior.
/// </summary>
public interface IEnemyController : IMonoBehaviourController {
    /// <summary>
    /// Kills the enemy and disables it from the scene.
    /// </summary>
    void Die();

    /// <summary>
    /// Returns whether the enemy is already dead.
    /// </summary>
    bool IsDead();

    /// <summary>
    /// Makes the enemy shoot a projectile.
    /// </summary>
    void Shoot();

    /// <summary>
    /// Checks if moving in the given direction will cause the enemy to hit a boundary.
    /// </summary>
    /// <param name="direction">The direction of movement.</param>
    /// <returns>True if the enemy will hit a boundary, false otherwise.</returns>
    bool WillHitBoundary(Vector2 direction);

    /// <summary>
    /// Moves the enemy in the given direction.
    /// </summary>
    /// <param name="direction">The movement direction.</param>
    void Move(Vector2 direction);

    /// <summary>
    /// Triggers the action to kill the player.
    /// </summary>
    void KillPlayer();
}

/// <summary>
/// Implementation of the enemy controller, handling movement, shooting, and death.
/// </summary>
public class EnemyController : MonoBehaviourController, IEnemyController {
    private readonly IEnemyView _view;
    private readonly IEnemyManager _enemyManager;
    private readonly IProjectilePoolManager _projectilePoolManager;
    private readonly IGameManager _gameManager;

    private bool _isDead;
    private float _currentMovementIncrement;

    /// <summary>
    /// Constructor for the EnemyController.
    /// </summary>
    /// <param name="view">The visual representation of the enemy.</param>
    /// <param name="enemyManager">Manager that controls all enemies in the scene.</param>
    /// <param name="projectilePoolManager">Manager for reusing projectile objects.</param>
    /// <param name="gameManager">Main game manager.</param>
    public EnemyController(
        IEnemyView view,
        IEnemyManager enemyManager,
        IProjectilePoolManager projectilePoolManager,
        IGameManager gameManager
    ) : base(view) {
        _view = view;
        _enemyManager = enemyManager;
        _projectilePoolManager = projectilePoolManager;
        _gameManager = gameManager;

        // Increase movement speed based on the current game level.
        _currentMovementIncrement = (_gameManager.GetCurrentLevel() - 1) * _view.GetMoveSpeedIncrement();
    }

    /// <inheritdoc/>
    public void Die() {
        _view.GameObject.SetActive(false);
        _isDead = true;
    }

    /// <inheritdoc/>
    public bool IsDead() {
        return _isDead;
    }

    /// <inheritdoc/>
    public void Shoot() {
        ProjectileView projectile = _projectilePoolManager.GetEnemyBullet();
        projectile.transform.position = _view.Transform.position + Vector3.down * 0.3f;
        projectile.StartMoving(Vector2.down);
    }

    /// <inheritdoc/>
    public bool WillHitBoundary(Vector2 direction) {
        Vector3 newPos = _view.Transform.position + (Vector3)(direction * _view.GetSpeed());

        // Prevent movement outside horizontal screen limits.
        switch (newPos.x) {
            case < -7 when direction.x < 0:
            case > 7 when direction.x > 0:
                return true;
            default:
                return false;
        }
    }

    /// <inheritdoc/>
    public void Move(Vector2 direction) {
        _view.Transform.position +=
            (Vector3)(direction * (_view.GetSpeed() + _currentMovementIncrement));
    }

    /// <inheritdoc/>
    public void KillPlayer() {
        _enemyManager.KillPlayer();
    }
}