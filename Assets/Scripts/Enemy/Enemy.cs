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

    private Player _player;
    private int _isDestroyedHash;
    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private AudioManager _audioManager;
    private float _nextFire;
    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -4.0f;


    protected GameObject LaserPrefab => _laserPrefab;

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

        _nextFire = Time.time + Random.Range(0.5f, 3f);
        print("Enemy started");
    }

    // Update is called once per frame
    void Update()
    {
        Move(_boxCollider.enabled);

        if (Time.time > _nextFire)
        {
            if (_boxCollider.enabled)
            {
                _nextFire += Random.Range(2f, 8f);
                if (_laserPrefab != null)
                {
                    Fire();
                }
            }
        }
    }

    protected virtual void Move(bool isAlive)
    {
        var moveDirection = Vector3.down * Time.deltaTime * _speed;
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

    private void SetStartPosition()
    {
        transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
    }

    protected virtual void Fire()
    {
        var laserObject = Instantiate(_laserPrefab, transform.position + Vector3.up, Quaternion.identity);
        var lasers = laserObject.GetComponentsInChildren<Laser>();
        for (int laserIndex = 0; laserIndex < lasers.Length; laserIndex++)
        {
            lasers[laserIndex].SetDownMissile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player?.TakeDamage(_damage);
            SetAsDestroyed();
        }

        if (other.CompareTag("Laser"))
        {
            if (other.GetComponent<Laser>().IsUpMissile)
            {
                Destroy(other.gameObject);
                SetAsDestroyed();

                _player?.IncreaseScore(10);
            }
        }
    }

    private void SetAsDestroyed()
    {
        _boxCollider.enabled = false;
        _animator?.SetTrigger(_isDestroyedHash);
        _audioManager?.PlayExplosion(transform.position);
        const float DestroyedMomentumeSpeedReduction = 0.9f;
        const float DestroyedTimeOnScreen = 2f;
        
        _speed *= DestroyedMomentumeSpeedReduction;
        Destroy(this.gameObject, DestroyedTimeOnScreen);
    }

    private static float SpawnXPoint()
    {
        return MaxBoundaryPositiveX / 2f;// Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }
}
