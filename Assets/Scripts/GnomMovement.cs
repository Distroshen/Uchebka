using UnityEngine;

public class GnomeMovement : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private float jumpHeight = 0.3f;        // ������ ������
    [SerializeField] private float sideStep = 0.2f;          // ������ ���� � �������
    [SerializeField] private float jumpDuration = 0.5f;      // ����������������� ������ ������
    [SerializeField] private float bodyTilt = 15f;           // ������ ���� ��� ������

    private float _jumpTimer;
    private bool _isJumpingLeft;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _jumpTimer = Random.Range(0f, jumpDuration); // ��������� ��������� ����
    }

    void Update()
    {
        // ��������� ������ ������
        _jumpTimer += Time.deltaTime;

        // ���������� ������ ��� ���������� �����
        if (_jumpTimer > jumpDuration)
        {
            _jumpTimer = 0f;
            _isJumpingLeft = !_isJumpingLeft; // ������ �����������
        }

        // ��������� �������� ������ (0-1)
        float progress = _jumpTimer / jumpDuration;

        // ������������ ������� � ��������
        Vector3 newPosition = CalculateJumpPosition(progress);
        Quaternion newRotation = CalculateBodyTilt(progress);

        // ��������� �������������
        transform.localPosition = newPosition;
        transform.localRotation = newRotation;
    }

    private Vector3 CalculateJumpPosition(float progress)
    {
        // �������������� ���������� ������ (y = 4 * h * x * (1 - x))
        float height = 4f * jumpHeight * progress * (1f - progress);

        // ������� �������� (� ������� ������� � ������)
        float sideMovement = Mathf.SmoothStep(0f, 1f, progress);
        if (!_isJumpingLeft) sideMovement = -sideMovement;
        float xPos = _startPosition.x + sideMovement * sideStep;

        return new Vector3(
            xPos,
            _startPosition.y + height,
            _startPosition.z
        );
    }

    private Quaternion CalculateBodyTilt(float progress)
    {
        // ������������ ������ � �������� ������
        float tiltAmount = Mathf.Sin(progress * Mathf.PI) * bodyTilt;

        // ����������� ������� ������� �� ����������� ������
        float tiltDirection = _isJumpingLeft ? 1f : -1f;

        return Quaternion.Euler(
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

        // ������ ��������� ���������� �������
        for (int i = 0; i < 20; i++)
        {
            float progress = i / 20f;
            float height = 4f * jumpHeight * progress * (1f - progress);

            // �����
            Vector3 leftPos = new Vector3(
                startPos.x - progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            // ������
            Vector3 rightPos = new Vector3(
                startPos.x + progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            if (i > 0)
            {
                Gizmos.DrawLine(leftPos, new Vector3(
                    startPos.x - (i - 1) / 20f * sideStep,
                    startPos.y + 4f * jumpHeight * (i - 1) / 20f * (1f - (i - 1) / 20f),
                    startPos.z
                ));

                Gizmos.DrawLine(rightPos, new Vector3(
                    startPos.x + (i - 1) / 20f * sideStep,
                    startPos.y + 4f * jumpHeight * (i - 1) / 20f * (1f - (i - 1) / 20f),
                    startPos.z
                ));
            }
        }
    }
}