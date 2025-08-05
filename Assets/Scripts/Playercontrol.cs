using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player speed; // ����� � ���������� Speed

    [Header("��������� ��������")]
    [SerializeField] private float _variation = 2f;    // ������� ��������
    [SerializeField] private float _frequency = 0.5f;  // ������� ���������

    private float _speedTimer;
    private float _currentSpeed;
    private bool _isFinish; // ���� ��� �������� ����

    void Start()
    {
        _currentSpeed = speed.Speed;
        _speedTimer = Random.Range(0f, 10f);
        _isFinish = gameObject.CompareTag("Finish"); // ��������� ��� ��� ������
    }

    void FixedUpdate()
    {
        if (_isFinish)
        {
            // ��� �������� � ����� "Finish" - �������� � �����������
            _speedTimer += Time.fixedDeltaTime * _frequency;
            _currentSpeed = speed.Speed + Mathf.Sin(_speedTimer) * _variation;
        }
        else
        {
            // ��� ��������� �������� - ���������� ��������
            _currentSpeed = speed.Speed;
        }

        // ��������� ��������
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * _currentSpeed, Space.World);
    }
}