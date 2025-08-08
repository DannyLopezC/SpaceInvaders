using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface for managing UI elements related to the game state.
/// </summary>
public interface IUIManager {
    /// <summary>
    /// Displays the player's hearts in the UI.
    /// </summary>
    /// <param name="hearts">Number of hearts to display.</param>
    void ActivateHeartsPanel(int hearts);
}

/// <summary>
/// Singleton UI manager responsible for displaying level information, 
/// game state messages (victory/defeat), countdown timers, and player health hearts.
/// </summary>
public class UIManager : Singleton<UIManager>, IUIManager {
    private ServiceLocator _serviceLocator;
    private IPlayerController _playerController;
    private IGameManager _gameManager => GameManager.Instance;

    [Header("UI Text Elements")] [SerializeField]
    private TMP_Text levelTMP; // Displays the current level number

    [SerializeField] private TMP_Text counterTMP; // Displays countdown before starting
    [SerializeField] private TMP_Text victoryTMP; // Displays victory message
    [SerializeField] private TMP_Text defeatTMP; // Displays defeat message
    [SerializeField] private TMP_Text titleTMP; // Displays the main title

    [Header("Buttons")] [SerializeField] private Button startButton; // Button to start the game
    [SerializeField] private Button restartButton; // Button to restart after loss/win
    [SerializeField] private Button quitButton; // Button to quit the game

    [Header("Hearts UI")] [SerializeField] private RectTransform heartsPanel; // Panel containing heart icons
    [SerializeField] private GameObject heartPrefab; // Prefab used for each heart

    private int _timer = 5; // Countdown timer before a level starts

    /// <summary>
    /// Sets up event subscriptions and retrieves necessary services.
    /// </summary>
    protected override void Awake() {
        _serviceLocator = ServiceLocator.Instance;
        _playerController = _serviceLocator.GetService<IPlayerController>();

        _gameManager.StartWaveEvent += InitLevel;
        _gameManager.PlayerLostEvent += PlayerLostEvent;
        _gameManager.PlayerWonEvent += PlayerWonEvent;
    }

    /// <summary>
    /// Displays victory UI when the player wins.
    /// </summary>
    private void PlayerWonEvent() {
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        victoryTMP.gameObject.SetActive(true);
        heartsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes the UI for a new level, hides menus, 
    /// shows the level text and starts the countdown.
    /// </summary>
    public void InitLevel() {
        heartsPanel.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        titleTMP.gameObject.SetActive(false);

        levelTMP.gameObject.SetActive(true);
        counterTMP.gameObject.SetActive(true);

        levelTMP.text = $"Level {_gameManager.GetCurrentLevel()}";
        StartCoroutine(Counter());
    }

    /// <inheritdoc/>
    public void ActivateHeartsPanel(int hearts) {
        // Remove existing hearts
        foreach (Transform child in heartsPanel.transform) {
            Destroy(child.gameObject);
        }

        heartsPanel.gameObject.SetActive(true);

        // Instantiate new hearts
        for (int i = 0; i < hearts; i++) {
            Instantiate(heartPrefab, heartsPanel);
        }
    }

    /// <summary>
    /// Coroutine for countdown before the wave starts.
    /// </summary>
    private IEnumerator Counter() {
        while (_timer > 0) {
            counterTMP.text = _timer.ToString();
            _timer--;
            yield return new WaitForSeconds(1);
        }

        _timer = 5;
        levelTMP.gameObject.SetActive(false);
        counterTMP.gameObject.SetActive(false);
        _gameManager.StartWave();
    }

    /// <summary>
    /// Resets the UI after restarting the game.
    /// </summary>
    public void RestartGame() {
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        defeatTMP.gameObject.SetActive(false);
        victoryTMP.gameObject.SetActive(false);

        _gameManager.RestartGame();
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void OnQuitButton() {
        Application.Quit();
    }

    /// <summary>
    /// Displays defeat UI when the player loses.
    /// </summary>
    private void PlayerLostEvent() {
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        defeatTMP.gameObject.SetActive(true);
        heartsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Unsubscribes from events when the object is destroyed.
    /// </summary>
    private void OnDestroy() {
        _gameManager.StartWaveEvent -= InitLevel;
        _gameManager.PlayerLostEvent -= PlayerLostEvent;
        _gameManager.PlayerWonEvent -= PlayerWonEvent;
    }
}