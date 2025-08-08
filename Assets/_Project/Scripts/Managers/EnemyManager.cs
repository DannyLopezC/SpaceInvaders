using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Interface defining the enemy management behavior.
/// Responsible for spawning, moving, and controlling enemies in the game.
/// </summary>
public interface IEnemyManager {
    /// <summary>
    /// Spawns the grid of enemies with specified sprites for each row.
    /// </summary>
    /// <param name="enemiesParent">Parent GameObject under which enemies are instantiated.</param>
    /// <param name="redSprite">Sprite for the top row (strongest enemies).</param>
    /// <param name="greenSprite">Sprite for the bottom rows.</param>
    /// <param name="yellowSprite">Sprite for the middle rows.</param>
    void SpawnEnemies(GameObject enemiesParent, Sprite redSprite, Sprite greenSprite, Sprite yellowSprite);

    /// <summary>
    /// Selects a random column and makes the bottom-most alive enemy in that column shoot.
    /// </summary>
    void MakeRandomEnemyShoot();

    /// <summary>
    /// Checks if there is at least one enemy still alive.
    /// </summary>
    bool AnyEnemyAlive();

    /// <summary>
    /// Coroutine that moves all enemies horizontally and vertically according to Space Invaders-style logic.
    /// </summary>
    IEnumerator MoveEnemies();

    /// <summary>
    /// Triggers the player being hit (removes health).
    /// </summary>
    void KillPlayer();
}

/// <summary>
/// Handles creation, movement, and attack logic for a grid of enemies.
/// Implements Space Invaders-style movement (side-to-side, downwards on wall hit).
/// </summary>
public class EnemyManager : IEnemyManager {
    private readonly IUpdateManager _updateManager;
    private readonly IPlayerController _playerController;
    private readonly GameObject _enemyPrefab;

    // Grid configuration
    private const int COL = 12;
    private const int ROW = 5;

    // Starting position for enemy formation
    private const float START_X = -5f;
    private const float START_Y = 4f;

    // Spacing between enemies in the grid
    private const float SPACING_X = 0.8f;
    private const float SPACING_Y = 0.8f;

    // Movement configuration
    private float _moveDelay = 0.5f; // Delay between movement steps
    private Vector2 _currentDirection = Vector2.right;
    private Sprite _currentSprite;

    private List<List<IEnemyView>> _enemies;
    private bool gettingKilled = false;

    /// <summary>
    /// Constructs an EnemyManager.
    /// </summary>
    /// <param name="updateManager">Manages game updates and coroutines.</param>
    /// <param name="enemyPrefab">Prefab used to instantiate enemies.</param>
    /// <param name="playerController">Reference to player controller for triggering player damage.</param>
    public EnemyManager(IUpdateManager updateManager, GameObject enemyPrefab, IPlayerController playerController) {
        _updateManager = updateManager;
        _enemyPrefab = enemyPrefab;
        _playerController = playerController;
    }

    /// <inheritdoc/>
    public void SpawnEnemies(GameObject enemiesParent, Sprite redSprite, Sprite greenSprite, Sprite yellowSprite) {
        _currentDirection = Vector2.right;
        gettingKilled = false;
        _enemies = new List<List<IEnemyView>>();

        for (int i = 0; i < ROW; i++) {
            _enemies.Add(new List<IEnemyView>());

            // Choose sprite based on row
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
                    Debug.LogError("Sprite not found, using red instead.");
                    break;
            }

            // Create each enemy in the row
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

    /// <inheritdoc/>
    public void MakeRandomEnemyShoot() {
        List<int> aliveCols = new List<int>();

        // Find all columns with at least one alive enemy
        for (int col = 0; col < COL; col++) {
            for (int row = 0; row < ROW; row++) {
                if (!_enemies[row][col].IsDead) {
                    aliveCols.Add(col);
                    break;
                }
            }
        }

        if (aliveCols.Count == 0) return;

        // Pick a random alive column
        int selectedCol = aliveCols[Random.Range(0, aliveCols.Count)];

        // Make the bottom-most alive enemy in that column shoot
        for (int row = ROW - 1; row >= 0; row--) {
            IEnemyView enemy = _enemies[row][selectedCol];
            if (!enemy.IsDead) {
                enemy.Shoot();
                break;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerator MoveEnemies() {
        while (AnyEnemyAlive()) {
            bool shouldChangeDirection = false;

            // Check if any enemy will hit a boundary
            foreach (List<IEnemyView> enemyC in _enemies) {
                foreach (IEnemyView enemy in enemyC) {
                    if (enemy.WillHitBoundary(_currentDirection) && !enemy.IsDead) {
                        shouldChangeDirection = true;
                        break;
                    }
                }
            }

            if (shouldChangeDirection) {
                // Reverse horizontal direction and move down
                _currentDirection *= -1;
                for (int i = _enemies.Count - 1; i >= 0; i--) {
                    yield return new WaitForSeconds(0.05f);
                    foreach (IEnemyView enemy in _enemies[i]) {
                        enemy.Move(Vector2.down);
                    }
                }
            } else {
                // Move horizontally
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

    /// <inheritdoc/>
    public void KillPlayer() {
        if (gettingKilled) return;
        gettingKilled = true;
        _playerController.RemoveHealth();
    }

    /// <inheritdoc/>
    public bool AnyEnemyAlive() {
        return _enemies.SelectMany(enemyC => enemyC).Any(enemy => !enemy.IsDead);
    }
}