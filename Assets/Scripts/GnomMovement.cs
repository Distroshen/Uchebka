using UnityEngine;

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
    private float _fourTimesJumpHeight; // 4f * jumpHeight
    private float _inverseJumpDuration; // 1f / jumpDuration
    private const float PI = Mathf.PI;

    void Start()
    {
        _transform = transform;
        _startPosition = _transform.localPosition;
        _startRotation = _transform.localRotation;

        // ��������������� ����������
        _fourTimesJumpHeight = 4f * jumpHeight;
        _inverseJumpDuration = 1f / jumpDuration;

        _jumpTimer = Random.Range(0f, jumpDuration);
    }

    void Update()
    {
        _jumpTimer += Time.deltaTime;

        if (_jumpTimer > jumpDuration)
        {
            _jumpTimer = 0f;
            _isJumpingLeft = !_isJumpingLeft;
        }

        float progress = _jumpTimer * _inverseJumpDuration;

        // ��������� ������������� ��������
        ApplyTransformations(progress);
    }

    private void ApplyTransformations(float progress)
    {
        // ��������� ������ ������
        float heightProgress = progress * (1f - progress);
        float height = _fourTimesJumpHeight * heightProgress;

        // ������� ��������
        float sideMovement = Mathf.SmoothStep(0f, 1f, progress);
        if (!_isJumpingLeft) sideMovement = -sideMovement;
        float xPos = _startPosition.x + sideMovement * sideStep;

        // ��������� �������
        _transform.localPosition = new Vector3(
            xPos,
            _startPosition.y + height,
            _startPosition.z
        );

        // ��������� � ��������� ��������
        float tiltAmount = Mathf.Sin(progress * PI) * bodyTilt;
        float tiltDirection = _isJumpingLeft ? 1f : -1f;

        _transform.localRotation = Quaternion.Euler(
            _startRotation.eulerAngles.x,
            _startRotation.eulerAngles.y,
            _startRotation.eulerAngles.z + tiltAmount * tiltDirection
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
        for (int i = 1; i < 20; i++)
        {
            float progress = i / 20f;
            float prevProgress = (i - 1) / 20f;

            float height = fourTimesJumpHeight * progress * (1f - progress);
            float prevHeight = fourTimesJumpHeight * prevProgress * (1f - prevProgress);

            // �����
            Vector3 leftPos = new Vector3(
                startPos.x - progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            Vector3 prevLeftPos = new Vector3(
                startPos.x - prevProgress * sideStep,
                startPos.y + prevHeight,
                startPos.z
            );

            // ������
            Vector3 rightPos = new Vector3(
                startPos.x + progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            Vector3 prevRightPos = new Vector3(
                startPos.x + prevProgress * sideStep,
                startPos.y + prevHeight,
                startPos.z
            );

            Gizmos.DrawLine(leftPos, prevLeftPos);
            Gizmos.DrawLine(rightPos, prevRightPos);
        }
    }
}