using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex;
    private Vector3 direction;
    public float gravity = -9.8f;
    public float strength = 5f;

    private int score = 0;

    private AudioSource audioSource;
    public AudioClip dieSound; // Звук получения очков
    public AudioClip scoringSound; // Звук получения очков



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // получаем spriterenderer для объекта к которому прикреплен скрипт
        audioSource = GetComponent<AudioSource>(); // Получаем AudioSource
    }

    private void Start() // вызывается в первом кадре когда объект включен
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f); // invokerepeating - бесконечное повторение другой функции, первое это начальное значение, второе частота повторения
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            direction = Vector3.up * strength;

            // Воспроизводим звук прыжка
            if (audioSource != null)
            {
                audioSource.Play(); // Воспроизводим звук
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Воспроизводим звук при нажатии на экран
                direction = Vector3.up * strength;
                if (audioSource != null)
                {
                    audioSource.Play(); // Воспроизводим звук
                }
            }
        }

        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
    }


    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            FindObjectOfType<GameManager>().GameOver();
            score = 0;
            if (dieSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(dieSound); // Воспроизводим звук при получении очков
            }
        }
        else if (other.gameObject.tag == "Scoring")
        {
            FindObjectOfType<GameManager>().IncreaseScore();

            score++;
            // Воспроизводим звук получения очков
            if (scoringSound != null && audioSource != null && score % 5 == 0)
            {
                audioSource.PlayOneShot(scoringSound); // Воспроизводим звук при получении очков
            }
        }
    }

    public void SetSoundState(bool isSoundOn)
    {
        if (audioSource != null)
        {
            audioSource.mute = !isSoundOn; // Устанавливаем mute в зависимости от состояния звука
        }
    }

}
