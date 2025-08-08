using Unity.VisualScripting;
using UnityEngine;

public partial class ServiceLocator {
    private void RegisterFactories() {
        // Add more factory registrations here
        AddFactory<IProjectilePoolManager>(_ => new ProjectilePoolManager(
            Resources.Load<ProjectileView>("PlayerProjectile"),
            Resources.Load<ProjectileView>("EnemyProjectile")));

        AddFactory<IEnemyManager>(sl => new EnemyManager(sl.GetService<IUpdateManager>(),
            Resources.Load<GameObject>("Enemy"), sl.GetService<IPlayerController>()));

        AddFactory<IObstacleManager>(sl => new ObstacleManager(Resources.Load<GameObject>("Obstacle")));

        AddFactory(_ => new InputSystem_Actions());

        AddFactory<IInputHandler>(sl => new InputHandler(
            sl.GetService<InputSystem_Actions>()));
    }
}