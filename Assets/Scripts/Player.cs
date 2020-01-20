using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _speedAccelerateMultiplier = 10.5f;

    [SerializeField]
    private float _speedPowerUpMultiplier = 2f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _laserTrippleShotPrefab;

    [SerializeField]
    private GameObject _laserMultiShotPrefab;

    [SerializeField]
    private GameObject _shieldsChildComponent;

    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private GameObject _rightEngine;

    // NB Want the audio here to control doubling up on power-ups, player's responsiblity

    [SerializeField]
    private AudioSource _laserAudio;

    [SerializeField]
    private AudioSource _powerUpAudio;

    [SerializeField]
    private AudioSource _powerDownAudio;

    [SerializeField]
    private AudioSource _emptyShot;

    [SerializeField]
    private float _laserStartingOffset = 1f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private float _fireRate = 0.5f;

    [SerializeField]
    private int _maxAmmo = 15;


    private int _score;
    private Animator _mainCameraAnimator;
    private int _ammo;

    private float _thrustFuel = 100f;

    private Vector3 playerMoveDirection = Vector3.zero;
    private float _nextFire = 0f;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioManager _audioManager;
    private const float MaxBoundaryPositiveX = 9f;
    private const float MinBoundaryPositiveX = -9f;
    private const float MaxBoundaryPositiveY = 8f;
    private const float MinBoundaryPositiveY = -2.0f;


    private bool _isTrippleShotActive = false;
    private bool _isSpeedUpActive = false;
    private bool _isShieldsUpActive;
    private bool _isLeftEngineOnFire;
    private bool _isRightEngineOnFire;

    private bool _isImmune;
    private Shields _shields;

    private bool _isMultiShotActive;
    private bool _canThrust = true;
 
    // Start is called before the first frame update
    void Start()
    {
        // take the current pos and set the current pos to zero
        this.transform.position = Vector3.zero;
        this._spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        LogHelper.CheckForNull(_spawnManager, nameof(_spawnManager));

        this._uiManager = GameObject.Find("UIManager_Canvas").GetComponent<UIManager>();
        LogHelper.CheckForNull(_uiManager, nameof(_uiManager));

        this._audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        LogHelper.CheckForNull(_audioManager, nameof(_audioManager));

        this._mainCameraAnimator = Camera.main.GetComponent<Animator>();
        LogHelper.CheckForNull(_mainCameraAnimator, nameof(_mainCameraAnimator));

        _ammo = _maxAmmo;
        this._uiManager?.UpdateScore(_score);
        this._uiManager?.UpdateLives(_lives);
        this._uiManager?.UpdateAmmo(_ammo, _maxAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }
    }

    public void IncreaseScore(int score)
    {
        _score += score;
        _uiManager?.UpdateScore(_score);
    }


    public void CollectAmmo()
    {
        _powerUpAudio?.Play();
        _ammo = _maxAmmo;
        _uiManager?.UpdateAmmo(_ammo, _maxAmmo);
    }

    public void CollectHealth()
    {
        _powerUpAudio?.Play();
        _lives = Mathf.Clamp(_lives + 1, 0, 3);
        RepairAnEngine();
        _uiManager?.UpdateLives(_lives);
    }

    public void CollectTrippleShot()
    {
        StartCoroutine(TrippleShotPowerdown());
    }

    public void CollectMultiShot()
    {
        StartCoroutine(MultiShotPowerdown());
    }

    public void CollectSpeedUp()
    {
        StartCoroutine(SpeedUpPowerdown());
    }

    public void CollectSlowDown()
    {
        StartCoroutine(SlowDownPowerdown());
    }


    public void CollectShields()
    {
        ActivateShields(true);

        // Keep shields on until they're destroyed, if you want them to go over time uncomment below
        // StartCoroutine(ShieldsUpPowerdown());
    }

    public void TakeDamage(float damage)
    {
        if (!_isImmune)
        {
            StartCoroutine(nameof(ShakeCameraRoutine));
            if (_isShieldsUpActive)
            {
                if (_shields != null)
                {
                    _shields.TakeDamage();
                    if (_shields.HasShieldDepleted())
                    {
                        ActivateShields(false);
                    }
                }
            }
            else
            {
                _lives--;
                _uiManager?.UpdateLives(_lives);
                if (_lives < 1)
                {
                    print("Game over");
                    _audioManager?.PlayExplosion(transform.position);
                    _mainCameraAnimator.enabled = false;
                    Destroy(this.gameObject);
                    _spawnManager?.OnPlayerDeath();
                }
                else
                {
                    print($"Remaining Lives {_lives}");
                    DamageAnEngine();
                }
            }

            StartCoroutine(StartImmunityRoutine());
        }
    }

    private IEnumerator ShakeCameraRoutine()
    {
        _mainCameraAnimator.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _mainCameraAnimator.enabled = false;
    }

    private IEnumerator StartImmunityRoutine()
    {
        _isImmune = true;
        const float ImmunityAfterHit = 1f;
        yield return new WaitForSeconds(ImmunityAfterHit);
        _isImmune = false;
    }

    private void DamageAnEngine()
    {
        if (!_isLeftEngineOnFire && !_isRightEngineOnFire)
        {
            if (UnityEngine.Random.Range(0, 1) > 0.5f)
            {
                _rightEngine?.SetActive(true);
                _isRightEngineOnFire = true;
            }
            else
            {
                _leftEngine?.SetActive(true);
                _isLeftEngineOnFire = true;
            }
        }
        else if (!_isLeftEngineOnFire)
        {
            _leftEngine?.SetActive(true);
            _isLeftEngineOnFire = true;
        }
        else if (!_isRightEngineOnFire)
        {
            _rightEngine?.SetActive(true);
            _isRightEngineOnFire = true;
        }
    }

    private void RepairAnEngine()
    {
        if (_isLeftEngineOnFire)
        {
            _leftEngine?.SetActive(false);
            _isLeftEngineOnFire = false;
        }
        else if (_isRightEngineOnFire)
        {
            _rightEngine?.SetActive(false);
            _isRightEngineOnFire = false;
        }
    }

    private void ActivateShields(bool isActive)
    {
        _isShieldsUpActive = isActive;
        _shieldsChildComponent?.SetActive(isActive);
        _powerUpAudio?.Play();
        if (_shields == null && _shieldsChildComponent != null)
        {
            _shields = _shieldsChildComponent.GetComponent<Shields>();
        }

        _shields?.FullStrength();
    }

    private void FireLaser()
    {
        if (Time.time > _nextFire)
        {
            if (_ammo > 0)
            {
                if (_laserPrefab != null && _laserTrippleShotPrefab != null && _laserMultiShotPrefab != null)
                {
                    if (_isMultiShotActive)
                    {
                        Instantiate(_laserMultiShotPrefab, transform.position + Vector3.up * _laserStartingOffset, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(_isTrippleShotActive ? _laserTrippleShotPrefab : _laserPrefab, transform.position + Vector3.up * _laserStartingOffset, Quaternion.identity);
                    }

                    _laserAudio?.Play();
                    _nextFire = Time.time + _fireRate;
                }

                // assuming tripple shot only counts as 1
                _ammo--;
                _uiManager?.UpdateAmmo(_ammo, _maxAmmo);
            }
            else
            {
                _emptyShot.Play();
            }
        }
    }

    private void CalculateMovement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var isAccelerating = false;
        if (_canThrust)
        {
            isAccelerating = Input.GetKey(KeyCode.LeftShift);
        }

        playerMoveDirection.x = horizontalInput;
        playerMoveDirection.y = verticalInput;
        playerMoveDirection = playerMoveDirection * Time.deltaTime * _speed * (isAccelerating && _thrustFuel > 0f ? _speedAccelerateMultiplier : 1f);
        transform.Translate(playerMoveDirection);

        UpdateThrustFuel(isAccelerating);
        ClampBoundaries();
    }

    private void UpdateThrustFuel(bool isAccelerating)
    {
        if (isAccelerating)
        {
            _thrustFuel = Mathf.Clamp(_thrustFuel - 0.5f, 0f, 100f);
        }
        else
        {
            _thrustFuel = Mathf.Clamp(_thrustFuel + 0.05f, 0f, 100f);
        }

        if (_thrustFuel == 0)
        {
            StartCoroutine(nameof(ThrustFuelCoolDownRoutine));
        }

        _uiManager.UpdateThrustFuel(_thrustFuel);
    }

    private IEnumerator ThrustFuelCoolDownRoutine()
    {
        _canThrust = false;
        const float DelayAfterEmptyFuel = 8f;
        yield return new WaitForSeconds(DelayAfterEmptyFuel);
        _canThrust = true;
    }

    private void ClampBoundaries()
    {
        var clampX = Mathf.Clamp(transform.position.x, MinBoundaryPositiveX, MaxBoundaryPositiveX);
        var clampY = Mathf.Clamp(transform.position.y, MinBoundaryPositiveY, MaxBoundaryPositiveY);

        if (clampX != playerMoveDirection.x || clampY != playerMoveDirection.y)
        {
            // if we are at the max/min X then wrap around
            if (clampX == MaxBoundaryPositiveX)
            {
                clampX = MinBoundaryPositiveX + 1f;
            }

            if (clampX == MinBoundaryPositiveX)
            {
                clampX = MaxBoundaryPositiveX - 1f;
            }

            transform.position = new Vector3(clampX, clampY, 0);
        }
    }

    IEnumerator TrippleShotPowerdown()
    {
        _isTrippleShotActive = true;
        _powerUpAudio?.Play();
        const float trippleShotLifeTime = 5f;
        yield return new WaitForSeconds(trippleShotLifeTime);
        _isTrippleShotActive = false;
    }

    IEnumerator MultiShotPowerdown()
    {
        _isMultiShotActive = true;
        _powerUpAudio?.Play();
        const float multiShotLifeTime = 5f;
        yield return new WaitForSeconds(multiShotLifeTime);
        _isMultiShotActive = false;
    }

    IEnumerator SlowDownPowerdown()
    {
        _powerDownAudio?.Play();
        var currentSpeed = _speed;
        const float slowDownMultiplier = 0.25f;
        this._speed *= slowDownMultiplier;
        const float slowDownLifeTime = 20f;
        yield return new WaitForSeconds(slowDownLifeTime);
        _speed = currentSpeed;
    }

    IEnumerator SpeedUpPowerdown()
    {
        _isSpeedUpActive = true;
        _powerUpAudio?.Play();
        var currentSpeed = _speed;
        this._speed *= _speedPowerUpMultiplier;
        const float speedUpLifeTime = 10f;
        yield return new WaitForSeconds(speedUpLifeTime);
        _isSpeedUpActive = false;
        _speed = currentSpeed;
    }

    IEnumerator ShieldsUpPowerdown()
    {
        _isShieldsUpActive = true;
        const float shieldsUpLifeTime = 20f;
        yield return new WaitForSeconds(shieldsUpLifeTime);
        _isShieldsUpActive = false;
    }
}
