using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Defines the public interface for the game manager.
/// Manages game flow, levels, and win/lose events.
/// </summary>
public interface IGameManager {
    int GetCurrentLevel();
    void StartWave();
    event Action PlayerLostEvent;
    event Action PlayerWonEvent;
    event Action StartWaveEvent;
    bool GetIsPlaying();
    void RestartGame();
}

/// <summary>
/// Main controller for game state and progression.
/// Handles level management, win/lose conditions, enemy and obstacle spawning,
/// and coordinates core game events.
/// </summary>
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

    // Tracks current level and game state
    private int _currentLevel = 1;
    private bool _playing;

    // Enemy shooting timing (randomized between x and y)
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

    /// <summary>
    /// Loads essential audio clips into the sound bank.
    /// </summary>
    protected override void Awake() {
        SoundBank.Instance.AddAudioClip("Projectile", Resources.Load<AudioClip>("Music/Laser"));
        SoundBank.Instance.AddAudioClip("GameOver", Resources.Load<AudioClip>("Music/gameOver"));
        SoundBank.Instance.AddAudioClip("Explosion", Resources.Load<AudioClip>("Music/Explosion"));
        SoundBank.Instance.AddAudioClip("Win", Resources.Load<AudioClip>("Music/Win"));
    }

    /// <summary>
    /// Retrieves service dependencies and subscribes to player health events.
    /// </summary>
    protected void Start() {
        _serviceLocator = ServiceLocator.Instance;
        _enemyManager = _serviceLocator.GetService<IEnemyManager>();
        _playerController = _serviceLocator.GetService<IPlayerController>();
        _obstacleManager = _serviceLocator.GetService<IObstacleManager>();
        _inputHandler = _serviceLocator.GetService<IInputHandler>();

        _playerController.OnRemoveHealthEvent += OnPlayerRemoveHealth;
    }

    /// <summary>
    /// Coroutine that repeatedly triggers random enemy shooting while enemies are alive.
    /// </summary>
    private IEnumerator StartShooting() {
        yield return new WaitForSeconds(1);

        while (_enemyManager.AnyEnemyAlive()) {
            _enemyManager.MakeRandomEnemyShoot();
            yield return new WaitForSeconds(Random.Range(
                shootingTimeRange.x - enemyShootingTimeRangeDecrease,
                shootingTimeRange.y - enemyShootingTimeRangeDecrease
            ));
        }

        NextLevel();
    }

    /// <summary>
    /// Starts a new wave: spawns enemies and obstacles, enables UI, and begins shooting/movement coroutines.
    /// </summary>
    public void StartWave() {
        _playing = true;
        _playerController.ToggleView(true);

        _enemyManager.SpawnEnemies(enemiesParent, redSprite, greenSprite, yellowSprite);
        _shootingCoroutine = StartCoroutine(StartShooting());
        _movementCoroutine = StartCoroutine(_enemyManager.MoveEnemies());

        _uiManager.ActivateHeartsPanel(_playerController.GetHealth());
        _obstacleManager.SpawnObstacles(obstacleParent.transform);
    }

    /// <summary>
    /// Advances to the next level or ends the game if max level is reached.
    /// </summary>
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

    /// <summary>
    /// Called when the player loses health.
    /// Ends the game if health is zero, otherwise restarts the level.
    /// </summary>
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

    /// <summary>
    /// Restarts the current level (triggers StartWaveEvent).
    /// </summary>
    public void RestartLevel() {
        StopGame();
        OnStartWaveEvent();
    }

    /// <summary>
    /// Stops all gameplay: ends coroutines, clears enemies and obstacles.
    /// </summary>
    private void StopGame() {
        _playing = false;
        StopAllCoroutines();

        _obstacleManager.ClearObstacles(obstacleParent.transform);

        foreach (Transform child in enemiesParent.transform) {
            Destroy(child.gameObject);
        }
    }

    public int GetCurrentLevel() => _currentLevel;

    private void OnStartWaveEvent() => StartWaveEvent?.Invoke();
    private void OnPlayerLostEvent() => PlayerLostEvent?.Invoke();
    private void OnPlayerWonEvent() => PlayerWonEvent?.Invoke();

    public bool GetIsPlaying() => _playing;

    /// <summary>
    /// Resets the game back to level 1 and restarts.
    /// </summary>
    public void RestartGame() {
        _currentLevel = 1;
        _playerController.RestartHealth();
        RestartLevel();
    }

    /// <summary>
    /// Cleanup: removes event subscriptions, clears sounds, and removes input listeners.
    /// </summary>
    private void OnDestroy() {
        if (_playerController != null)
            _playerController.OnRemoveHealthEvent -= OnPlayerRemoveHealth;

        SoundBank.Instance.ClearDictionary();
        _inputHandler?.RemoveListeners();
    }
}