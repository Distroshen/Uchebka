using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [Header("Sound Settings")]
    [Range(0f, 1f)] public float maxVolume = 1f;
    [Range(0f, 1f)] public float minVolume = 0.1f;
    public float volumeChangeSpeed = 5f;

    [Header("Music Clips")]
    public AudioClip defaultMusic; // Музыка по умолчанию
    public AudioClip[] additionalMusic; // Дополнительные треки...

    [Header("UI Settings")]
    public Image soundButtonImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private AudioSource musicSource;
    private bool isSoundOn = true;
    private float targetVolume;
    private AudioClip currentClip;
    public AudioClip newClip;
    public AudioClip newClip1;
    public AudioClip newClip2;
    private bool isChangingTrack = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = GetComponent<AudioSource>();
        LoadSettings();
    }

    void Start()
    {
        // Загружаем последний выбранный трек или используем по умолчанию
        string savedClipName = PlayerPrefs.GetString("CurrentMusicClip", "");
        AudioClip savedClip = null;

        if (!string.IsNullOrEmpty(savedClipName))
        {
            // Поиск сохраненного клипа среди доступных
            foreach (var clip in additionalMusic)
            {
                if (clip.name == savedClipName)
                {
                    savedClip = clip;
                    break;
                }
            }
        }

        currentClip = savedClip != null ? savedClip : defaultMusic;
        PlayCurrentMusic();
    }

    private void PlayCurrentMusic()
    {
        if (currentClip == null) return;

        musicSource.clip = currentClip;
        musicSource.Play();
        targetVolume = isSoundOn ? maxVolume : minVolume;
        musicSource.volume = targetVolume;
    }

    void Update()
    {
        if (!Mathf.Approximately(musicSource.volume, targetVolume) && !isChangingTrack)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume,
                                           Time.deltaTime * volumeChangeSpeed);
        }
    }

    private void LoadSettings()
    {
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Метод для изменения музыки из других скриптов
    public void ChangeMusic ()
    {
        Debug.Log("Сраб");
        if (newClip == null) return;
        Debug.Log("Сработао");
        StartCoroutine(SmoothChangeMusic());
    }

    public void ChangeMusic1()
    {
        Debug.Log("Сраб1");
        if (newClip1 == null) return;
        Debug.Log("Сработао1");
        StartCoroutine(SmoothChangeMusic1());
    }

    public void ChangeMusic2()
    {
        Debug.Log("Сраб2");
        if (newClip2 == null) return;
        Debug.Log("Сработао2");
        StartCoroutine(SmoothChangeMusic2());
    }

    private IEnumerator SmoothChangeMusic()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // Плавное уменьшение громкости
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // Смена трека
        currentClip = newClip1;
        musicSource.clip = currentClip;
        musicSource.Play();

        // Сохраняем выбор музыки
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // Плавное восстановление громкости
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }

    private IEnumerator SmoothChangeMusic1()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // Плавное уменьшение громкости
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // Смена трека
        currentClip = newClip;
        musicSource.clip = currentClip;
        musicSource.Play();

        // Сохраняем выбор музыки
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // Плавное восстановление громкости
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }

    private IEnumerator SmoothChangeMusic2()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // Плавное уменьшение громкости
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // Смена трека
        currentClip = newClip2;
        musicSource.clip = currentClip;
        musicSource.Play();

        // Сохраняем выбор музыки
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // Плавное восстановление громкости
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        targetVolume = isSoundOn ? maxVolume : minVolume;
        UpdateButtonImage();
        SaveSettings();
    }

    private void UpdateButtonImage()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void OnSoundButtonClick()
    {
        ToggleSound();
    }

    // Обновление кнопки звука при смене сцены
    public void UpdateSoundButton(Image newButtonImage)
    {
        soundButtonImage = newButtonImage;
        UpdateButtonImage();
    }
}