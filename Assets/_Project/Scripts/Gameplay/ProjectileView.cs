using System;
using UnityEngine;

public interface IProjectileView : IMonoBehaviourView {
    void StartMoving(Vector2 direction);
    float GetProjectileSpeed();
    float GetProjectileLifeTime();
    GameObject GetExplosionPrefab();
}

public class ProjectileView : MonoBehaviourView, IProjectileView {
    private IProjectileController _controller;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private bool playerProjectile;
    [SerializeField] private GameObject explosionPrefab;

    private const string ENEMY_TAG = "Enemy";
    private const string PLAYER_TAG = "Player";
    private const string OBSTACLE_TAG = "Obstacle";

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
        SoundManager.Instance.PlaySoundEffect("Projectile");
    }

    public float GetProjectileSpeed() {
        return projectileSpeed;
    }

    public float GetProjectileLifeTime() {
        return projectileLifeTime;
    }

    public GameObject GetExplosionPrefab() {
        return explosionPrefab;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(ENEMY_TAG) && playerProjectile) {
            _controller.OnProjectileCollided();
            SoundManager.Instance.PlaySoundEffect("Explosion", 0.8f);
        }

        if (other.CompareTag(PLAYER_TAG) && !playerProjectile) {
            _controller.OnProjectileCollided();
        }

        if (other.CompareTag(OBSTACLE_TAG)) {
            _controller.OnProjectileCollided();
            SoundManager.Instance.PlaySoundEffect("Explosion", 0.8f);
        }
    }
}