using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private bool _isUpMissile = true;
    private Player _player;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -2.0f;

    public void SetDownMissile()
    {
        _isUpMissile = false;
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
        transform.Translate((_isUpMissile ? Vector3.up : Vector3.down) * Time.deltaTime * _speed);
        if (transform.position.y >= MaxBoundaryPositiveY || transform.position.y < MinBoundaryPositiveY)
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
