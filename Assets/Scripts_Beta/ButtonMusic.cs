// Скрипт для кнопок с тегами песен
using UnityEngine;
using UnityEngine.UI;

public class SongButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            // Получаем тег из объекта кнопки
            string songTag = gameObject.tag;

            // Проверяем валидность тега
            if (!string.IsNullOrEmpty(songTag))
            {
                button.onClick.AddListener(() => SongController.Instance.ToggleSong(songTag));
            }
            else
            {
                Debug.LogError("Button tag is not set!", this);
            }
        }
    }
}