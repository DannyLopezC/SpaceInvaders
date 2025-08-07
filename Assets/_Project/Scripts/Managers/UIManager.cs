using System;
using System.Collections;
using TMPro;
using UnityEngine;

public interface IUIManager {
}

public class UIManager : Singleton<UIManager>, IUIManager {
    private IGameManager _gameManager => GameManager.Instance;

    [SerializeField] private TMP_Text levelTMP;
    [SerializeField] private TMP_Text counterTMP;
    private int _timer = 5;

    protected override void Awake() {
        _gameManager.StartWaveEvent += InitLevel;
    }

    public void InitLevel() {
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

    public void OnQuitButton() {
        Application.Quit();
    }

    private void OnDestroy() {
        _gameManager.StartWaveEvent -= InitLevel;
    }
}