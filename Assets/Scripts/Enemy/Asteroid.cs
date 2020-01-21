using UnityEngine;

public class Asteroid : MonoBehaviour, ITakeDamage
{
    [SerializeField]
    private float _speed = 4f;

    [SerializeField]
    private float _speedRotation = 0.2f;

    [SerializeField]
    private float _damage = 1f;

    [SerializeField]
    private GameObject _explosion;

    private Player _player;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;
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

        this._circleCollider = GetComponent<CircleCollider2D>();
        LogHelper.CheckForNull(_circleCollider, nameof(_circleCollider));

        this._spriteRenderer = GetComponent<SpriteRenderer>();
        LogHelper.CheckForNull(_spriteRenderer, nameof(_spriteRenderer));

        this._spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        LogHelper.CheckForNull(_spawnManager, nameof(_spawnManager));

        this._audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        LogHelper.CheckForNull(_audioManager, nameof(_audioManager));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * Time.deltaTime * _speed);
        transform.Rotate(Vector3.forward * Time.deltaTime * _speedRotation);
        if (this.transform.position.y <= MinBoundaryPositiveY)
        {
            if (_circleCollider.enabled)
            {
                this.transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.IsTouching(_circleCollider))
        {
            if (other.CompareTag("Player"))
            {
                if (_player != null)
                {
                    _player.TakeDamage(_damage);
                }
                SetAsDestroyed();
            }
            else if (other.CompareTag("Laser"))
            {
                var homingShot = other.GetComponent<HomingShot>();
                if (homingShot != null)
                {
                    // ignore collision here, let the homing shot deal.
                    print("ignore collision here, let the homing shot deal.");
                }
                else
                {
                    print("Asteroid hit");
                    TakeDamage(other.gameObject);
                }
            }
        }
    }

    private void SetAsDestroyed()
    {
        _circleCollider.enabled = false;
        _spriteRenderer.enabled = false;
        Instantiate(_explosion, transform);
        _audioManager.PlayExplosion(transform.position);
        _speed *= 0.9f;
        Destroy(this.gameObject, 2f);
        _spawnManager.StartSpawning();
    }

    private static float SpawnXPoint()
    {
        return Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }

    public void TakeDamage(GameObject other)
    {
        Destroy(other.gameObject);
        SetAsDestroyed();

        if (_player != null)
        {
            _player.IncreaseScore(10);
        }
    }
}
