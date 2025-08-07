using UnityEngine;

public interface IProjectilePoolManager {
    ProjectileView GetPlayerBullet();
    ProjectileView GetEnemyBullet();

    void ReturnPlayerBullet(ProjectileView projectile);
    void ReturnEnemyBullet(ProjectileView projectile);
}

public class ProjectilePoolManager : IProjectilePoolManager {
    private readonly ObjectPool<ProjectileView> _playerProjectilePool;
    private readonly ObjectPool<ProjectileView> _enemyProjectilePool;

    public ProjectilePoolManager(ProjectileView playerPrefab, ProjectileView enemyPrefab) {
        _playerProjectilePool = new ObjectPool<ProjectileView>(playerPrefab, 10);
        _enemyProjectilePool = new ObjectPool<ProjectileView>(enemyPrefab, 10);
    }

    public ProjectileView GetPlayerBullet() => _playerProjectilePool.Get();
    public ProjectileView GetEnemyBullet() => _enemyProjectilePool.Get();

    public void ReturnPlayerBullet(ProjectileView projectile) => _playerProjectilePool.ReturnToPool(projectile);
    public void ReturnEnemyBullet(ProjectileView projectile) => _enemyProjectilePool.ReturnToPool(projectile);
}