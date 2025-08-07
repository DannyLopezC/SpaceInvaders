public interface IEnemyController : IMonoBehaviourController {
}

public class EnemyController : MonoBehaviourController, IEnemyController {
    private readonly IEnemyView _view;

    public EnemyController(IEnemyView view) : base(view) {
        _view = view;
    }
}