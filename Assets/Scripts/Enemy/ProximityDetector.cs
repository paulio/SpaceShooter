using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximityDetector : MonoBehaviour
{
    private CircleCollider2D _collider;
    private Action<Collider2D> _onCollision;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.enabled = true;
    }

    public void Initialize(Action<Collider2D> onCollision)
    {
        // just single callback for now
        _onCollision = onCollision;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print($"Proximity {collision.name} {collision.tag}");
        _onCollision?.Invoke(collision);
    }

    private void OnEnable()
    {
        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }

    private void OnDisable()
    {
        _collider.enabled = false;
    }
}
