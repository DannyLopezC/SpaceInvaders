using UnityEngine;

public interface IObstacleManager {
    void SpawnObstacles(Transform parent);
    void ClearObstacles(Transform parent);
}

public class ObstacleManager : IObstacleManager {
    private readonly GameObject _obstaclePrefab;
    private readonly int _count;
    private readonly float _spacing;

    public ObstacleManager(GameObject obstaclePrefab, int count = 5, float spacing = 3f) {
        _obstaclePrefab = obstaclePrefab;
        _count = count;
        _spacing = spacing;
    }

    public void SpawnObstacles(Transform parent) {
        ClearObstacles(parent);

        float totalWidth = (_count - 1) * _spacing;
        float startX = -totalWidth / 2f;
        float y = -2.5f;

        for (int i = 0; i < _count; i++) {
            Vector3 pos = new Vector3(startX + i * _spacing, y, 0f);
            Object.Instantiate(_obstaclePrefab, pos, Quaternion.identity, parent);
        }
    }

    public void ClearObstacles(Transform parent) {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Object.Destroy(parent.GetChild(i).gameObject);
    }
}