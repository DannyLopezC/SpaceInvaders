using UnityEngine;

/// <summary>
/// Generic singleton base class for MonoBehaviour components.
/// Ensures only one instance of the specified type exists and persists across scenes.
/// </summary>
/// <typeparam name="T">Type of the singleton component.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component {
    private static T _instance;

    /// <summary>
    /// Gets the singleton instance of type T.
    /// If none exists in the scene, it tries to find one or creates a new GameObject with the component attached.
    /// </summary>
    public static T Instance {
        get {
            if (_instance != null) return _instance;

            // Attempt to find existing instance in the scene
            Instance = FindObjectOfType<T>();

            if (_instance != null) return _instance;

            // No instance found, create new GameObject and attach component T
            GameObject obj = new() {
                name = typeof(T).Name
            };
            _instance = obj.AddComponent<T>();

            return _instance;
        }
        private set => _instance = value;
    }

    /// <summary>
    /// Returns true if the singleton instance exists.
    /// </summary>
    public static bool HasInstance => _instance != null;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Ensures that the singleton is created and duplicates are destroyed.
    /// </summary>
    protected virtual void Awake() {
        CreateSingleton();
    }

    /// <summary>
    /// Creates the singleton instance or destroys the GameObject if another instance already exists.
    /// Also marks the singleton to not be destroyed on scene load.
    /// </summary>
    private void CreateSingleton() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}