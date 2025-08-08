public interface IObstaclePartController : IMonoBehaviourController {
    void Destroy();
}

public class ObstaclePartController : MonoBehaviourController, IObstaclePartController {
    private readonly IObstaclePartView _view;

    public ObstaclePartController(IObstaclePartView view) : base(view) {
        _view = view;
    }

    public void Destroy() {
        _view.GameObject.SetActive(false);
    }
}