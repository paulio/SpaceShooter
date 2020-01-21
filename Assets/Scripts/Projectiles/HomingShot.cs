using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Projectiles;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
////[RequireComponent(typeof(BoxCollider2D))]
public class HomingShot : MonoBehaviour, IProjectile
{
    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private float _turningSpeedMultiplier = 0.5f;

    [SerializeField]
    private float _radarRadius = 14f;

    private CircleCollider2D _radarCollider;
    private Transform _target;
    private bool _hasAquiredTarget;

    public bool HasTarget => _target != null;

    public bool IsEnemyMissile => false;

    // Start is called before the first frame update
    void Start()
    {
        this._radarCollider = GetComponent<CircleCollider2D>();
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            if (_hasAquiredTarget)
            {
                _hasAquiredTarget = false;
                _radarCollider.radius = _radarRadius;
            }

            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }
        else
        {
            if (_target.position.y < transform.position.y)
            {
                transform.Translate(Vector3.down * Time.deltaTime * _speed);
            }
            else if (_target.position.y > transform.position.y)
            {
                transform.Translate(Vector3.up * Time.deltaTime * _speed);
            }

            if (_target.position.x < transform.position.x)
            {
                transform.Translate(Vector3.left * Time.deltaTime * _speed * _turningSpeedMultiplier);
            }
            else if (_target.position.x > transform.position.x)
            {
                transform.Translate(Vector3.right * Time.deltaTime * _speed * _turningSpeedMultiplier);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_target == null)
        {
            if (collision.IsTouching(_radarCollider))
            {
                if (collision.CompareTag("Enemy"))
                {
                    _target = collision.transform;
                    const float colliderRadius = 1f;
                    _radarCollider.radius = colliderRadius;
                    _hasAquiredTarget = true;
                }
            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<ITakeDamage>();
                if (enemy != null)
                {
                    enemy.TakeDamage(this.gameObject);
                }
            }
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }

}
