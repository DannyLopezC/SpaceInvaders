public interface IEnemyController : IMonoBehaviourController {
    void Die();
}

public class EnemyController : MonoBehaviourController, IEnemyController {
    private readonly IEnemyView _view;

    public EnemyController(IEnemyView view) : base(view) {
        _view = view;
    }

    public void Die() {
        _view.GameObject.SetActive(false);
    }
}