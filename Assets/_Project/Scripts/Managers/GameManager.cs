using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager {
}

public class GameManager : Singleton<GameManager>, IGameManager {
    private ServiceLocator _serviceLocator;
    private IEnemyManager _enemyManager;

    [SerializeField] private GameObject enemiesParent;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    protected override void Awake() {
        _serviceLocator = ServiceLocator.Instance;
        _enemyManager = _serviceLocator.GetService<IEnemyManager>();
    }

    private IEnumerator StartShooting() {
        while (_enemyManager.AnyEnemyAlive()) {
            _enemyManager.MakeRandomEnemyShoot();
            yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
        }
    }

    private void StartWave() {
        _enemyManager.SpawnEnemies(enemiesParent, redSprite, greenSprite, yellowSprite);
        StartCoroutine(StartShooting());
        StartCoroutine(_enemyManager.MoveEnemies());
    }
}