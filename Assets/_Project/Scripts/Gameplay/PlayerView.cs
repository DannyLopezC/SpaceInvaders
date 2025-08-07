using UnityEngine;
using UnityEngine.Serialization;

public interface IPlayerView : IMonoBehaviourView {
    float GetMoveSpeed();
    float GetScreenBorder();
    float GetShootCooldown();
}

public class PlayerView : MonoBehaviourView, IPlayerView {
    private IPlayerController _controller;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float screenBorders;

    [SerializeField] private float shootCooldown;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new PlayerController(this,
            serviceLocator.GetService<IInputHandler>(),
            serviceLocator.GetService<IProjectilePoolManager>(),
            serviceLocator.GetService<IUpdateManager>());
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
}