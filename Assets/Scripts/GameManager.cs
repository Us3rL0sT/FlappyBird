using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;
public class GameManager : MonoBehaviour
{

    public Player player;
    public TMP_Text scoreText;
    public TMP_Text maxScoreText;

    public TMP_Text finishText;
    public TMP_Text ofText;
    public GameObject playButton;
    public GameObject gameOver;

    public Button soundButton;
    public Button ruLanguageButton;
    public Button engLanguageButton;
    public AudioSource audioSourceButton;
    public Image soundButtonImage;

    public Sprite soundOnIcon;
    public Sprite soundOffIcon;
    public AudioSource audioSource;

    public GameObject background;
    public Button adsButton; // Перетащите сюда ссылку на вашу кнопку из инспектора

    private int score;
    private int scoreTemp;
    private int maxScore;
    public static int language = 0;
    private bool isSoundOn = true; // Состояние звука

    private bool adsCheck = false;
    private void Awake()
    {

        if (ruLanguageButton == null || engLanguageButton == null)
        {
            Debug.LogError("One or more buttons are not assigned!");
            return; // Прекращаем выполнение метода, если кнопки не назначены
        }

        ruLanguageButton.onClick.AddListener(() => SwitchLanguage(ruLanguageButton, engLanguageButton, 1));
        engLanguageButton.onClick.AddListener(() => SwitchLanguage(engLanguageButton, ruLanguageButton, 0));



        Application.targetFrameRate = 60;
        gameOver.SetActive(false);
        Pause();
    }

    private void Start()
    {
        YandexGame.StickyAdActivity(true);
        YandexGame.FullscreenShow();
        soundButton.onClick.AddListener(OnButtonSoundClick);
        adsButton.onClick.AddListener(WatchAds);
        adsButton.gameObject.SetActive(false);
        finishText.gameObject.SetActive(false);
        maxScoreText.gameObject.SetActive(false);

    }

    private void Update()
    {
        switch (language)
        {
            case 0:
                finishText.text = "Финиш";
                ofText.text = "за";
                break;
            case 1:
                finishText.text = "Game Over";
                ofText.text = "of";
                break;
            default:
                finishText.text = "Game Over"; // Значение по умолчанию
                ofText.text = "of";
                break;
        }
    }

    void SwitchLanguage(Button buttonToHide, Button buttonToShow, int lang)
    {
        PlayButtonSound(isSoundOn);
        language = lang; // Обновляем глобальную переменную
        PlayerPrefs.SetInt("language", language);
        PlayerPrefs.Save(); // Сохраняем изменения
        buttonToHide.gameObject.SetActive(false); // Скрыть текущую кнопку
        buttonToShow.gameObject.SetActive(true);  // Показать следующую кнопку
    }


    public void Play()
    {
        Debug.Log("dsad" + adsCheck);
        if (adsCheck == true)
        {
            score = scoreTemp;
            scoreTemp = 0;
        }
        else
        {
            scoreTemp = 0;
            score = 0;
        }
        scoreText.text = score.ToString();
        adsCheck = false;


        maxScoreText.gameObject.SetActive(false);
        playButton.SetActive(false);
        gameOver.SetActive(false);
        soundButton.gameObject.SetActive(false);
        ruLanguageButton.gameObject.SetActive(false);
        engLanguageButton.gameObject.SetActive(false);
        adsButton.gameObject.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }


    public void GameOver()
    {
        gameOver.SetActive(true);
        playButton.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        soundButton.gameObject.SetActive(true);
        ruLanguageButton.gameObject.SetActive(true);
        engLanguageButton.gameObject.SetActive(true);
        adsButton.gameObject.SetActive(true);
        finishText.gameObject.SetActive(true);
        if (maxScore <= score)
        {
            maxScore = score;
            YandexGame.NewLeaderboardScores("score", maxScore);
        }
        maxScoreText.text = maxScore.ToString();
        Pause();
    }
    public void IncreaseScore()
    {
        score++;
        scoreTemp++;
        scoreText.text = score.ToString();
    }

    private void PlayButtonSound(bool isSoundOn)
    {
        if (isSoundOn)
        {
            audioSourceButton.Play();
        }
    }

    private void OnButtonSoundClick()
    {
        isSoundOn = !isSoundOn;
        soundButtonImage.sprite = isSoundOn ? soundOnIcon : soundOffIcon;

        if (audioSource == null)
        {
            Debug.LogError("audioSource не назначен!");
            return;
        }

        audioSource.mute = !isSoundOn; // Устанавливаем mute в зависимости от состояния звука
        audioSourceButton.mute = !isSoundOn; // Устанавливаем mute для кнопок

        // Устанавливаем состояние звука в Player
        if (player != null)
        {
            player.SetSoundState(isSoundOn);
        }

        Debug.Log("Звук " + (isSoundOn ? "включен" : "выключен"));
        PlayButtonSound(isSoundOn);
    }


    private void WatchAds()
    {
        YandexGame.RewVideoShow(0);
    }

    private void OnEnable()
    {
        // Подписываемся на событие успешного просмотра рекламы
        YandexGame.RewardVideoEvent += Rewarded;
    }

    private void OnDisable()
    {
        // Отписываемся от события успешного просмотра рекламы
        YandexGame.RewardVideoEvent -= Rewarded;
        GameOver();
    }

    // Метод, который вызывается при успешном просмотре рекламы
    void Rewarded(int id)
    {
        if (id == 0) // Если ID равен 0, то выполняем вознаграждение
        {
            adsCheck = true;
            Play();
        }
    }
}
