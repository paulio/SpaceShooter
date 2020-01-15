using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private bool _isUpMissile = true;

    [SerializeField]
    private Vector3 _direction = Vector3.up;

    private Player _player;

    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -2.0f;


    public void SetDownMissile()
    {
        _isUpMissile = false;
        _direction = Vector3.down;
    }


    public bool IsUpMissile => _isUpMissile;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isUpMissile && collision.CompareTag("Player"))
        {
            _player?.TakeDamage(10);
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
        if (transform.position.y >= MaxBoundaryPositiveY || transform.position.y < MinBoundaryPositiveY || transform.position.x < MinBoundaryPositiveX || transform.position.x > MaxBoundaryPositiveX)
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
}
