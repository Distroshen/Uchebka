// ������ ��� ������ � ������ �����
using UnityEngine;
using UnityEngine.UI;

public class SongButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            // �������� ��� �� ������� ������
            string songTag = gameObject.tag;

            // ��������� ���������� ����
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