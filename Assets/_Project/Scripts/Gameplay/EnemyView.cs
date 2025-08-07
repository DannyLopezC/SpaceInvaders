using System;
using UnityEngine;

public interface IEnemyView : IMonoBehaviourView {
}

public class EnemyView : MonoBehaviourView, IEnemyView {
    private IEnemyController _controller;

    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        _controller = new EnemyController(this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Projectile")) {
            _controller.Die();
        }
    }
}