using UnityEngine;
using System.Collections;

public class BoxAnimation : MonoBehaviour
{
    [Header("��������� ��������")]
    public float flySpeed = 5f;     // �������� ������
    public float waitBeforeStart = 5f; // �������� ����� ������� �������� (�������)
    public float waitAtDestination = 1f; // �������� � ����� ���������� (�������)
    public float travelDistance = 10f; // ��������� �����������

    private Vector3 startPosition;  // ��������� �������
    private bool isAnimating = false; // ���� ���������� ��������

    void Start()
    {
        // ��������� ��������� �������
        startPosition = transform.position;

        // ��������� ��������
        StartCoroutine(BoxAnimationSequence());
    }

    IEnumerator BoxAnimationSequence()
    {
        isAnimating = true;

        // ��� 1: �������� 5 ������
        yield return new WaitForSeconds(waitBeforeStart);

        // ��� 2: ����������� ������ �� 10 ������
        float journey = 0f;
        Vector3 targetPosition = startPosition + Vector3.back * travelDistance;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * flySpeed / travelDistance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, journey);
            yield return null;
        }

        // ��� 3: �������� 1 �������
        yield return new WaitForSeconds(waitAtDestination);

        // ��� 4: ���������� ����������� �� ��������� �������
        transform.position = startPosition;

        isAnimating = false;

        // ���� �����, ����� �������� �����������, ���������������� ��������� ������:
         StartCoroutine(BoxAnimationSequence());
    }

    // ����� ��� ��������������� ������� ��������
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(BoxAnimationSequence());
        }
    }

    // ����� ��� ������ � ��������� ���������
    public void ResetPosition()
    {
        transform.position = startPosition;
        StopAllCoroutines();
        isAnimating = false;
    }
}