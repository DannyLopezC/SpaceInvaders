using UnityEngine;

public class ServiceLocatorInstantiator : MonoBehaviour {
    private void Awake() {
        if (ServiceLocator.Instance == null) {
            // This does nothing LOL.
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}