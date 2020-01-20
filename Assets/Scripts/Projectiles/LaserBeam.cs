using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Player _player;
    private bool _hasStarted;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        LogHelper.CheckForNull(_spriteRenderer, nameof(_spriteRenderer));

        this._player = GameObject.Find("Player").GetComponent<Player>();
        LogHelper.CheckForNull(_player, nameof(_player));

        this._audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        LogHelper.CheckForNull(_audioManager, nameof(_audioManager));
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasStarted)
        {
            _hasStarted = true;
            _audioManager?.PlayLaserBeam(transform.position);
            StartCoroutine(AutoDestruct());
        }

        if (_spriteRenderer.size.y < 20f)
        {
            _spriteRenderer.size += new Vector2(0f, 5f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player?.TakeDamage(10);
        }
    }

    IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
