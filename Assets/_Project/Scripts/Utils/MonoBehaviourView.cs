using UnityEngine;

/// <summary>
/// Defines the public properties that any MonoBehaviour-based view should expose
/// to allow controllers to interact with the Unity object without directly referencing MonoBehaviour.
/// </summary>
public interface IMonoBehaviourView {
    /// <summary>
    /// Gets the Transform component of this view.
    /// </summary>
    Transform Transform { get; }

    /// <summary>
    /// Gets the GameObject associated with this view.
    /// </summary>
    GameObject GameObject { get; }

    /// <summary>
    /// Gets the MonoBehaviour instance for this view.
    /// </summary>
    MonoBehaviour MonoBehaviour { get; }
}

/// <summary>
/// Base abstract class for all view components in the MVC-like architecture.
/// It connects Unity's MonoBehaviour lifecycle to an <see cref="IMonoBehaviourController"/>.
/// </summary>
public abstract class MonoBehaviourView : MonoBehaviour, IMonoBehaviourView {
    /// <inheritdoc/>
    public Transform Transform => transform;

    /// <inheritdoc/>
    public GameObject GameObject => gameObject;

    /// <inheritdoc/>
    public MonoBehaviour MonoBehaviour => this;

    /// <summary>
    /// Provides access to the controller instance associated with this view.
    /// Must be implemented in subclasses to return the correct controller type.
    /// </summary>
    /// <returns>The controller that manages this view.</returns>
    protected abstract IMonoBehaviourController Controller();

    /// <summary>
    /// Called by Unity when the object is created.
    /// Creates the controller and invokes its <see cref="IMonoBehaviourController.OnAwake"/> method.
    /// </summary>
    protected virtual void Awake() {
        CreateController();
        Controller().OnAwake();
    }

    /// <summary>
    /// Called by Unity before the first frame update.
    /// Invokes the controller's <see cref="IMonoBehaviourController.OnStart"/> method.
    /// </summary>
    protected virtual void Start() {
        Controller().OnStart();
    }

    /// <summary>
    /// Called by Unity once per frame.
    /// Invokes the controller's <see cref="IMonoBehaviourController.OnUpdate"/> method.
    /// </summary>
    protected virtual void Update() {
        Controller().OnUpdate();
    }

    /// <summary>
    /// Called by Unity at fixed time intervals.
    /// Invokes the controller's <see cref="IMonoBehaviourController.OnFixedUpdate"/> method.
    /// </summary>
    protected virtual void FixedUpdate() {
        Controller().OnFixedUpdate();
    }

    /// <summary>
    /// Called by Unity when the object is being destroyed.
    /// Invokes the controller's <see cref="IMonoBehaviourController.OnDestroy"/> method.
    /// </summary>
    protected virtual void OnDestroy() {
        Controller().OnDestroy();
    }

    /// <summary>
    /// Called by Unity when the object becomes disabled or inactive.
    /// Invokes the controller's <see cref="IMonoBehaviourController.OnDisable"/> method.
    /// </summary>
    protected virtual void OnDisable() {
        Controller().OnDisable();
    }

    /// <summary>
    /// Creates and assigns the controller for this view.
    /// Must be implemented in subclasses to instantiate the proper controller type.
    /// </summary>
    protected abstract void CreateController();
}