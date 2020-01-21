using System;
using Assets.Scripts.Enemy;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 4f;

    [SerializeField]
    private float _damage = 1f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _shieldsChildComponent;

    [SerializeField]
    private ProximityDetector _proximityChildComponent;


    [SerializeField]
    private float _percentageChanceOfShields = 20f;

    [SerializeField]
    private float _percentageChanceOfAggressive = 20f;

    private Player _player;
    private int _isDestroyedHash;
    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private AudioManager _audioManager;
    private bool _hasShields;
    private SpriteRenderer _spriteRender;
    private float _nextFire;
    private Shields _shields;
    private bool _isAggressive;
    private bool _isRamPlayer;
    private bool _hasFiredAtPowerUp;
    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -4.0f;


    protected GameObject LaserPrefab => _laserPrefab;

    protected Player Player => _player;

    protected SpriteRenderer SpriteRenderer => _spriteRender;

    // Start is called before the first frame update
    void Start()
    {
        SetStartPosition();
        this._player = GameObject.Find("Player").GetComponent<Player>();
        LogHelper.CheckForNull(_player, nameof(_player));

        this._isDestroyedHash = Animator.StringToHash("IsDestroyed");
        this._animator = GetComponent<Animator>();
        LogHelper.CheckForNull(_animator, nameof(_animator));

        this._boxCollider = GetComponent<BoxCollider2D>();
        LogHelper.CheckForNull(_boxCollider, nameof(_boxCollider));

        this._audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        LogHelper.CheckForNull(_audioManager, nameof(_audioManager));

        this._spriteRender = GetComponent<SpriteRenderer>();

        _nextFire = Time.time + UnityEngine.Random.Range(0.5f, 3f);

        if (_shieldsChildComponent != null)
        {
            _hasShields = UnityEngine.Random.Range(0f, 100f) > 100f -_percentageChanceOfShields;
            if (_hasShields)
            {
                _shields = _shieldsChildComponent.GetComponent<Shields>();
                _shields.Initialize(2);
                ActivateShields(true);
            }
        }

        if (_proximityChildComponent != null)
        {
            _proximityChildComponent.Initialize(OnProximityDetected);
            _proximityChildComponent.enabled = true;

            _isAggressive = UnityEngine.Random.Range(0f, 100f) > 100f - _percentageChanceOfAggressive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move(_boxCollider.enabled);

        if (Time.time > _nextFire)
        {
            if (_boxCollider.enabled)
            {
                if (_laserPrefab != null)
                {
                    Fire(isDirectionDown: true);
                }
            }
        }
    }

    protected virtual void Move(bool isAlive)
    {
        Vector3 moveDirection;
        if (_isRamPlayer)
        {
            moveDirection = Vector3.down * Time.deltaTime * _speed;
            if (_player.transform.position.x < transform.position.x)
            {
                moveDirection += Vector3.left * Time.deltaTime * _speed;
            }

            if (_player.transform.position.x > transform.position.x)
            {
                moveDirection += Vector3.right * Time.deltaTime * _speed;
            }
        }
        else
        {
            moveDirection = Vector3.down * Time.deltaTime * _speed;
        }

        transform.Translate(moveDirection);

        if (isAlive)
        {
            ClampBoundaries(moveDirection);
        }
    }

    protected bool ClampBoundaries(Vector3 moveDirection)
    {
        var hasClamped = false;
        var clampX = Mathf.Clamp(transform.position.x, MinBoundaryPositiveX, MaxBoundaryPositiveX);
        var clampY = Mathf.Clamp(transform.position.y, MinBoundaryPositiveY, MaxBoundaryPositiveY);

        if (clampX != moveDirection.x || clampY != moveDirection.y)
        {
            // if we are at the max/min X then wrap around
            if (clampX == MaxBoundaryPositiveX)
            {
                clampX = MinBoundaryPositiveX + 1f;
                hasClamped = true;
            }

            if (clampX == MinBoundaryPositiveX)
            {
                clampX = MaxBoundaryPositiveX - 1f;
                hasClamped = true;
            }

            if (clampY == MinBoundaryPositiveY)
            {
                clampY = MaxBoundaryPositiveY;
                hasClamped = true;
            }

            transform.position = new Vector3(clampX, clampY, 0);
        }
        else
        {
            print("no clamp");
        }

        return hasClamped;
    }


    private void ActivateShields(bool isActive)
    {
        if (_shieldsChildComponent)
            _shieldsChildComponent.SetActive(isActive);
    }

    private void SetStartPosition()
    {
        transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
    }

    protected virtual void Fire(bool isDirectionDown)
    {
        var laserObject = Instantiate(_laserPrefab, transform.position + Vector3.up, Quaternion.identity);
        var lasers = laserObject.GetComponentsInChildren<Laser>();
        for (int laserIndex = 0; laserIndex < lasers.Length; laserIndex++)
        {
            lasers[laserIndex].SetEnemyMissile(isDirectionDown);
        }

        _nextFire += UnityEngine.Random.Range(2f, 8f);
    }

    /// <remarks>Don't strictly need this as OnTriggerEnter2D fires as well, but keeps clean separation</remarks>
    private void OnProximityDetected(Collider2D other)
    {
        if (_isAggressive && other.CompareTag("Player"))
        {
            _isRamPlayer = true;
        }

        if (!_hasFiredAtPowerUp && other.CompareTag("PowerUp"))
        {
            _hasFiredAtPowerUp = true;
            Fire(isDirectionDown: true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.IsTouching(_boxCollider))
        {
            if (other.CompareTag("Player"))
            {
                _player.TakeDamage(_damage);
                SetAsDestroyed();
            }

            if (other.CompareTag("Laser"))
            {
                if (!other.GetComponent<Laser>().IsEnemyMissile)
                {
                    TakeDamage(other);
                }
            }
        }
    }

    private void TakeDamage(Collider2D other)
    {
        if (_hasShields)
        {
            _shields.TakeDamage();
            if (_shields.HasShieldDepleted())
            {
                ActivateShields(false);
                _hasShields = false;
            }
        }
        else
        {
            Destroy(other.gameObject);
            SetAsDestroyed();

            _player.IncreaseScore(10);
        }
    }

    private void SetAsDestroyed()
    {
        _boxCollider.enabled = false;
        _animator.SetTrigger(_isDestroyedHash);
        _audioManager.PlayExplosion(transform.position);
        const float DestroyedMomentumeSpeedReduction = 0.9f;
        const float DestroyedTimeOnScreen = 2f;
        
        _speed *= DestroyedMomentumeSpeedReduction;
        Destroy(this.gameObject, DestroyedTimeOnScreen);
    }

    private static float SpawnXPoint()
    {
        return UnityEngine.Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }
}
