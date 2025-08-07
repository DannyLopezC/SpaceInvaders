using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T _instance;

    public static T Instance {
        get {
            if (_instance != null) return _instance;
            Instance = FindObjectOfType<T>();

            if (_instance != null) return _instance;
            GameObject obj = new() {
                name = typeof(T).Name
            };
            _instance = obj.AddComponent<T>();

            return _instance;
        }

        private set => _instance = value;
    }

    public static bool HasInstance => _instance != null;

    protected virtual void Awake() {
        CreateSingleton();
    }

    private void CreateSingleton() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}