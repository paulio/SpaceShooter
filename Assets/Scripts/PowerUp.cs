using UnityEngine;


public enum PowerUpType
{
    None = -1,
    TrippleShot = 0,
    Speed = 1,
    Shield = 2,
    Ammo = 3
}

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private PowerUpType _powerUpType = PowerUpType.None;

    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MinBoundaryPositiveY = -2.0f;
    private const float MaxBoundaryPositiveY = 8f;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(SpawnXPoint(), MaxBoundaryPositiveY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (this.transform.position.y <= MinBoundaryPositiveY)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print($"Power-up collected {_powerUpType}");
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                switch(_powerUpType)
                {
                    case PowerUpType.TrippleShot:
                        player.CollectTrippleShot();
                        break;
                    case PowerUpType.Speed:
                        player.CollectSpeedUp();
                        break;
                    case PowerUpType.Shield:
                        player.CollectShields();
                        break;
                    case PowerUpType.Ammo:
                        player.CollectAmmo();
                        break;
                    default:
                        print("Unknown powerup");
                        break;
                }
            }
            else
            {
                Debug.LogError("Player component not found");
            }

            Destroy(this.gameObject);
        }
    }

    private static float SpawnXPoint()
    {
        return Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }
}
