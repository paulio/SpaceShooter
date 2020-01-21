using UnityEngine;


public enum PowerUpType
{
    None = -1,
    TrippleShot = 0,
    Speed = 1,
    Shield = 2,
    Ammo = 3,
    Health = 4,
    MultiShot = 5,
    Slow
}

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private float _rarityPercentage = 80f;


    [SerializeField]
    private PowerUpType _powerUpType = PowerUpType.None;

    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MinBoundaryPositiveY = -2.0f;
    private const float MaxBoundaryPositiveY = 8f;


    public bool IsAvailableDueToRarity()
    {
        return Random.Range(1f, 100f) > _rarityPercentage;
    }

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
            CollectPlayerPowerUp(collision);
        }
        else if (collision.CompareTag("Laser"))
        {
            var laser = collision.GetComponent<Laser>();
            if (laser.IsEnemyMissile)
            {
                print("Power up hit by enemy missile");
                Destroy(this.gameObject);
                Destroy(laser.gameObject);
            }
        }
    }

    private void CollectPlayerPowerUp(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            switch (_powerUpType)
            {
                case PowerUpType.TrippleShot:
                    player.CollectTrippleShot();
                    break;
                case PowerUpType.MultiShot:
                    player.CollectMultiShot();
                    break;
                case PowerUpType.Speed:
                    player.CollectSpeedUp();
                    break;
                case PowerUpType.Slow:
                    player.CollectSlowDown();
                    break;
                case PowerUpType.Shield:
                    player.CollectShields();
                    break;
                case PowerUpType.Ammo:
                    player.CollectAmmo();
                    break;
                case PowerUpType.Health:
                    player.CollectHealth();
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

    private static float SpawnXPoint()
    {
        return Random.Range(MinBoundaryPositiveX, MaxBoundaryPositiveX);
    }
}
