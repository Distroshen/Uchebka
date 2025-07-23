using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [Header("Sound Settings")]
    [Range(0f, 1f)] public float maxVolume = 1f;
    [Range(0f, 1f)] public float minVolume = 0.1f; // ����������� ��������� (�� 0)
    public float volumeChangeSpeed = 5f;

    [Header("UI Settings")]
    public Image soundButtonImage;
    public Sprite soundOnSprite; // ������ "���� ����"
    public Sprite soundOffSprite; // ������ "���� ��������"

    private AudioSource musicSource;
    private bool isSoundOn = true; // ������ true ��� ������
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
        // 1. ������ ������ ������ ��� ������
        musicSource.Play();

        // 2. ��������� �� �������� (���� ���� � ���������� ���� ���������)
        targetVolume = maxVolume;
        musicSource.volume = maxVolume;

        // 3. ������������� �������� ���� � ��������� ������
        isSoundOn = true;
        UpdateButtonImage();
    }

    void Update()
    {
        // ������� ��������� ���������
        if (!Mathf.Approximately(musicSource.volume, targetVolume))
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume,
                                           Time.deltaTime * volumeChangeSpeed);
        }
    }

    private void LoadSettings()
    {
        // ��������� ���������, �� ���������� �� ��� ������
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
            // ������ �������������� ������ � isSoundOn
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void OnSoundButtonClick()
    {
        ToggleSound();
    }
}