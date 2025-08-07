using UnityEngine;

public interface IMonoBehaviourView {
    public Transform Transform { get; }
    public GameObject GameObject { get; }
    public MonoBehaviour MonoBehaviour { get; }
}

public abstract class MonoBehaviourView : MonoBehaviour, IMonoBehaviourView {
    public Transform Transform => transform;
    public GameObject GameObject => gameObject;
    public MonoBehaviour MonoBehaviour => this;

    protected abstract IMonoBehaviourController Controller();

    protected virtual void Awake() {
        CreateController();
        Controller().OnAwake();
    }

    protected virtual void Start() {
        Controller().OnStart();
    }

    protected virtual void Update() {
        Controller().OnUpdate();
    }

    protected virtual void FixedUpdate() {
        Controller().OnFixedUpdate();
    }

    protected virtual void OnDestroy() {
        Controller().OnDestroy();
    }

    protected virtual void OnDisable() {
        Controller().OnDisable();
    }

    protected abstract void CreateController();
}