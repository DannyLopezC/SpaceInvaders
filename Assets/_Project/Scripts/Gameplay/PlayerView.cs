using System;
using UnityEngine;
using UnityEngine.Serialization;

public interface IPlayerView : IMonoBehaviourView {
    float GetMoveSpeed();
    float GetScreenBorder();
    float GetShootCooldown();
    void AddHealth();
}

public class PlayerView : MonoBehaviourView, IPlayerView {
    private IPlayerController _controller;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float screenBorders;

    [SerializeField] private float shootCooldown;

    private const string PROJECTILE_TAG = "Projectile";

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new PlayerController(this,
            serviceLocator.GetService<IInputHandler>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            serviceLocator.GetService<IUpdateManager>(), GameManager.Instance);

        serviceLocator.AddFactory<IPlayerController>(_ => _controller);
        gameObject.SetActive(false);
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

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(PROJECTILE_TAG)) {
            _controller.RemoveHealth();
        }
    }
}