using System;
using System.Collections;
using Assets.Scripts.Enemy;
using Assets.Scripts.Managers.Waves;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    [SerializeField]
    private float _spawnWaveDelay = 3.5f;

    [SerializeField]
    private float _delayNextEnemy = 5f;

    [SerializeField]
    private float _delayPowerup = 5f;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyWithWaypointsPrefab;

    [SerializeField]
    private GameObject _enemyWithBeamPrefab;

    [SerializeField]
    private GameObject _enemyFireBackwardsPrefab;

    [SerializeField]
    private GameObject _asteroidPrefab;

    [SerializeField]
    private GameObject _bossPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    private PowerUp[] _powerups;

    [SerializeField]
    private Waypoints[] _waypointGroups;

    [SerializeField]
    private Wave[] _waves;

    private bool _hasStoppedSpawning;

    private const float MinimumPowerupDelay = 3f;
    private const float DefaultMaximumPowerupDelayFloor = 5f;

    private int _currentWaveIndex;
    private Wave _currentWave;
    private int _currentSubWaveIndex;
    private SubWave _currentSubWave;

    private int _currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        LogHelper.CheckForNull(_asteroidPrefab, nameof(_asteroidPrefab));

        if (_asteroidPrefab != null)
        {
            Instantiate(_asteroidPrefab, _enemyContainer.transform);
        }

        _currentWaveIndex = -1;
    }

    public void StartSpawning()
    {
        // if you want to test a single type of enemy uncomment the following;
        ////CreateTypeOfEnemy(EnemyType.LaserBeam);
        ////return;

        SetNextWave();
        StartCoroutine(nameof(SpawnRoutine));
        StartCoroutine(SpawnPowerupRoutine());
    }

    private void SetNextWave()
    {
        _currentWaveIndex++;
        _currentWave = _waves[_currentWaveIndex % _waves.Length];
        if (_currentWaveIndex % _waves.Length == 0)
        {
            _currentLevel++;
        }

        _currentSubWaveIndex = -1;
        print($"Wave {_currentWaveIndex} {_currentWave.name}");
    }

    public void OnPlayerDeath()
    {
        _hasStoppedSpawning = true;
        foreach (var enemy in _enemyContainer.GetComponentsInChildren<Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        foreach (var enemy in _enemyContainer.GetComponentsInChildren<Asteroid>())
        {
            Destroy(enemy.gameObject);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_spawnWaveDelay);

        while (!_hasStoppedSpawning)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(MinimumPowerupDelay, Mathf.Max(_delayPowerup, DefaultMaximumPowerupDelayFloor)));
            if (!_hasStoppedSpawning)
            {
                PowerUp nextPowerUp = null;
                int attemptCount = 4;
                while(nextPowerUp == null && attemptCount > 0)
                {
                    nextPowerUp = _powerups[UnityEngine.Random.Range(0, _powerups.Length)];
                    var powerUp = nextPowerUp.GetComponent<PowerUp>();
                    if (!powerUp.IsAvailableDueToRarity())
                    {
                        nextPowerUp = null;
                        attemptCount--;
                    }
                }

                if (nextPowerUp != null)
                {
                    Instantiate(nextPowerUp);
                }
            }
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(_spawnWaveDelay);
        while (!_hasStoppedSpawning)
        {
            bool isWaitingForAllDestroyedEnemy = false;
            if (_currentSubWave != null && _currentSubWave.DelayUntilNextWave == SubWave.DelayUntilAllEnemiesAreDead)
            {
                var anyChildren = _enemyContainer.transform.childCount > 0;
                isWaitingForAllDestroyedEnemy = anyChildren;
            }

            if (!isWaitingForAllDestroyedEnemy)
            {
                _currentSubWaveIndex++;
                print($"_currentSubWaveIndex {_currentSubWaveIndex}");
                if (_currentSubWaveIndex > _currentWave.Waves.Length - 1)
                {
                    print("subwaves reached max");
                    yield return new WaitForSeconds(_delayNextEnemy);
                    SetNextWave();
                    _currentSubWaveIndex = 0;
                }

                try
                {
                    _currentSubWave = _currentWave.Waves[_currentSubWaveIndex];
                    if (_currentSubWaveIndex == _currentWave.Waves.Length - 1)
                    {
                        // always force last sub wave to wait until all enemies are dead
                        _currentSubWave.DelayUntilNextWave = SubWave.DelayUntilAllEnemiesAreDead;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message} _currentSubWaveIndex {_currentSubWaveIndex} for {_currentWave.name}");
                }

                SetPowerUpsForSubWave();
                CreateEnemiesForSubWave();
            }

            if (_currentSubWave.DelayUntilNextWave == SubWave.DelayUntilAllEnemiesAreDead)
            {
                // poll to check for destroyed enemy
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(_currentSubWave.DelayUntilNextWave);
            }
            
        }
    }

    private void SetPowerUpsForSubWave()
    {
        _powerups = _currentSubWave.PowerUps;
    }

    private void CreateEnemiesForSubWave()
    {
        for (int enemyTypeIndex = 0; enemyTypeIndex < _currentSubWave.Enemies.Length; enemyTypeIndex++)
        {
            var enemyDefinition = _currentSubWave.Enemies[enemyTypeIndex];
            if (enemyDefinition.EnemyType == EnemyType.Boss)
            {
                var bossGameObject = CreateTypeOfEnemy(enemyDefinition.EnemyType);
                var boss = bossGameObject.GetComponent<EnemyBoss>();
                boss.CurrentLevel = _currentLevel;
            }
            else
            {
                for (int enemyIndex = 0; enemyIndex < (enemyDefinition.Count - 1 + _currentLevel); enemyIndex++)
                {
                    CreateTypeOfEnemy(enemyDefinition.EnemyType);
                }
            }
        }
    }

    private GameObject CreateTypeOfEnemy(EnemyType enemyType)
    {
        GameObject enemy;

        switch (enemyType)
        {
            case EnemyType.WaypointFollower:
                enemy = CreateWaypointFollowerEnemy(_enemyWithWaypointsPrefab);
                break;
            case EnemyType.LaserBeam:
                enemy = CreateWaypointFollowerEnemy(_enemyWithBeamPrefab);
                break;
            case EnemyType.FireBackwards:
                enemy = Instantiate(_enemyFireBackwardsPrefab, _enemyContainer.transform);
                break;
            case EnemyType.Boss:
                enemy = Instantiate(_bossPrefab, _enemyContainer.transform);
                break;
            default:
                enemy = Instantiate(_enemyPrefab, _enemyContainer.transform);
                break;
        }

        if (enemy == null)
        {
            Debug.LogError("Enemy failed to be created");
        }

        return enemy;
    }

    private GameObject CreateWaypointFollowerEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(enemyPrefab, _enemyContainer.transform);
        var waypointEnemy = enemy.GetComponent<EnemyWithWaypoints>();
        var waypointType = UnityEngine.Random.Range(0, _waypointGroups.Length);
        waypointEnemy.Waypoints = _waypointGroups[waypointType];
        return enemy;
    }
}
