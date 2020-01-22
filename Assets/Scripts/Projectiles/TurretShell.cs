using UnityEngine;

public class TurretShell : MonoBehaviour
{
   
    [SerializeField]
    private float _speed = 8f;

    public Vector3 Direction { get; set; } = Vector3.up;

    private Player _player;

   
    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -2.0f;

    private void Start()
    {
        this._player = GameObject.Find("Player").GetComponent<Player>();
        LogHelper.CheckForNull(_player, nameof(_player));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Direction * Time.deltaTime * _speed);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player.TakeDamage(1);
            Destroy(this.gameObject);
        }
    }
}
