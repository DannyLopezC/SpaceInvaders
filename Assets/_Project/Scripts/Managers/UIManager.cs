using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IUIManager {
}

public class UIManager : Singleton<UIManager>, IUIManager {
    private IGameManager _gameManager => GameManager.Instance;

    [SerializeField] private TMP_Text levelTMP;
    [SerializeField] private TMP_Text counterTMP;
    [SerializeField] private TMP_Text victoryTMP;
    [SerializeField] private TMP_Text defeatTMP;
    [SerializeField] private TMP_Text titleTMP;

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private int _timer = 5;

    protected override void Awake() {
        _gameManager.StartWaveEvent += InitLevel;
        _gameManager.PlayerLostEvent += PlayerLostEvent;
        _gameManager.PlayerWonEvent += PlayerWonEvent;
    }

    private void PlayerWonEvent() {
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        victoryTMP.gameObject.SetActive(true);
    }

    public void InitLevel() {
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        titleTMP.gameObject.SetActive(false);

        levelTMP.gameObject.SetActive(true);
        counterTMP.gameObject.SetActive(true);

        levelTMP.text = $"Level {_gameManager.GetCurrentLevel()}";
        StartCoroutine(Counter());
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
    }

    private void OnDestroy() {
        _gameManager.StartWaveEvent -= InitLevel;
        _gameManager.PlayerLostEvent -= PlayerLostEvent;
        _gameManager.PlayerWonEvent -= PlayerWonEvent;
    }
}