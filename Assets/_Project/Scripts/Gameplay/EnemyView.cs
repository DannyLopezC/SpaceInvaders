using System;
using UnityEngine;

public interface IEnemyView : IMonoBehaviourView {
    bool IsDead { get; }
    void Shoot();
    bool WillHitBoundary(Vector2 direction);
    void Move(Vector2 direction);

    float GetSpeed();
    float GetMoveSpeedIncrement();
    void SetSprite(Sprite sprite);
}

public class EnemyView : MonoBehaviourView, IEnemyView {
    private IEnemyController _controller;

    private const string PLAYER_PROJECTILE_TAG = "PlayerProjectile";
    private const string ENEMIES_LIMIT_TAG = "EnemiesLimit";

    public bool IsDead => _controller.IsDead();

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedIncrement;
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new EnemyController(this,
            serviceLocator.GetService<IEnemyManager>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            GameManager.Instance);
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

    public float GetMoveSpeedIncrement() {
        return moveSpeedIncrement;
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PLAYER_PROJECTILE_TAG)) {
            _controller.Die();
        }

        if (other.CompareTag(ENEMIES_LIMIT_TAG)) {
        }
    }
}