using UnityEngine;

public class WatermelonRoll : MonoBehaviour
{
    [Header("��������� ��������")]
    public float rotationSpeed = 180f; // �������� �������� (�������� � �������)
    private void Update()
    {
        // �������� ������ ��� X (��� ������������� �������)
        float rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation, 0f, 0f, Space.Self);
    }
}