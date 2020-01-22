using System.Collections;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private GameObject _destroyedTurretPrefab;

    [SerializeField]
    private GameObject _laserBeamPrefab;

    [SerializeField]
    private float _destroyedTurretYOffset = 0f;

    [SerializeField]
    private float _speed = 1f;

    [SerializeField]
    private Vector3 _startingPosition = new Vector3(0f, 11f);

    [SerializeField]
    private Vector3 _endPosition = new Vector3(0f, 4.65f);

    [SerializeField]
    private GameObject _explosion;

    private LaserBeam _laserBeam;
    private EnemyBossEye _bossEye;
    private EnemyTurret[] _turrets;
    private AudioManager _audioManager;
    private bool _isDestroyed;
    private Player _player;
    private float _nextFire;
    private bool _canFire = false;

    public int CurrentLevel { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        this._audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        LogHelper.CheckForNull(_audioManager, nameof(_audioManager));

        this._player = GameObject.Find("Player").GetComponent<Player>();
        LogHelper.CheckForNull(_player, nameof(_player));

        _laserBeam = _laserBeamPrefab.GetComponent<LaserBeam>();
        LogHelper.CheckForNull(_laserBeam, nameof(_laserBeam));

        _bossEye = GetComponentInChildren<EnemyBossEye>();
        _bossEye.HitPoints += CurrentLevel;
        LogHelper.CheckForNull(_bossEye, nameof(_bossEye));

        _bossEye.Destroyed.AddListener(BossEyeDestroyed);
        _bossEye.IsImmune = true;

        _turrets = GetComponentsInChildren<EnemyTurret>();
        
        foreach(var turret in _turrets)
        {
            turret.Destroyed.AddListener(OnTurretDestroyed);
            turret.IsImmune = true;
            turret.HitPoints += CurrentLevel;
        }

        _nextFire = Time.time + UnityEngine.Random.Range(2f, 10f);

        this.transform.position = _startingPosition;
    }

    void Update()
    {
        if (_isDestroyed)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime;
        }
        else
        {
            if (transform.position.y > _endPosition.y)
            {
                transform.Translate(Vector3.down * Time.deltaTime * _speed);
            }
            else
            {
                if (!_canFire)
                {
                    _canFire = true;
                    _nextFire = Time.time + UnityEngine.Random.Range(5f, 10f);
                    _bossEye.IsImmune = false;
                    foreach (var turret in _turrets)
                    {
                        turret.IsImmune = false;
                    }
                }
            }

            if (_canFire && _player != null && Time.time > _nextFire)
            {
                _nextFire = Time.time + UnityEngine.Random.Range(5f, 10f);
                var laserObject = Instantiate(_laserBeamPrefab, transform.position + (Vector3.up * -0.4f), Quaternion.identity);
                StartCoroutine(FireBeamRoutine());
            }
        }
    }

    private IEnumerator FireBeamRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        _laserBeam.enabled = true;
        yield return new WaitForSeconds(3.0f);
    }

    private void OnTurretDestroyed(EnemyTurret enemyTurret)
    {
        enemyTurret.Destroyed.RemoveAllListeners();
        _audioManager.PlayExplosion(transform.position);
        Instantiate(_destroyedTurretPrefab, enemyTurret.transform.position + new Vector3(0, _destroyedTurretYOffset), Quaternion.identity, transform);
        Destroy(enemyTurret.gameObject);
    }

    private void BossEyeDestroyed()
    {
        _bossEye.Destroyed.RemoveAllListeners();
        foreach(var turret in _turrets)
        {
            if (turret != null)
            {
                turret.Destroyed.RemoveAllListeners();
                turret.MarkAsDestroyed();
            }
        }

        Instantiate(_explosion, transform);
        if (_audioManager) _audioManager.PlayExplosion(transform.position);
        Destroy(_bossEye.gameObject);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        _isDestroyed = true;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player.TakeDamage(1);
        }
    }
}
