using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEnemyManager {
    void SpawnEnemies(GameObject enemiesParent, Sprite redSprite, Sprite greenSprite, Sprite yellowSprite);
    void MakeRandomEnemyShoot();
    bool AnyEnemyAlive();
    IEnumerator MoveEnemies();
}

public class EnemyManager : IEnemyManager {
    private readonly IUpdateManager _updateManager;
    private readonly GameObject _enemyPrefab;

    private const int COL = 15;
    private const int ROW = 5;

    private const float START_X = -5f;
    private const float START_Y = 4f;
    private const float SPACING_X = 0.8f;
    private const float SPACING_Y = 0.8f;

    private float _moveDelay = 0.5f;
    private Vector2 _currentDirection = Vector2.right;
    private Sprite _currentSprite;

    private List<List<IEnemyView>> _enemies;

    public EnemyManager(IUpdateManager updateManager, GameObject enemyPrefab) {
        _updateManager = updateManager;
        _enemyPrefab = enemyPrefab;
    }

    public void SpawnEnemies(GameObject enemiesParent, Sprite redSprite, Sprite greenSprite, Sprite yellowSprite) {
        _enemies = new List<List<IEnemyView>>();
        for (int i = 0; i < ROW; i++) {
            _enemies.Add(new List<IEnemyView>());

            switch (i) {
                case 0:
                    _currentSprite = redSprite;
                    break;
                case 1:
                case 2:
                    _currentSprite = yellowSprite;
                    break;
                case 3:
                case 4:
                    _currentSprite = greenSprite;
                    break;
                default:
                    _currentSprite = redSprite;
                    Debug.LogError($"SPRITE NOT FOUND USING RED INSTEAD");
                    break;
            }

            for (int j = 0; j < COL; j++) {
                IEnemyView enemy = Object.Instantiate(_enemyPrefab, enemiesParent.transform).GetComponent<EnemyView>();
                _enemies[i].Add(enemy);

                float x = START_X + j * SPACING_X;
                float y = START_Y - i * SPACING_Y;
                enemy.Transform.position = new Vector3(x, y, 0f);
                enemy.SetSprite(_currentSprite);
            }
        }
    }

    public void MakeRandomEnemyShoot() {
        int col = Random.Range(0, COL);

        for (int row = ROW - 1; row >= 0; row--) {
            IEnemyView enemy = _enemies[row][col];
            if (enemy is { IsDead: false }) {
                enemy.Shoot();
                break;
            }
        }
    }

    public IEnumerator MoveEnemies() {
        while (AnyEnemyAlive()) {
            bool shouldChangeDirection = false;

            foreach (List<IEnemyView> enemyC in _enemies) {
                foreach (IEnemyView enemy in enemyC) {
                    if (enemy.WillHitBoundary(_currentDirection) && !enemy.IsDead) {
                        shouldChangeDirection = true;
                        break;
                    }
                }
            }

            if (shouldChangeDirection) {
                _currentDirection *= -1;
                for (int i = _enemies.Count - 1; i >= 0; i--) {
                    yield return new WaitForSeconds(0.05f);
                    foreach (IEnemyView enemy in _enemies[i]) {
                        enemy.Move(Vector2.down);
                    }
                }
            } else {
                for (int i = _enemies.Count - 1; i >= 0; i--) {
                    yield return new WaitForSeconds(0.05f);
                    foreach (IEnemyView enemy in _enemies[i]) {
                        enemy.Move(_currentDirection);
                    }
                }
            }

            yield return new WaitForSeconds(_moveDelay);
        }
    }

    public bool AnyEnemyAlive() {
        return _enemies.SelectMany(enemyC => enemyC).Any(enemy => !enemy.IsDead);
    }
}