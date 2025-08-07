public partial class ServiceLocator {
    private void RegisterFactories() {
        // Add more factory registrations here
        AddFactory<IGameManager>(_ => new GameManager());

        AddFactory(_ => new InputSystem_Actions());

        AddFactory<IInputHandler>(sl => new InputHandler(
            sl.GetService<InputSystem_Actions>()));
    }
}