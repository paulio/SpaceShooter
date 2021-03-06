﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private Image _livesImage;

    [SerializeField]
    private Image _thrustFuelImage;

    [SerializeField]
    private Sprite[] livesSprites;

    private GameManager _gameManager;
    private ThrustFuel _thrustFuel;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 000";
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        LogHelper.CheckForNull(_gameManager, nameof(_gameManager));
        _thrustFuel = new ThrustFuel(_thrustFuelImage);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score.ToString("D3")}";
    }

    public void UpdateAmmo(int ammo, int maxAmmo)
    {
        _ammoText.text = $"Ammo: {ammo.ToString("D2")}/{maxAmmo.ToString("D2")}";
        if (ammo < 1)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
        }
    }

    public void UpdateThrustFuel(float thrustFuel)
    {
        if (_thrustFuel != null)
        {
            _thrustFuel.UpdateThrustFuel(thrustFuel);
        }
    }

    public void UpdateLives(int lives)
    {
        _livesImage.sprite = livesSprites[lives];
        if (lives < 1)
        {
            if (_gameManager != null)
            {
                _gameManager.IsGameOver = true;
            }

            StartCoroutine(GameOverFlickerRoutine());
        }
        else
        {
            if (_gameOverText != null && _gameOverText.enabled)
            {
                StopCoroutine(GameOverFlickerRoutine());
                _gameOverText.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        if (_restartText != null)
        {
            _restartText.gameObject.SetActive(true);
            while (true)
            {
                _gameOverText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                _gameOverText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
