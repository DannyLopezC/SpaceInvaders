using UnityEngine;
using UnityEngine.Serialization;

public interface IPlayerView : IMonoBehaviourView {
    float GetMoveSpeed();
    float GetScreenBorder();
}

public class PlayerView : MonoBehaviourView, IPlayerView {
    private IPlayerController _controller;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float screenBorders;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new PlayerController(this, serviceLocator.GetService<IInputHandler>());
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }

    public float GetScreenBorder() {
        return screenBorders;
    }
}