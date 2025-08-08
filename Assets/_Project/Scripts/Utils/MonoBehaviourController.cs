/// <summary>
/// Defines a standard interface for MonoBehaviour-style controllers 
/// that handle Unity lifecycle events without directly inheriting from MonoBehaviour.
/// </summary>
public interface IMonoBehaviourController {
    /// <summary>
    /// Called during the MonoBehaviour's Awake phase.
    /// </summary>
    void OnAwake();

    /// <summary>
    /// Called during the MonoBehaviour's Start phase.
    /// </summary>
    void OnStart();

    /// <summary>
    /// Called every frame during the MonoBehaviour's Update phase.
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// Called during the MonoBehaviour's FixedUpdate phase (used for physics updates).
    /// </summary>
    void OnFixedUpdate();

    /// <summary>
    /// Called when the MonoBehaviour is being destroyed.
    /// </summary>
    void OnDestroy();

    /// <summary>
    /// Called when the MonoBehaviour is disabled.
    /// </summary>
    void OnDisable();
}

/// <summary>
/// Base implementation of <see cref="IMonoBehaviourController"/> 
/// that provides empty virtual methods for Unity's lifecycle events.
/// Designed to be extended by specific game controllers.
/// </summary>
public class MonoBehaviourController : IMonoBehaviourController {
    /// <summary>
    /// The view (MonoBehaviour) associated with this controller.
    /// </summary>
    protected readonly IMonoBehaviourView View;

    /// <summary>
    /// Creates a new MonoBehaviour controller bound to a specific view.
    /// </summary>
    /// <param name="view">The view associated with this controller.</param>
    public MonoBehaviourController(IMonoBehaviourView view) {
        View = view;
    }

    /// <inheritdoc/>
    public virtual void OnAwake() {
    }

    /// <inheritdoc/>
    public virtual void OnStart() {
    }

    /// <inheritdoc/>
    public virtual void OnUpdate() {
    }

    /// <inheritdoc/>
    public virtual void OnFixedUpdate() {
    }

    /// <inheritdoc/>
    public virtual void OnDestroy() {
    }

    /// <inheritdoc/>
    public virtual void OnDisable() {
    }
}