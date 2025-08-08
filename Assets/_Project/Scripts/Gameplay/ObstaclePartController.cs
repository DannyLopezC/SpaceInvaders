/// <summary>
/// Defines the behavior of an obstacle part's controller.
/// </summary>
public interface IObstaclePartController : IMonoBehaviourController {
    /// <summary>
    /// Destroys (disables) this part of the obstacle.
    /// </summary>
    void Destroy();
}

/// <summary>
/// Controller for a single part of an obstacle.  
/// Responsible for managing its active state.
/// </summary>
public class ObstaclePartController : MonoBehaviourController, IObstaclePartController {
    private readonly IObstaclePartView _view;

    /// <summary>
    /// Creates a new instance of the <see cref="ObstaclePartController"/>.
    /// </summary>
    /// <param name="view">The view associated with this obstacle part.</param>
    public ObstaclePartController(IObstaclePartView view) : base(view) {
        _view = view;
    }

    /// <inheritdoc/>
    public void Destroy() {
        _view.GameObject.SetActive(false);
    }
}