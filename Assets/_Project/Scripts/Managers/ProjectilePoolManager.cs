using UnityEngine;

/// <summary>
/// Interface for managing projectile pooling in the game.
/// Provides methods for retrieving and returning player and enemy bullets.
/// </summary>
public interface IProjectilePoolManager {
    /// <summary>
    /// Retrieves an available player bullet from the pool.
    /// </summary>
    ProjectileView GetPlayerBullet();

    /// <summary>
    /// Retrieves an available enemy bullet from the pool.
    /// </summary>
    ProjectileView GetEnemyBullet();

    /// <summary>
    /// Returns a player bullet back to the pool.
    /// </summary>
    void ReturnPlayerBullet(ProjectileView projectile);

    /// <summary>
    /// Returns an enemy bullet back to the pool.
    /// </summary>
    void ReturnEnemyBullet(ProjectileView projectile);
}

/// <summary>
/// Handles pooling for both player and enemy projectiles.
/// This prevents unnecessary Instantiate/Destroy calls during gameplay,
/// improving performance and reducing garbage collection spikes.
/// </summary>
public class ProjectilePoolManager : IProjectilePoolManager {
    private readonly ObjectPool<ProjectileView> _playerProjectilePool;
    private readonly ObjectPool<ProjectileView> _enemyProjectilePool;

    /// <summary>
    /// Creates a new projectile pool manager with separate pools
    /// for player and enemy projectiles.
    /// </summary>
    /// <param name="playerPrefab">Prefab for the player bullet.</param>
    /// <param name="enemyPrefab">Prefab for the enemy bullet.</param>
    public ProjectilePoolManager(ProjectileView playerPrefab, ProjectileView enemyPrefab) {
        _playerProjectilePool = new ObjectPool<ProjectileView>(playerPrefab, 10);
        _enemyProjectilePool = new ObjectPool<ProjectileView>(enemyPrefab, 10);
    }

    public ProjectileView GetPlayerBullet() => _playerProjectilePool.Get();
    public ProjectileView GetEnemyBullet() => _enemyProjectilePool.Get();

    public void ReturnPlayerBullet(ProjectileView projectile) => _playerProjectilePool.ReturnToPool(projectile);
    public void ReturnEnemyBullet(ProjectileView projectile) => _enemyProjectilePool.ReturnToPool(projectile);
}