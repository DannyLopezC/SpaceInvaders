using System;
using UnityEngine;

public interface IEnemyView : IMonoBehaviourView {
    bool IsDead { get; }
    void Shoot();
    bool WillHitBoundary(Vector2 direction);
    void Move(Vector2 direction);

    float GetSpeed();
}

public class EnemyView : MonoBehaviourView, IEnemyView {
    private IEnemyController _controller;

    private const string PROJECTILE_TAG = "Projectile";

    public bool IsDead => _controller.IsDead();

    [SerializeField] private float moveSpeed;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new EnemyController(this,
            serviceLocator.GetService<IEnemyManager>(),
            serviceLocator.GetService<IProjectilePoolManager>());
    }

    public void Shoot() {
        _controller.Shoot();
    }

    public bool WillHitBoundary(Vector2 direction) {
        return _controller.WillHitBoundary(direction);
    }

    public void Move(Vector2 direction) {
        _controller.Move(direction);
    }

    public float GetSpeed() {
        return moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PROJECTILE_TAG)) {
            _controller.Die();
        }
    }
}