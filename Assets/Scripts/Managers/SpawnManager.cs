﻿using System;
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
    private GameObject _asteroidPrefab;


    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerups;

    [SerializeField]
    private Waypoints[] _waypointGroups;

    [SerializeField]
    private Wave[] _waves;

    private bool _hasStoppedSpawning;

    private bool _isSpawningAsteroids = true;

    private const float MinimumPowerupDelay = 3f;
    private const float DefaultMaximumPowerupDelayFloor = 5f;

    private int _currentWaveIndex;
    private Wave _currentWave;
    private int _currentSubWaveIndex;
    private SubWave _currentSubWave;

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
        SetNextWave();
        StartCoroutine(nameof(SpawnRoutine));
        StartCoroutine(SpawnPowerupRoutine());
    }

    private void SetNextWave()
    {
        _currentWaveIndex++;
        _currentWave = _waves[_currentWaveIndex % _waves.Length];
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
                
                GameObject nextPowerUp = null;
                int attemptCount = 4;
                while(nextPowerUp == null && attemptCount > 0)
                {
                    nextPowerUp = _powerups[UnityEngine.Random.Range(0, _powerups.Length)];
                    var powerUp = nextPowerUp.GetComponent<PowerUp>();
                    if (!powerUp.IsAvailableDueToRarity())
                    {
                        powerUp = null;
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
            _currentSubWaveIndex++;
            print($"Wave {_currentWave} Sub Wave {_currentSubWaveIndex}");
            if (_currentSubWaveIndex >= _currentWave.Waves.Length - 1)
            {
                yield return new WaitForSeconds(_delayNextEnemy);
                SetNextWave();
                _currentSubWaveIndex = 0;
            }

            _currentSubWave = _currentWave.Waves[_currentSubWaveIndex];

            CreateEnemiesForSubWave();

            yield return new WaitForSeconds(_currentSubWave.DelayUntilNextWave);
        }
    }

    private void CreateEnemiesForSubWave()
    {
        for (int enemyTypeIndex = 0; enemyTypeIndex < _currentSubWave.Enemies.Length; enemyTypeIndex++)
        {
            var enemyDefinition = _currentSubWave.Enemies[enemyTypeIndex];
            for (int enemyIndex = 0; enemyIndex < enemyDefinition.Count; enemyIndex++)
            {
                GameObject enemy;
                switch (enemyDefinition.EnemyType)
                {
                    case EnemyType.WaypointFollower:
                        enemy = CreateWaypointFollowerEnemy();
                        break;
                    default:
                        enemy = Instantiate(_enemyPrefab, _enemyContainer.transform);
                        break;
                }
            }
        }
    }

    private GameObject CreateWaypointFollowerEnemy()
    {
        GameObject enemy = Instantiate(_enemyWithWaypointsPrefab, _enemyContainer.transform);
        var waypointEnemy = enemy.GetComponent<EnemyWithWaypoints>();
        var waypointType = UnityEngine.Random.Range(0, _waypointGroups.Length);
        print($"Next Waypoint pattern {waypointType}");
        waypointEnemy.Waypoints = _waypointGroups[waypointType];
        return enemy;
    }
}