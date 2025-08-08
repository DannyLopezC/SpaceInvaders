using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IUIManager {
    void ActivateHeartsPanel(int hearts);
}

public class UIManager : Singleton<UIManager>, IUIManager {
    private ServiceLocator _serviceLocator;
    private IPlayerController _playerController;
    private IGameManager _gameManager => GameManager.Instance;

    [SerializeField] private TMP_Text levelTMP;
    [SerializeField] private TMP_Text counterTMP;
    [SerializeField] private TMP_Text victoryTMP;
    [SerializeField] private TMP_Text defeatTMP;
    [SerializeField] private TMP_Text titleTMP;

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private RectTransform heartsPanel;
    [SerializeField] private GameObject heartPrefab;

    private int _timer = 5;

    protected override void Awake() {
        _serviceLocator = ServiceLocator.Instance;
        _playerController = _serviceLocator.GetService<IPlayerController>();

        _gameManager.StartWaveEvent += InitLevel;
        _gameManager.PlayerLostEvent += PlayerLostEvent;
        _gameManager.PlayerWonEvent += PlayerWonEvent;
    }

    private void PlayerWonEvent() {
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        victoryTMP.gameObject.SetActive(true);

        heartsPanel.gameObject.SetActive(false);
    }

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

    public void ActivateHeartsPanel(int hearts) {
        foreach (Transform child in heartsPanel.transform) {
            Destroy(child.gameObject);
        }

        heartsPanel.gameObject.SetActive(true);
        for (int i = 0; i < hearts; i++) {
            Instantiate(heartPrefab, heartsPanel);
        }
    }

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

    public void RestartGame() {
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        defeatTMP.gameObject.SetActive(false);
        victoryTMP.gameObject.SetActive(false);

        _gameManager.RestartGame();
    }

    public void OnQuitButton() {
        Application.Quit();
    }

    private void PlayerLostEvent() {
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        defeatTMP.gameObject.SetActive(true);

        heartsPanel.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        _gameManager.StartWaveEvent -= InitLevel;
        _gameManager.PlayerLostEvent -= PlayerLostEvent;
        _gameManager.PlayerWonEvent -= PlayerWonEvent;
    }
}