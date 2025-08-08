using UnityEngine;

public interface IEnemyController : IMonoBehaviourController {
    void Die();
    bool IsDead();
    void Shoot();
    bool WillHitBoundary(Vector2 direction);
    void Move(Vector2 direction);
    void KillPlayer();
}

public class EnemyController : MonoBehaviourController, IEnemyController {
    private readonly IEnemyView _view;
    private readonly IEnemyManager _enemyManager;
    private readonly IProjectilePoolManager _projectilePoolManager;
    private readonly IGameManager _gameManager;
    private bool _isDead;

    public EnemyController(IEnemyView view,
        IEnemyManager enemyManager,
        IProjectilePoolManager projectilePoolManager,
        IGameManager gameManager) :
        base(view) {
        _view = view;
        _enemyManager = enemyManager;
        _projectilePoolManager = projectilePoolManager;
        _gameManager = gameManager;
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
        projectile.transform.position = _view.Transform.position + Vector3.down * 0.3f;
        projectile.StartMoving(Vector2.down);
    }

    public bool WillHitBoundary(Vector2 direction) {
        Vector3 newPos = _view.Transform.position + (Vector3)(direction * _view.GetSpeed());

        return newPos.x is < -7 or > 7;
    }

    public void Move(Vector2 direction) {
        _view.Transform.position +=
            (Vector3)(direction *
                      (_view.GetSpeed() + (_gameManager.GetCurrentLevel() - 1) * _view.GetMoveSpeedIncrement()));
    }

    public void KillPlayer() {
        _enemyManager.KillPlayer();
    }
}