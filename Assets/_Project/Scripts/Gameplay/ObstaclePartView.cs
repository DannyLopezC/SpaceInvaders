using UnityEngine;

public interface IObstaclePartView : IMonoBehaviourView {
}

public class ObstaclePartView : MonoBehaviourView, IObstaclePartView {
    private IObstaclePartController _controller;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new ObstaclePartController(this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Projectile")) {
            _controller.Destroy();
        }
    }
}