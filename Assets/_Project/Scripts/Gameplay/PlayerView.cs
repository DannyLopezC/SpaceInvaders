using System;
using UnityEngine;
using UnityEngine.Serialization;

public interface IPlayerView : IMonoBehaviourView {
    float GetMoveSpeed();
    float GetScreenBorder();
    float GetShootCooldown();
    void AddHealth();
    event Action OnRemoveHealthEvent;
}

public class PlayerView : MonoBehaviourView, IPlayerView {
    private IPlayerController _controller;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float screenBorders;

    [SerializeField] private float shootCooldown;

    private const string PROJECTILE_TAG = "Projectile";

    public event Action OnRemoveHealthEvent;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new PlayerController(this,
            serviceLocator.GetService<IInputHandler>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            serviceLocator.GetService<IUpdateManager>());

        _controller.OnRemoveHealthEvent += OnRemoveHealth;
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }

    public float GetScreenBorder() {
        return screenBorders;
    }

    public float GetShootCooldown() {
        return shootCooldown;
    }

    public void AddHealth() {
        _controller.AddHealth();
    }

    private void OnRemoveHealth() {
        OnRemoveHealthEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PROJECTILE_TAG)) {
            _controller.RemoveHealth();
        }
    }

    protected override void OnDestroy() {
        _controller.OnRemoveHealthEvent -= OnRemoveHealth;
    }
}