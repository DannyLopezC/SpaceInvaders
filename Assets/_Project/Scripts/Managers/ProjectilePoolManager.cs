using UnityEngine;

public interface IProjectilePoolManager {
    ProjectileView GetPlayerBullet();
    ProjectileView GetEnemyBullet();

    void ReturnPlayerBullet(ProjectileView projectile);
    void ReturnEnemyBullet(ProjectileView projectile);
}

public class ProjectilePoolManager : IProjectilePoolManager {
    private readonly ObjectPool<ProjectileView> _playerBulletPool;
    private readonly ObjectPool<ProjectileView> _enemyBulletPool;

    public ProjectilePoolManager(ProjectileView playerPrefab, ProjectileView enemyPrefab) {
        _playerBulletPool = new ObjectPool<ProjectileView>(playerPrefab, 10);
        _enemyBulletPool = new ObjectPool<ProjectileView>(enemyPrefab, 10);
    }

    public ProjectileView GetPlayerBullet() => _playerBulletPool.Get();
    public ProjectileView GetEnemyBullet() => _enemyBulletPool.Get();

    public void ReturnPlayerBullet(ProjectileView projectile) => _playerBulletPool.ReturnToPool(projectile);
    public void ReturnEnemyBullet(ProjectileView projectile) => _enemyBulletPool.ReturnToPool(projectile);
}