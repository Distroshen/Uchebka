using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [Header("Sound Settings")]
    [Range(0f, 1f)] public float maxVolume = 1f;
    [Range(0f, 1f)] public float minVolume = 0.1f; // Минимальная громкость (не 0)
    public float volumeChangeSpeed = 5f;

    [Header("UI Settings")]
    public Image soundButtonImage;
    public Sprite soundOnSprite; // Иконка "Звук есть"
    public Sprite soundOffSprite; // Иконка "Звук выключен"

    private AudioSource musicSource;
    private bool isSoundOn = true; // Всегда true при старте
    private float targetVolume;

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
        // 1. Всегда играем музыку при старте
        musicSource.Play();

        // 2. Громкость на максимум (даже если в настройках было выключено)
        targetVolume = maxVolume;
        musicSource.volume = maxVolume;

        // 3. Принудительно включаем звук и обновляем иконку
        isSoundOn = true;
        UpdateButtonImage();
    }

    void Update()
    {
        // Плавное изменение громкости
        if (!Mathf.Approximately(musicSource.volume, targetVolume))
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume,
                                           Time.deltaTime * volumeChangeSpeed);
        }
    }

    private void LoadSettings()
    {
        // Загружаем настройки, но игнорируем их для музыки
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
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
            // Всегда синхронизируем иконку с isSoundOn
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void OnSoundButtonClick()
    {
        ToggleSound();
    }
}