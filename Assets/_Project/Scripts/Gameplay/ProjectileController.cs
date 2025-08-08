using System.Collections;
using UnityEngine;

/// <summary>
/// Defines the public interface for projectile controllers.
/// Responsible for starting projectile movement and handling collisions.
/// </summary>
public interface IProjectileController : IMonoBehaviourController {
    /// <summary>
    /// Starts moving the projectile in the given direction.
    /// </summary>
    /// <param name="direction">Direction vector for movement (only Y-axis is used).</param>
    void StartMoving(Vector2 direction);

    /// <summary>
    /// Handles the logic when the projectile collides with something.
    /// Stops movement and deactivates the projectile.
    /// </summary>
    void OnProjectileCollided();
}

/// <summary>
/// Controls projectile behavior, including movement, lifetime, and collision handling.
/// </summary>
public class ProjectileController : MonoBehaviourController, IProjectileController {
    private readonly IProjectileView _view; // View interface to access projectile properties
    private readonly IUpdateManager _updateManager; // Handles coroutines outside of MonoBehaviour
    private bool _isMoving; // Whether the projectile is currently moving
    private Vector3 _moveDirection; // Movement direction
    private Coroutine _destroyCoroutine; // Coroutine reference for automatic deactivation

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectileController"/>.
    /// </summary>
    public ProjectileController(IProjectileView view, IUpdateManager updateManager) : base(view) {
        _view = view;
        _updateManager = updateManager;
    }

    /// <summary>
    /// Called every frame. Moves the projectile if it is active.
    /// </summary>
    public override void OnUpdate() {
        if (_isMoving) Move();
    }

    /// <inheritdoc/>
    public void StartMoving(Vector2 direction) {
        // We only care about vertical movement (Y-axis)
        _moveDirection = new Vector3(0f, direction.y, 0f);
        _isMoving = true;

        // Schedule projectile to be disabled after its lifetime expires
        _destroyCoroutine = _updateManager.StartCoroutine(DisableProjectile());
    }

    /// <summary>
    /// Disables the projectile after its configured lifetime.
    /// </summary>
    private IEnumerator DisableProjectile() {
        yield return new WaitForSeconds(_view.GetProjectileLifeTime());
        _view.GameObject.SetActive(false);
    }

    /// <inheritdoc/>
    public void OnProjectileCollided() {
        _isMoving = false;

        // Optional explosion effect could be triggered here
        // _updateManager.StartCoroutine(ProjectileExplosion());

        _view.GameObject.SetActive(false);

        // Stop lifetime coroutine if still running
        if (_destroyCoroutine != null)
            _updateManager.StopCoroutine(_destroyCoroutine);

        _destroyCoroutine = null;
    }

    /// <summary>
    /// Spawns an explosion effect at the projectile's position, then destroys it.
    /// </summary>
    private IEnumerator ProjectileExplosion() {
        GameObject explosionInstance = Object.Instantiate(_view.GetExplosionPrefab());
        explosionInstance.transform.position = _view.GameObject.transform.position;
        explosionInstance.transform.localScale = Vector3.one * 0.4f;

        yield return new WaitForSeconds(2);
        Object.Destroy(explosionInstance);
    }

    /// <summary>
    /// Moves the projectile each frame according to its speed and direction.
    /// </summary>
    private void Move() {
        Vector3 position = _view.Transform.position;
        position += _moveDirection * _view.GetProjectileSpeed() * Time.deltaTime;
        _view.Transform.position = position;
    }

    /// <summary>
    /// Called when the projectile is disabled. Resets its movement state.
    /// </summary>
    public override void OnDisable() {
        _isMoving = false;
    }
}