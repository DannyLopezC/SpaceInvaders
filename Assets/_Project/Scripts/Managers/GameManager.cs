using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IGameManager {
    int GetCurrentLevel();
    void StartWave();
    event Action PlayerLostEvent;
    event Action PlayerWonEvent;
    event Action StartWaveEvent;
    bool GetIsPlaying();
    void RestartGame();
}

public class GameManager : Singleton<GameManager>, IGameManager {
    private ServiceLocator _serviceLocator;
    private IEnemyManager _enemyManager;
    private IPlayerController _playerController;
    private IUIManager _uiManager => UIManager.Instance;
    private IObstacleManager _obstacleManager;
    private IInputHandler _inputHandler;

    public event Action PlayerWonEvent;
    public event Action StartWaveEvent;
    public event Action PlayerLostEvent;

    private int _currentLevel = 10;
    private bool _playing;
    private Vector2 shootingTimeRange = new Vector2(1f, 1.3f);

    [SerializeField] private float enemyShootingTimeRangeDecrease;
    [SerializeField] private GameObject enemiesParent;
    [SerializeField] private GameObject obstacleParent;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    [SerializeField] private Material backgroundMaterial;

    private Coroutine _shootingCoroutine;
    private Coroutine _movementCoroutine;

    protected override void Awake() {
        SoundBank.Instance.AddAudioClip("Projectile", Resources.Load<AudioClip>("Music/Laser"));
        SoundBank.Instance.AddAudioClip("GameOver", Resources.Load<AudioClip>("Music/gameOver"));
        SoundBank.Instance.AddAudioClip("Explosion", Resources.Load<AudioClip>("Music/Explosion"));
        SoundBank.Instance.AddAudioClip("Win", Resources.Load<AudioClip>("Music/Win"));
    }

    protected void Start() {
        _serviceLocator = ServiceLocator.Instance;
        _enemyManager = _serviceLocator.GetService<IEnemyManager>();
        _playerController = _serviceLocator.GetService<IPlayerController>();
        _obstacleManager = _serviceLocator.GetService<IObstacleManager>();
        _inputHandler = _serviceLocator.GetService<IInputHandler>();
        _playerController.OnRemoveHealthEvent += OnPlayerRemoveHealth;
    }

    private IEnumerator StartShooting() {
        yield return new WaitForSeconds(1);

        while (_enemyManager.AnyEnemyAlive()) {
            _enemyManager.MakeRandomEnemyShoot();
            yield return new WaitForSeconds(Random.Range(shootingTimeRange.x - enemyShootingTimeRangeDecrease,
                shootingTimeRange.y - enemyShootingTimeRangeDecrease));
        }

        NextLevel();
    }

    public void StartWave() {
        _playing = true;
        _playerController.ToggleView(true);
        _enemyManager.SpawnEnemies(enemiesParent, redSprite, greenSprite, yellowSprite);
        _shootingCoroutine = StartCoroutine(StartShooting());
        _movementCoroutine = StartCoroutine(_enemyManager.MoveEnemies());
        _uiManager.ActivateHeartsPanel(_playerController.GetHealth());
        _obstacleManager.SpawnObstacles(obstacleParent.transform);
    }

    private void NextLevel() {
        if (!_playing) return;
        SoundManager.Instance.PlaySoundEffect("Win", 0.8f);
        _currentLevel++;
        if (_currentLevel > 10) {
            StopGame();
            OnPlayerWonEvent();
            return;
        }

        _playerController.AddHealth();
        RestartLevel();
    }

    private void OnPlayerRemoveHealth() {
        if (!_playing) return;
        SoundManager.Instance.PlaySoundEffect("GameOver", 0.4f);
        if (_playerController.GetHealth() <= 0) {
            StopGame();
            OnPlayerLostEvent();
            return;
        }

        RestartLevel();
    }

    public void RestartLevel() {
        StopGame();
        OnStartWaveEvent();
    }

    private void StopGame() {
        _playing = false;
        StopAllCoroutines();
        _obstacleManager.ClearObstacles(obstacleParent.transform);

        foreach (Transform child in enemiesParent.transform) {
            Destroy(child.gameObject);
        }
    }

    public int GetCurrentLevel() {
        return _currentLevel;
    }

    private void OnStartWaveEvent() {
        StartWaveEvent?.Invoke();
    }

    private void OnPlayerLostEvent() {
        PlayerLostEvent?.Invoke();
    }

    private void OnPlayerWonEvent() {
        PlayerWonEvent?.Invoke();
    }

    public bool GetIsPlaying() {
        return _playing;
    }

    public void RestartGame() {
        _currentLevel = 1;
        _playerController.RestartHealth();
        RestartLevel();
    }

    private void OnDestroy() {
        if (_playerController != null) _playerController.OnRemoveHealthEvent -= OnPlayerRemoveHealth;
        SoundBank.Instance.ClearDictionary();
        _inputHandler?.RemoveListeners();
    }
}