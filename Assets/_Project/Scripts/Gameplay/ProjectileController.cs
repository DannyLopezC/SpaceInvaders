using UnityEngine;

public interface IProjectileController : IMonoBehaviourController {
    void StartMoving(Vector2 direction);
}

public class ProjectileController : MonoBehaviourController, IProjectileController {
    private readonly IProjectileView _view;
    private bool _isMoving;
    private Vector3 _moveDirection;

    public ProjectileController(IProjectileView view) : base(view) {
        _view = view;
    }

    public override void OnUpdate() {
        if (_isMoving) Move();
    }

    public void StartMoving(Vector2 direction) {
        _moveDirection = new Vector3(0f, direction.y, 0f);
        _isMoving = true;
    }

    private void Move() {
        Vector3 position = _view.Transform.position;
        position += _moveDirection * _view.GetProjectileSpeed() * Time.deltaTime;
        _view.Transform.position = position;
    }

    public override void OnDisable() {
        _isMoving = false;
    }
}