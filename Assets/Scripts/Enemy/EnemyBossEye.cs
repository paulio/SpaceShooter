using UnityEngine;
using UnityEngine.Events;

public class EnemyBossEye : MonoBehaviour, ITakeDamage
{
    [SerializeField]
    int _hitPoints = 10;

    [SerializeField]
    float _hitAnimationEffectStep = 1.2f;

    public UnityEvent Destroyed = new UnityEvent();

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private int _speedMultiplierParam;
    private float _speedMultipler = 1f;

    public bool IsImmune { get; set; }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        LogHelper.CheckForNull(_spriteRenderer, nameof(_spriteRenderer));

        _animator = GetComponent<Animator>();
        LogHelper.CheckForNull(_animator, nameof(_animator));

        _speedMultiplierParam = Animator.StringToHash("SpeedMultiplier");
    }

    public void TakeDamage(GameObject other)
    {
        _hitPoints--;
        if (_hitPoints < 1)
        {
            if (Destroyed != null)
            {
                Destroyed.Invoke();
            }
            else
            {
                Debug.LogError("No handler for eye destroyed");
            }
        }
        else
        {
            DarkenEye();
            IncreaseEyePulseRate();
        }

        Destroy(other);
    }

    private void IncreaseEyePulseRate()
    {
        _speedMultipler += _hitAnimationEffectStep;
        _animator.SetFloat(_speedMultiplierParam, _speedMultipler);
    }

    private void DarkenEye()
    {
        const float colorHitStep = 0.05f;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g - colorHitStep, _spriteRenderer.color.b - colorHitStep);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsImmune && collision.CompareTag("Laser"))
        {
            TakeDamage(collision.gameObject);
        }
    }
}
