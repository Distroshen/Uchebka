using UnityEngine;
using System.Collections;

public class GnomeMovement : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private float jumpHeight = 0.3f;
    [SerializeField] private float sideStep = 0.2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float bodyTilt = 15f;

    private float _jumpTimer;
    private bool _isJumpingLeft;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    // ������������ �������� ��� �����������
    private Transform _transform;
    private float _fourTimesJumpHeight;
    private float _inverseJumpDuration;
    private const float PI = Mathf.PI;

    // �������������� ����������� �������� ��� ��������
    private float _sideStepValue;
    private float _tiltDirection;
    private Vector3 _newPosition;


    [Header("��������� ��������")]
    public float flySpeed = 5f;     // �������� ������
    public float waitBeforeStart = 5f; // �������� ����� ������� �������� (�������)
    public float waitAtDestination = 1f; // �������� � ����� ���������� (�������)
    public float travelDistance = 10f; // ��������� �����������

    void Start()
    {


        _transform = transform;
        _startPosition = _transform.localPosition;
        _startRotation = _transform.localRotation;

        // ��������������� ����������
        _fourTimesJumpHeight = 4f * jumpHeight;
        _inverseJumpDuration = 1f / jumpDuration;

        _jumpTimer = Random.Range(0f, jumpDuration);

        // ��������������� ���������� ����� ������������ ��������
        _newPosition = new Vector3();
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("12");
        Debug.Log("123");
        StartCoroutine(BoxAnimationSequence1());

        StartCoroutine(BoxAnimationSequence());

    }

    IEnumerator BoxAnimationSequence1()
    {

        // ��� 2: ����������� ������ �� 10 ������
        float journey = 0f;
        Vector3 targetPosition = _startPosition + Vector3.back * travelDistance;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * flySpeed / travelDistance;
            transform.position = Vector3.Lerp(_startPosition, targetPosition, journey);
            yield return null;
        }
    }

    IEnumerator BoxAnimationSequence()
    {
        // ��� 3: ��������
        yield return new WaitForSeconds(waitBeforeStart);
        float journey1 = 0f;
        Vector3 targetPosition1 = _startPosition + Vector3.forward * travelDistance;

        while (journey1 <= 1f)
        {
            journey1 += Time.deltaTime * flySpeed / travelDistance;
            transform.position = Vector3.Lerp(_startPosition, targetPosition1, journey1);
            yield return null;
        }
    }


    void Update()
    {
        _jumpTimer += Time.deltaTime;

        if (_jumpTimer > jumpDuration)
        {
            _jumpTimer -= jumpDuration; // ���������� ��������� ������ ������������ 0 ��� ���������� ��������
            _isJumpingLeft = !_isJumpingLeft;
        }

        float progress = _jumpTimer * _inverseJumpDuration;

        // ��������� �������������
        ApplyTransformations(progress);
    }

    private void ApplyTransformations(float progress)
    {
        // ��������� ������ ������
        float heightProgress = progress * (1f - progress);
        float height = _fourTimesJumpHeight * heightProgress;

        // ������� ��������
        float sideMovement = progress * progress * (3f - 2f * progress); // ������������� SmoothStep
        _sideStepValue = _isJumpingLeft ? sideMovement : -sideMovement;

        // ��������� �������
        _newPosition.Set(
            _startPosition.x + _sideStepValue * sideStep,
            _startPosition.y + height,
            _startPosition.z
        );
        _transform.localPosition = _newPosition;

        // ��������� � ��������� ��������
        float tiltAmount = Mathf.Sin(progress * PI) * bodyTilt;
        _tiltDirection = _isJumpingLeft ? 1f : -1f;

        _transform.localRotation = Quaternion.Euler(
            _startRotation.eulerAngles.x,
            _startRotation.eulerAngles.y,
            _startRotation.eulerAngles.z + tiltAmount * _tiltDirection
        );
    }

    // ��� ������������ ���������� � ���������
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Vector3 startPos = transform.position;
        float fourTimesJumpHeight = 4f * jumpHeight;

        // ������ ��������� ���������� �������
        Vector3 leftPos = new Vector3();
        Vector3 rightPos = new Vector3();
        Vector3 prevLeftPos = new Vector3();
        Vector3 prevRightPos = new Vector3();

        for (int i = 1; i < 20; i++)
        {
            float progress = i / 20f;
            float prevProgress = (i - 1) / 20f;

            float height = fourTimesJumpHeight * progress * (1f - progress);
            float prevHeight = fourTimesJumpHeight * prevProgress * (1f - prevProgress);

            // �����
            leftPos.Set(
                startPos.x - progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            prevLeftPos.Set(
                startPos.x - prevProgress * sideStep,
                startPos.y + prevHeight,
                startPos.z
            );

            // ������
            rightPos.Set(
                startPos.x + progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            prevRightPos.Set(
                startPos.x + prevProgress * sideStep,
                startPos.y + prevHeight,
                startPos.z
            );

            Gizmos.DrawLine(leftPos, prevLeftPos);
            Gizmos.DrawLine(rightPos, prevRightPos);
        }
    }
}