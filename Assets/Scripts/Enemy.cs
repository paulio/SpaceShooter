using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

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
    private const float MinBoundaryPositiveY = -2.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
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
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (this.transform.position.y <= MinBoundaryPositiveY)
        {
            if (_boxCollider.enabled)
            {
                this.transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
            }
        }

        if (Time.time > _nextFire)
        {
            if (_boxCollider.enabled)
            {
                _nextFire += Random.Range(2f, 8f);
                if (_laserPrefab != null)
                {
                    FireDoubleLaser();
                }
            }
        }
    }

    private void FireDoubleLaser()
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
        return Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }
}
