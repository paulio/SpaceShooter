using System.Collections;
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
    private GameObject _asteroidPrefab;


    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerups;

    private bool _hasStoppedSpawning;

    private bool _isSpawningAsteroids = true;

    private const float MinimumPowerupDelay = 3f;
    private const float DefaultMaximumPowerupDelayFloor = 5f;

    // Start is called before the first frame update
    void Start()
    {
        LogHelper.CheckForNull(_asteroidPrefab, nameof(_asteroidPrefab));
        if (_asteroidPrefab != null)
        {
            Instantiate(_asteroidPrefab, _enemyContainer.transform);
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(nameof(SpawnRoutine));
        StartCoroutine(SpawnPowerupRoutine());
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
            yield return new WaitForSeconds(Random.Range(MinimumPowerupDelay, Mathf.Max(_delayPowerup, DefaultMaximumPowerupDelayFloor)));
            if (!_hasStoppedSpawning)
            {
                
                GameObject nextPowerUp = null;
                int attemptCount = 4;
                while(nextPowerUp == null && attemptCount > 0)
                {
                    nextPowerUp = _powerups[Random.Range(0, _powerups.Length)];
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
            Instantiate(_enemyPrefab, _enemyContainer.transform);
            yield return new WaitForSeconds(_delayNextEnemy);
        }
    }
}
