using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip; // ���������, ������� ����� �������������
    [SerializeField] private AudioSource audioSource; // ��������� ��� ���������������

    private Button playButton;

    private void Start()
    {
        // �������� ��������� ������
        playButton = GetComponent<Button>();

        // ���������, ���� �� AudioSource (���� ��� � ���������)
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // ��������� ����� �� ������� ������
        playButton.onClick.AddListener(PlayMusic);
    }

    private void PlayMusic()
    {
        if (musicClip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // �������������, ���� ��� ������
            }
            else
            {
                audioSource.clip = musicClip;
                audioSource.Play(); // ��������� ������
            }
        }
        else
        {
            Debug.LogError("No music clip assigned!");
        }
    }
}