using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SongController : MonoBehaviour
{
    // Система управления песнями
    public static SongController Instance;

    // Словарь для хранения аудио-клипов по тегам
    private Dictionary<string, AudioClip> songLibrary = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;
    private string currentlyPlayingTag = "";

   void Awake()
   {
       // Паттерн Singleton
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

        // Загрузка аудиоресурсов (пример имен: "Song", "Song1", "Song2")
        LoadSongs();
    }

    void LoadSongs()
    {
        // Загрузка всех аудиоклипов из папки Resources/Songs
        AudioClip[] songs = Resources.LoadAll<AudioClip>("Songs");

        foreach (AudioClip song in songs)
        {
            songLibrary[song.name] = song;
            Debug.Log($"Loaded song: {song.name}");
        }
    }

    public void ToggleSong(string tag)
    {
        // Если запрашиваемая песня уже играет - остановить
        if (currentlyPlayingTag == tag)
        {
            audioSource.Stop();
            currentlyPlayingTag = "";
            return;
        }

        // Если песня найдена в библиотеке
        if (songLibrary.TryGetValue(tag, out AudioClip clip))
        {
            // Остановить текущую песню
            audioSource.Stop();

            // Начать воспроизведение новой
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

