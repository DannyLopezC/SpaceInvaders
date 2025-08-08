using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileController : IMonoBehaviourController {
    void StartMoving(Vector2 direction);
    void OnProjectileCollided();
}

public class ProjectileController : MonoBehaviourController, IProjectileController {
    private readonly IProjectileView _view;
    private readonly IUpdateManager _updateManager;
    private bool _isMoving;
    private Vector3 _moveDirection;
    private Coroutine _destroyCoroutine;

    public ProjectileController(IProjectileView view, IUpdateManager updateManager) : base(view) {
        _view = view;
        _updateManager = updateManager;
    }

    public override void OnUpdate() {
        if (_isMoving) Move();
    }

    public void StartMoving(Vector2 direction) {
        _moveDirection = new Vector3(0f, direction.y, 0f);
        _isMoving = true;
        _destroyCoroutine = _updateManager.StartCoroutine(DisableProjectile());
    }

    private IEnumerator DisableProjectile() {
        yield return new WaitForSeconds(_view.GetProjectileLifeTime());
        _view.GameObject.SetActive(false);
    }

    public void OnProjectileCollided() {
        _isMoving = false;
        // _updateManager.StartCoroutine(ProjectileExplosion());
        _view.GameObject.SetActive(false);
        if (_destroyCoroutine != null) _updateManager.StopCoroutine(_destroyCoroutine);
        _destroyCoroutine = null;
    }

    private IEnumerator ProjectileExplosion() {
        GameObject explosionInstance = Object.Instantiate(_view.GetExplosionPrefab());
        explosionInstance.transform.position = _view.GameObject.transform.position;
        explosionInstance.transform.localScale = Vector3.one * 0.4f;

        yield return new WaitForSeconds(2);
        Object.Destroy(explosionInstance);
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