using Assets.Scripts.Projectiles;
using UnityEngine;

public class Laser : MonoBehaviour, IProjectile
{
    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private bool _isEnemyMissile = true;

    [SerializeField]
    private Vector3 _direction = Vector3.up;

    private Player _player;

    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -2.0f;

    public bool IsEnemyMissile => _isEnemyMissile;


    public void SetEnemyMissile(bool isDirectionDown)
    {
        _isEnemyMissile = true;
        if (isDirectionDown)
        {
            _direction = Vector3.down;
        }
        else
        {
            _direction = Vector3.up;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isEnemyMissile && collision.CompareTag("Player"))
        {
            _player.TakeDamage(10);
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        this._player = GameObject.Find("Player").GetComponent<Player>();
        LogHelper.CheckForNull(_player, nameof(_player));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * Time.deltaTime * _speed);
        if (IsOutsideOfGameBounds())
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private bool IsOutsideOfGameBounds()
    {
        return transform.position.y >= MaxBoundaryPositiveY || transform.position.y < MinBoundaryPositiveY || transform.position.x < MinBoundaryPositiveX || transform.position.x > MaxBoundaryPositiveX;
    }
}
