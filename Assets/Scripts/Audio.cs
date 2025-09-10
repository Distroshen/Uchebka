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
    public AudioClip defaultMusic; // ������ �� ���������
    public AudioClip[] additionalMusic; // �������������� �����...

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
     public AudioClip newClip3;
    public AudioClip newClip4;
    public AudioClip newClip5;
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
        // ��������� ��������� ��������� ���� ��� ���������� �� ���������
        string savedClipName = PlayerPrefs.GetString("CurrentMusicClip", "");
        AudioClip savedClip = null;

        if (!string.IsNullOrEmpty(savedClipName))
        {
            // ����� ������������ ����� ����� ���������
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

    // ����� ��� ��������� ������ �� ������ ��������
    public void ChangeMusic ()
    {
        Debug.Log("����");
        if (newClip == null) return;
        Debug.Log("��������");
        StartCoroutine(SmoothChangeMusic());
    }

    public void ChangeMusic1()
    {
        Debug.Log("����1");
        if (newClip1 == null) return;
        Debug.Log("��������1");
        StartCoroutine(SmoothChangeMusic1());
    }

    public void ChangeMusic2()
    {
        Debug.Log("����2");
        if (newClip2 == null) return;
        Debug.Log("��������2");
        StartCoroutine(SmoothChangeMusic2());
    }
    public void ChangeMusic3()
    {
        Debug.Log("����2");
        if (newClip3 == null) return;
        Debug.Log("��������2");
        StartCoroutine(SmoothChangeMusic3());
    }
    public void ChangeMusic4()
    {
        Debug.Log("����2");
        if (newClip4 == null) return;
        Debug.Log("��������2");
        StartCoroutine(SmoothChangeMusic4());
    }
    public void ChangeMusic5()
    {
        Debug.Log("����2");
        if (newClip5 == null) return;
        Debug.Log("��������2");
        StartCoroutine(SmoothChangeMusic5());
    }

    private IEnumerator SmoothChangeMusic()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip1;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
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

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
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

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip2;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }

    private IEnumerator SmoothChangeMusic3()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip3;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }

    private IEnumerator SmoothChangeMusic4()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip4;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        isChangingTrack = false;
    }


    private IEnumerator SmoothChangeMusic5()
    {
        isChangingTrack = true;
        float startVolume = musicSource.volume;

        // ������� ���������� ���������
        while (musicSource.volume > minVolume)
        {
            musicSource.volume -= Time.deltaTime * volumeChangeSpeed;
            yield return null;
        }

        // ����� �����
        currentClip = newClip5;
        musicSource.clip = currentClip;
        musicSource.Play();

        // ��������� ����� ������
        PlayerPrefs.SetString("CurrentMusicClip", currentClip.name);
        PlayerPrefs.Save();

        // ������� �������������� ���������
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

    // ���������� ������ ����� ��� ����� �����
    public void UpdateSoundButton(Image newButtonImage)
    {
        soundButtonImage = newButtonImage;
        UpdateButtonImage();
    }
}