using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class TurretDestroyed : UnityEvent<EnemyTurret>
{ 
}


public class EnemyTurret : MonoBehaviour, ITakeDamage
{
    [SerializeField]
    private GameObject _shellPrefab;

    [SerializeField]
    private int _salvoLength = 4;

    [SerializeField]
    float _rotationAdjustment = 0f;

    [SerializeField]
    int _hitPoints = 5;

    private float _nextFire;
    private GameObject _player;
    private SpriteRenderer _spriteRenderer;
    public UnityEvent<EnemyTurret> Destroyed = new TurretDestroyed();
    private bool _isDestroyed;

    public bool IsImmune { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        LogHelper.CheckForNull(_player, nameof(_player));

        _spriteRenderer = GetComponent<SpriteRenderer>();
        LogHelper.CheckForNull(_spriteRenderer, nameof(_spriteRenderer));

        _nextFire = Time.time + UnityEngine.Random.Range(2f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDestroyed && gameObject != null && _player != null)
        {
            RotateTurretTowardsPlayer();

            if (!IsImmune && Time.time > _nextFire)
            {
                if (_shellPrefab != null)
                {
                    _nextFire += UnityEngine.Random.Range(5f, 10f);
                    StartCoroutine(FireSalvo());
                }
            }
        }
    }

    private void RotateTurretTowardsPlayer()
    {
        var dir = _player.transform.position - transform.position;
        var angle = (Mathf.Atan2(dir.y, dir.x) + _rotationAdjustment) * Mathf.Rad2Deg;
        var targetAngle = Quaternion.AngleAxis(angle, Vector3.forward);

        const float speed = 2f;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * speed);
    }

    private IEnumerator FireSalvo()
    {
        for (int salvoIndex = 0; salvoIndex < _salvoLength; salvoIndex++)
        {
            FireShellFromTipOfGun();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void FireShellFromTipOfGun()
    {
        var direction = Vector3.Normalize(_player.transform.position - transform.position);
        const float shellStartOffset = 0.4f;
        direction = Vector3.down * shellStartOffset;
        var shellGameObject = Instantiate(_shellPrefab, transform.position, transform.rotation);
        shellGameObject.transform.Translate(direction);
        var shell = shellGameObject.GetComponent<TurretShell>();
        shell.Direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsImmune && collision.CompareTag("Laser"))
        {
            TakeDamage(collision.gameObject);
        }
    }

    public void TakeDamage(GameObject other)
    {
        _hitPoints--;
        if (_hitPoints < 1)
        {
            if (Destroyed != null)
            {
                Destroyed.Invoke(this);
            }
        }
        else
        {
            DarkenTurret();
        }

        Destroy(other);
    }

    private void DarkenTurret()
    {
        const float colorHitStep = 0.05f;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g - colorHitStep, _spriteRenderer.color.b - colorHitStep);
    }

    public void MarkAsDestroyed()
    {
        _isDestroyed = true;
    }
}
