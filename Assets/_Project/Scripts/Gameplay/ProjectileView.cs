using UnityEngine;

public interface IProjectileView : IMonoBehaviourView {
    void StartMoving(Vector2 direction);
    float GetProjectileSpeed();
}

public class ProjectileView : MonoBehaviourView, IProjectileView {
    private IProjectileController _controller;
    [SerializeField] private float projectileSpeed;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new ProjectileController(this);
    }

    public void StartMoving(Vector2 direction) {
        _controller.StartMoving(direction);
    }

    public float GetProjectileSpeed() {
        return projectileSpeed;
    }
}