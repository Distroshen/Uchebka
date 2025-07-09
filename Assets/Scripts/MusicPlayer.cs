using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SongController : MonoBehaviour
{
    // ������� ���������� �������
    public static SongController Instance;

    // ������� ��� �������� �����-������ �� �����
    private Dictionary<string, AudioClip> songLibrary = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;
    private string currentlyPlayingTag = "";

   void Awake()
   {
       // ������� Singleton
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

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;

        // �������� ������������� (������ ����: "Song", "Song1", "Song2")
        LoadSongs();
    }

    void LoadSongs()
    {
        // �������� ���� ����������� �� ����� Resources/Songs
        AudioClip[] songs = Resources.LoadAll<AudioClip>("Songs");

        foreach (AudioClip song in songs)
        {
            songLibrary[song.name] = song;
            Debug.Log($"Loaded song: {song.name}");
        }
    }

    public void ToggleSong(string tag)
    {
        // ���� ������������� ����� ��� ������ - ����������
        if (currentlyPlayingTag == tag)
        {
            audioSource.Stop();
            currentlyPlayingTag = "";
            return;
        }

        // ���� ����� ������� � ����������
        if (songLibrary.TryGetValue(tag, out AudioClip clip))
        {
            // ���������� ������� �����
            audioSource.Stop();

            // ������ ��������������� �����
            audioSource.clip = clip;
            audioSource.Play();
            currentlyPlayingTag = tag;
        }
        else
        {
            Debug.LogWarning($"Song not found for tag: {tag}");
        }
    }
}

