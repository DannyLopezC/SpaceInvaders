using Unity.VisualScripting;
using UnityEngine;

public partial class ServiceLocator {
    /// <summary>
    /// Registers default service factories for the application.  
    /// This method is called from the constructor to ensure all core systems 
    /// are available when requested via <see cref="GetService{T}"/>.
    /// </summary>
    private void RegisterFactories() {
        // Projectile pool manager
        // Loads projectile prefabs from Resources and creates separate pools for player and enemy projectiles.
        AddFactory<IProjectilePoolManager>(_ => new ProjectilePoolManager(
            Resources.Load<ProjectileView>("PlayerProjectile"),
            Resources.Load<ProjectileView>("EnemyProjectile")));

        // Enemy manager
        // Manages enemy spawning and behavior. Requires UpdateManager and PlayerController services.
        AddFactory<IEnemyManager>(sl => new EnemyManager(
            sl.GetService<IUpdateManager>(),
            Resources.Load<GameObject>("Enemy"),
            sl.GetService<IPlayerController>()));

        // Obstacle manager
        // Manages spawning and handling of obstacles in the game world.
        AddFactory<IObstacleManager>(_ => new ObstacleManager(
            Resources.Load<GameObject>("Obstacle")));

        // Input system actions
        // Generates an instance of Unity's new Input System actions asset.
        AddFactory(_ => new InputSystem_Actions());

        // Input handler
        // Processes input events and translates them into game actions.
        AddFactory<IInputHandler>(sl => new InputHandler(
            sl.GetService<InputSystem_Actions>()));
    }
}