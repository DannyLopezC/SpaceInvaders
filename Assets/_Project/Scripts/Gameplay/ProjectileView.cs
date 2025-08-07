using System;
using UnityEngine;

public interface IProjectileView : IMonoBehaviourView {
    void StartMoving(Vector2 direction);
    float GetProjectileSpeed();
    float GetProjectileLifeTime();
}

public class ProjectileView : MonoBehaviourView, IProjectileView {
    private IProjectileController _controller;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;

    private const string ENEMY_TAG = "Enemy";

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new ProjectileController(this,
            serviceLocator.GetService<IUpdateManager>());
    }

    public void StartMoving(Vector2 direction) {
        _controller.StartMoving(direction);
    }

    public float GetProjectileSpeed() {
        return projectileSpeed;
    }

    public float GetProjectileLifeTime() {
        return projectileLifeTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(ENEMY_TAG)) {
            _controller.OnProjectileCollided();
        }
    }
}