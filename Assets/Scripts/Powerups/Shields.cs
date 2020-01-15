using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shields : MonoBehaviour
{
    private Animator _animator;
    private int _animatorHitHash;

    [SerializeField]
    private int _maxLives = 3;

    private int _shieldHits;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        LogHelper.CheckForNull(_animator, nameof(_animator));
        _animatorHitHash = Animator.StringToHash("HitCount");
    }

    public void FullStrength()
    {
        _shieldHits = 0;
        SetHitOnAnimator();
    }

    public void TakeDamage()
    {
        _shieldHits++;
        SetHitOnAnimator();
    }

    private void SetHitOnAnimator()
    {
        if (_animator)
        {
            _animator.SetInteger(_animatorHitHash, _shieldHits);
        }
    }

    public bool HasShieldDepleted()
    {
        return _maxLives <= _shieldHits;
    }
}
