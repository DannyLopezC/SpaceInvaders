using UnityEngine;

/// <summary>
/// View interface for a single obstacle part.
/// </summary>
public interface IObstaclePartView : IMonoBehaviourView {
}

/// <summary>
/// Handles the visual and collision behavior of an individual obstacle part.
/// </summary>
public class ObstaclePartView : MonoBehaviourView, IObstaclePartView {
    private IObstaclePartController _controller;

    /// <summary>
    /// Returns the associated controller for this obstacle part.
    /// </summary>
    protected override IMonoBehaviourController Controller() {
        return _controller;
    }

    /// <summary>
    /// Creates the controller instance for this obstacle part using the service locator.
    /// </summary>
    protected override void CreateController() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        // Currently no external dependencies are required, so only "this" view is passed
        _controller = new ObstaclePartController(this);
    }

    /// <summary>
    /// Detects collisions and triggers destruction when hit by a projectile.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other) {
        // If the colliding object is a projectile, destroy this obstacle part
        if (other.CompareTag("Projectile")) {
            _controller.Destroy();
        }
    }
}