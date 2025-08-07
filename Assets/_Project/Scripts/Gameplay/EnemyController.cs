using UnityEngine;

public interface IEnemyController : IMonoBehaviourController {
    void Die();
    bool IsDead();
    void Shoot();
    bool WillHitBoundary(Vector2 direction);
    void Move(Vector2 direction);
}

public class EnemyController : MonoBehaviourController, IEnemyController {
    private readonly IEnemyView _view;
    private readonly IEnemyManager _enemyManager;
    private readonly IProjectilePoolManager _projectilePoolManager;
    private bool _isDead;

    public EnemyController(IEnemyView view,
        IEnemyManager enemyManager,
        IProjectilePoolManager projectilePoolManager) :
        base(view) {
        _view = view;
        _enemyManager = enemyManager;
        _projectilePoolManager = projectilePoolManager;
    }

    public void Die() {
        _view.GameObject.SetActive(false);
        _isDead = true;
    }

    public bool IsDead() {
        return _isDead;
    }

    public void Shoot() {
        ProjectileView projectile = _projectilePoolManager.GetEnemyBullet();
        projectile.transform.position = _view.Transform.position + Vector3.down * 1f;
        projectile.StartMoving(Vector2.down);
    }

    public bool WillHitBoundary(Vector2 direction) {
        Vector3 newPos = _view.Transform.position + (Vector3)(direction * _view.GetSpeed());

        return newPos.x is < -8 or > 8;
    }

    public void Move(Vector2 direction) {
        _view.Transform.position += (Vector3)(direction * _view.GetSpeed());
    }
}