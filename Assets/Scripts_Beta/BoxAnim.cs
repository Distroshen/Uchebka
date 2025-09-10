using UnityEngine;
using System.Collections;

public class BoxAnimation : MonoBehaviour
{
    [Header("Настройки анимации")]
    public float flySpeed = 5f;     // Скорость полета
    public float waitBeforeStart = 5f; // Ожидание перед началом движения (секунды)
    public float waitAtDestination = 1f; // Ожидание в точке назначения (секунды)
    public float travelDistance = 10f; // Дистанция перемещения

    private Vector3 startPosition;  // Начальная позиция
    private bool isAnimating = false; // Флаг выполнения анимации

    void Start()
    {
        // Сохраняем начальную позицию
        startPosition = transform.position;

        // Запускаем анимацию
        StartCoroutine(BoxAnimationSequence());
    }

    IEnumerator BoxAnimationSequence()
    {
        isAnimating = true;

        // Шаг 1: Ожидание 5 секунд
        yield return new WaitForSeconds(waitBeforeStart);

        // Шаг 2: Перемещение вперед на 10 единиц
        float journey = 0f;
        Vector3 targetPosition = startPosition + Vector3.back * travelDistance;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * flySpeed / travelDistance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, journey);
            yield return null;
        }

        // Шаг 3: Ожидание 1 секунду
        yield return new WaitForSeconds(waitAtDestination);

        // Шаг 4: Мгновенное возвращение на стартовую позицию
        transform.position = startPosition;

        isAnimating = false;

        // Если нужно, чтобы анимация повторялась, раскомментируйте следующую строку:
         StartCoroutine(BoxAnimationSequence());
    }

    // Метод для принудительного запуска анимации
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(BoxAnimationSequence());
        }
    }

    // Метод для сброса в начальное положение
    public void ResetPosition()
    {
        transform.position = startPosition;
        StopAllCoroutines();
        isAnimating = false;
    }
}