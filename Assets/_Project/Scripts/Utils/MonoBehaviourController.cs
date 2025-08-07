public interface IMonoBehaviourController {
    void OnAwake();
    void OnStart();
    void OnUpdate();
    void OnFixedUpdate();
    void OnDestroy();
    void OnDisable();
}

public class MonoBehaviourController : IMonoBehaviourController {
    protected readonly IMonoBehaviourView View;

    public MonoBehaviourController(IMonoBehaviourView view) {
        View = view;
    }

    public virtual void OnAwake() {
    }

    public virtual void OnStart() {
    }

    public virtual void OnUpdate() {
    }

    public virtual void OnFixedUpdate() {
    }

    public virtual void OnDestroy() {
    }

    public virtual void OnDisable() {
    }
}