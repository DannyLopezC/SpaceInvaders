using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Interface for managing centralized Unity update loops (Update, FixedUpdate, LateUpdate).
/// Also exposes coroutine management to non-MonoBehaviour classes.
/// </summary>
public interface IUpdateManager {
    /// <summary>
    /// Event invoked once per frame during Unity's Update loop.
    /// </summary>
    event Action OnUpdate;

    /// <summary>
    /// Event invoked on Unity's FixedUpdate cycle (used for physics updates).
    /// </summary>
    event Action OnFixedUpdate;

    /// <summary>
    /// Event invoked once per frame after all Update calls (LateUpdate).
    /// </summary>
    event Action OnLateUpdate;

    /// <summary>
    /// Starts a coroutine from outside a MonoBehaviour.
    /// </summary>
    Coroutine StartCoroutine(IEnumerator routine);

    /// <summary>
    /// Stops a coroutine previously started by the update manager.
    /// </summary>
    void StopCoroutine(Coroutine coroutine);
}

/// <summary>
/// Centralized MonoBehaviour that provides global access to Unity's update cycles and coroutine execution.
/// Allows non-MonoBehaviour classes to subscribe to update events or start coroutines.
/// Implemented as a singleton service registered via a ServiceLocator.
/// </summary>
public class UpdateManager : MonoBehaviour, IUpdateManager {
    public event Action OnUpdate;
    public event Action OnFixedUpdate;
    public event Action OnLateUpdate;

    /// <summary>
    /// Ensures a single instance of UpdateManager exists and registers it in the ServiceLocator.
    /// Persists between scenes.
    /// </summary>
    private void Awake() {
        // Prevent duplicate instances if another UpdateManager is already registered
        if (ServiceLocator.Instance.GetService<IUpdateManager>() != null) {
            Destroy(gameObject);
            return;
        }

        // Register this instance as the global IUpdateManager
        ServiceLocator.Instance.AddFactory<IUpdateManager>((_) => this);

        // Keep this object alive across scene loads
        DontDestroyOnLoad(gameObject);
    }

    /// <inheritdoc/>
    Coroutine IUpdateManager.StartCoroutine(IEnumerator routine) {
        return StartCoroutine(routine);
    }

    /// <inheritdoc/>
    void IUpdateManager.StopCoroutine(Coroutine coroutine) {
        StopCoroutine(coroutine);
    }

    /// <summary>
    /// Unity's Update method — invokes any subscribed OnUpdate handlers.
    /// </summary>
    private void Update() {
        OnUpdate?.Invoke();
    }

    /// <summary>
    /// Unity's FixedUpdate method — invokes any subscribed OnFixedUpdate handlers.
    /// </summary>
    private void FixedUpdate() {
        OnFixedUpdate?.Invoke();
    }

    /// <summary>
    /// Unity's LateUpdate method — invokes any subscribed OnLateUpdate handlers.
    /// </summary>
    private void LateUpdate() {
        OnLateUpdate?.Invoke();
    }
}