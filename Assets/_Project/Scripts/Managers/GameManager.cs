using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IGameManager {
    int GetCurrentLevel();
    void StartWave();
    event Action StartWaveEvent;
}

public class GameManager : Singleton<GameManager>, IGameManager {
    private ServiceLocator _serviceLocator;
    private IEnemyManager _enemyManager;
    private IPlayerView _playerView;
    public event Action StartWaveEvent;

    private int _currentLevel = 1;

    [SerializeField] private GameObject enemiesParent;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    private Coroutine shootingCoroutine;
    private Coroutine movementCoroutine;

    protected override void Awake() {
        _serviceLocator = ServiceLocator.Instance;
        _enemyManager = _serviceLocator.GetService<IEnemyManager>();
        _playerView = _serviceLocator.GetService<IPlayerView>();

        _playerView.OnRemoveHealthEvent += RestartLevel;
    }

    private IEnumerator StartShooting() {
        while (_enemyManager.AnyEnemyAlive()) {
            _enemyManager.MakeRandomEnemyShoot();
            yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
        }

        NextLevel();
    }

    public void StartWave() {
        _enemyManager.SpawnEnemies(enemiesParent, redSprite, greenSprite, yellowSprite);
        shootingCoroutine = StartCoroutine(StartShooting());
        movementCoroutine = StartCoroutine(_enemyManager.MoveEnemies());
    }

    private void NextLevel() {
        _currentLevel++;
        RestartLevel();
    }

    private void RestartLevel() {
        StopCoroutine(shootingCoroutine);
        StopCoroutine(movementCoroutine);

        StartWaveEvent?.Invoke();
    }

    public int GetCurrentLevel() {
        return _currentLevel;
    }

    public void OnStartWaveEvent() {
        StartWaveEvent?.Invoke();
    }
}