using UnityEngine;

/// <summary>
/// Ensures that the ServiceLocator singleton instance persists across scenes.
/// If the ServiceLocator instance is not initialized, this script will do nothing.
/// </summary>
public class ServiceLocatorInstantiator : MonoBehaviour {
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Checks if the ServiceLocator instance exists; if so, prevents this GameObject from being destroyed on scene load.
    /// </summary>
    private void Awake() {
        if (ServiceLocator.Instance == null) {
            // No ServiceLocator instance found, so exit early.
            return;
        }

        // Make this GameObject persist across scene loads.
        DontDestroyOnLoad(gameObject);
    }
}