using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip; // Аудиофайл, который будет проигрываться
    [SerializeField] private AudioSource audioSource; // Компонент для воспроизведения

    private Button playButton;

    private void Start()
    {
        // Получаем компонент кнопки
        playButton = GetComponent<Button>();

        // Проверяем, есть ли AudioSource (если нет — добавляем)
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Назначаем метод на нажатие кнопки
        playButton.onClick.AddListener(PlayMusic);
    }

    private void PlayMusic()
    {
        if (musicClip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // Останавливаем, если уже играет
            }
            else
            {
                audioSource.clip = musicClip;
                audioSource.Play(); // Запускаем музыку
            }
        }
        else
        {
            Debug.LogError("No music clip assigned!");
        }
    }
}