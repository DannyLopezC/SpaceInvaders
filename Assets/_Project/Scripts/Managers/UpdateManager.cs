using System;
using System.Collections;
using UnityEngine;

public interface IUpdateManager {
    event Action OnUpdate;
    event Action OnFixedUpdate;
    event Action OnLateUpdate;

    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine coroutine);
}

public class UpdateManager : MonoBehaviour, IUpdateManager {
    public event Action OnUpdate;
    public event Action OnFixedUpdate;
    public event Action OnLateUpdate;

    private void Awake() {
        if (ServiceLocator.Instance.GetService<IUpdateManager>() != null) {
            Destroy(gameObject);
            return;
        }

        ServiceLocator.Instance.AddFactory<IUpdateManager>((_) => this);

        DontDestroyOnLoad(gameObject);
    }

    Coroutine IUpdateManager.StartCoroutine(IEnumerator routine) {
        return StartCoroutine(routine);
    }

    void IUpdateManager.StopCoroutine(Coroutine coroutine) {
        StopCoroutine(coroutine);
    }

    private void Update() {
        OnUpdate?.Invoke();
    }

    private void FixedUpdate() {
        OnFixedUpdate?.Invoke();
    }

    private void LateUpdate() {
        OnLateUpdate?.Invoke();
    }
}