using UnityEngine;

public partial class ServiceLocator {
    private void RegisterFactories() {
        // Add more factory registrations here
        AddFactory<IGameManager>(_ => new GameManager());
        AddFactory<IProjectilePoolManager>(_ => new ProjectilePoolManager(
            Resources.Load<ProjectileView>("Projectile"),
            Resources.Load<ProjectileView>("Projectile")));

        AddFactory(_ => new InputSystem_Actions());

        AddFactory<IInputHandler>(sl => new InputHandler(
            sl.GetService<InputSystem_Actions>()));
    }
}