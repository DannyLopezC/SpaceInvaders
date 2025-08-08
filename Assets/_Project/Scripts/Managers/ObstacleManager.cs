using UnityEngine;

/// <summary>
/// Interface for managing obstacle spawning and removal.
/// </summary>
public interface IObstacleManager {
    /// <summary>
    /// Spawns a set of obstacles under the given parent transform.
    /// </summary>
    /// <param name="parent">The transform that will hold the spawned obstacles.</param>
    void SpawnObstacles(Transform parent);

    /// <summary>
    /// Removes all obstacle instances under the given parent transform.
    /// </summary>
    /// <param name="parent">The transform containing the obstacles to clear.</param>
    void ClearObstacles(Transform parent);
}

/// <summary>
/// Handles the creation and removal of obstacles in the game world.
/// </summary>
public class ObstacleManager : IObstacleManager {
    private readonly GameObject _obstaclePrefab;
    private readonly int _count;
    private readonly float _spacing;

    /// <summary>
    /// Creates a new instance of the <see cref="ObstacleManager"/>.
    /// </summary>
    /// <param name="obstaclePrefab">The prefab to use when spawning obstacles.</param>
    /// <param name="count">The number of obstacles to spawn. Default is 5.</param>
    /// <param name="spacing">The distance between each obstacle. Default is 3f.</param>
    public ObstacleManager(GameObject obstaclePrefab, int count = 5, float spacing = 3f) {
        _obstaclePrefab = obstaclePrefab;
        _count = count;
        _spacing = spacing;
    }

    /// <inheritdoc/>
    public void SpawnObstacles(Transform parent) {
        // Remove any existing obstacles before spawning new ones
        ClearObstacles(parent);

        // Calculate horizontal placement
        float totalWidth = (_count - 1) * _spacing;
        float startX = -totalWidth / 2f;
        float y = -2.5f;

        // Instantiate obstacles evenly spaced along the X-axis
        for (int i = 0; i < _count; i++) {
            Vector3 pos = new Vector3(startX + i * _spacing, y, 0f);
            Object.Instantiate(_obstaclePrefab, pos, Quaternion.identity, parent);
        }
    }

    /// <inheritdoc/>
    public void ClearObstacles(Transform parent) {
        // Destroy all children under the parent transform
        for (int i = parent.childCount - 1; i >= 0; i--)
            Object.Destroy(parent.GetChild(i).gameObject);
    }
}