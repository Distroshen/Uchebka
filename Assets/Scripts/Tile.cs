using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>();

    // ������� ��������
    private GameObject _coin;
    private GameObject _bomb;
    private GameObject _coin5;
    private GameObject _Ya1;
    private GameObject _Ya2;
    private GameObject _bocka1;
    private GameObject _bocka2;
    private GameObject _drop;    // ��������� �������� (�����)
    private GameObject _bass;    // ��������� ����� (���-������)
    private GameObject _gnome;   // ������� ����������� (����)

    // ���������
    private float _startSpawnBomb;
    private float _timer;

    // �����������
    private const float BombChance = 30f;
    private const float YaChance = 15f;
    private const float BockaChance = 20f;
    private const float GnomeChance = 1f; // ���� ����� (����� ������)
    private const int MaxObstaclesPerTile = 2;

    // ����� ����������
    private static float _boostersChance = 10f; // ��������� ���� 10%
    private const float BoosterDecayFactor = 0.9f; // ��������� ���������� �����
    private const float MinBoosterChance = 0.5f; // ����������� ����

    // ��� ��� �������� ��������
    private Dictionary<GameObject, float> _prefabBottomOffsets = new Dictionary<GameObject, float>();

    [SerializeField] private AudioSource BombFX;
    public bool Life;

    public void Initialize(
        GameObject coin5, GameObject coin, GameObject bomb,
        GameObject Ya1, GameObject Ya2, GameObject bocka1, GameObject bocka2,
        float startSpawnBomb, float timer,
        GameObject drop, GameObject bass, GameObject gnome)
    {
        _coin5 = coin5;
        _coin = coin;
        _bomb = bomb;
        _Ya1 = Ya1;
        _Ya2 = Ya2;
        _bocka1 = bocka1;
        _bocka2 = bocka2;
        _drop = drop;
        _bass = bass;
        _gnome = gnome;
        _startSpawnBomb = startSpawnBomb;
        _timer = timer;

        List<Transform> availablePoints = new List<Transform>(_points);
        GenerateObjects(ref availablePoints);
    }

    private void GenerateObjects(ref List<Transform> points)
    {
        if (points.Count <= 2)
        {
            SpawnOneBombsMode();
        }
        else
        {
            StandardSpawning();
        }
    }

    private void SpawnOneBombsMode()
    {
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb);
        FillRemainingPoints(availablePoints);
    }

    private void StandardSpawning()
    {
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObstacles(ref availablePoints);
        FillRemainingPoints(availablePoints);
    }

    private void SpawnObstacles(ref List<Transform> points)
    {
        if (points.Count == 0) return;

        float obstacleChance = _timer < _startSpawnBomb ? 0 :
            Mathf.Clamp(20f + (_timer / 2f), 0f, 50f);

        int obstaclesToSpawn = Mathf.Min(MaxObstaclesPerTile, points.Count);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            if (Random.Range(0f, 100f) < obstacleChance)
            {
                GameObject obstaclePrefab = GetRandomObstacle();
                SpawnObjectAtRandomPoint(ref points, obstaclePrefab);
            }
            obstacleChance *= 0.5f;
        }
    }

    private GameObject GetRandomObstacle()
    {
        float randomValue = Random.Range(0f, 100f);
        float currentChance = BombChance;

        if (randomValue < BombChance) return _bomb;
        currentChance += YaChance;
        if (randomValue < currentChance) return _Ya1;
        currentChance += YaChance;
        if (randomValue < currentChance) return _Ya2;
        currentChance += BockaChance;
        if (randomValue < currentChance) return _bocka1;
        currentChance += GnomeChance;
        if (randomValue < currentChance) return _gnome;

        return _bocka2;
    }

    private void FillRemainingPoints(List<Transform> points)
    {
        if (points.Count == 0) return;

        var rows = points
            .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var row in rows)
        {
            var randomPoint = row.OrderBy(x => Random.value).FirstOrDefault();
            if (randomPoint != null)
            {
                // �������� �� ����� ��������� ������ ������
                if (_timer > 10f && Random.Range(0f, 100f) < _boostersChance)
                {
                    GameObject booster = Random.Range(0, 2) == 0 ? _drop : _bass;
                    SpawnCoin(booster, randomPoint.position);
                    _boostersChance = Mathf.Max(MinBoosterChance, _boostersChance * BoosterDecayFactor);
                }
                else
                {
                    GameObject coinPrefab = ShouldSpawnCoin5() ? _coin5 : _coin;
                    SpawnCoin(coinPrefab, randomPoint.position);
                }
            }
        }
    }

    private void SpawnCoin(GameObject coinPrefab, Vector3 position)
    {
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity, transform);
        AdjustObjectPosition(coin, position.y);
    }

    private void SpawnObjectAtRandomPoint(ref List<Transform> points, GameObject prefab)
    {
        if (points.Count == 0) return;

        Transform point = null;
        //bool isGnomeSpecialCase = false;

        // ��������� ����� (������� 3 �����)
        if (prefab == _gnome && points.Count >= 3)
        {
            // ���������� ����� �� Z-����������
            var groups = points
                .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f)
                .Where(g => g.Count() >= 3)
                .ToList();

            if (groups.Count > 0)
            {
                // �������� ��������� ������
                var group = groups[Random.Range(0, groups.Count)];
                // ��������� ����� �� X
                var sortedPoints = group.OrderBy(p => p.position.x).ToList();

                // ���� ��� ���������������� �����
                for (int i = 0; i < sortedPoints.Count - 2; i++)
                {
                    float diff1 = sortedPoints[i + 1].position.x - sortedPoints[i].position.x;
                    float diff2 = sortedPoints[i + 2].position.x - sortedPoints[i + 1].position.x;

                    // ��������� ����������� �������������
                    if (Mathf.Abs(diff1 - diff2) < 0.1f && diff1 > 0.1f)
                    {
                        // ����� ���������� �����
                        Transform leftPoint = sortedPoints[i];
                        point = sortedPoints[i + 1]; // ���� �� ������
                        Transform rightPoint = sortedPoints[i + 2];

                        // ������� ����� ����� (������)
                        points.Remove(leftPoint);
                        // ������� ����������� ����� (����)
                        points.Remove(point);
                        // ������ ����� ������� ��� ������/���������

                        //isGnomeSpecialCase = true;..�
                        break;
                    }
                }
            }
        }

        // ���� ���� �� ��������� ������ �������� - �������� ��������� �����
        if (point == null)
        {
            if (prefab == _gnome)
            {
                // ���� ��� ����� �� ������� ����� - �������� �� �����
                prefab = _bomb;
            }

            int randomIndex = Random.Range(0, points.Count);
            point = points[randomIndex];
            points.RemoveAt(randomIndex);
        }

        GameObject spawnedObject = Instantiate(prefab, point.position, Quaternion.identity, transform);
        ApplyRandomRotation(spawnedObject);
        AdjustObjectPosition(spawnedObject, point.position.y);
    }

    private void ApplyRandomRotation(GameObject spawnedObject)
    {
        if (spawnedObject.CompareTag("�������"))
        {
            bool shouldRotate = Random.Range(0, 2) == 1;
            if (shouldRotate)
            {
                spawnedObject.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
        }
    }

    private void AdjustObjectPosition(GameObject obj, float groundY)
    {
        float bottomOffset = GetBottomOffsetForPrefab(obj);
        Vector3 newPosition = obj.transform.position;

        // ������ ������������ ��� ������ ��������
        float reductionFactor = 0.2f; // �� ���������

        if (obj.CompareTag("�������"))
        {
            reductionFactor = 2f;
        }
        else if (obj == _bomb)
        {
            reductionFactor = 0.3f;
        }
        else if (obj == _drop || obj == _bass)
        {
            reductionFactor = 0.4f;
        }
        else if (obj == _gnome)
        {
            reductionFactor = 0.35f;
        }

        newPosition.y = groundY + bottomOffset * reductionFactor;
        obj.transform.position = newPosition;
    }

    private float GetBottomOffsetForPrefab(GameObject prefab)
    {
        // ���� �������� ��� � ���� - ���������� ���
        if (_prefabBottomOffsets.ContainsKey(prefab))
        {
            return _prefabBottomOffsets[prefab];
        }

        // ������� ��������� ��������� ��� ����������
        GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        temp.SetActive(false);

        float bottomOffset = CalculateBottomOffset(temp);

        Destroy(temp);

        // �������� ���������
        _prefabBottomOffsets[prefab] = bottomOffset;
        return bottomOffset;
    }

    private float CalculateBottomOffset(GameObject obj)
    {
        // ������� �������� ���������
        Collider col = obj.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            return bounds.size.y / 2f;
        }

        // ���� ���������� ���, ������� �������� ��������
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            return bounds.size.y / 2f;
        }

        // ���� ��� �� ����������, �� ���������
        Debug.LogWarning($"No Collider or Renderer found on {obj.name}");
        return 0f;
    }

    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f;
    }

    public float GetEndPosition()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.max.z;
        }
        return transform.position.z + transform.localScale.z / 2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerTrigger();
        }
    }

    private void HandlePlayerTrigger()
    {
        BombFX.Play();
        this.gameObject.SetActive(false);
        Life = false;
        FindAnyObjectByType<BustB>().B1();
    }

    // ����������� ����� ��� ������ ����� ����������
    public static void ResetBoostersChance()
    {
        _boostersChance = 10f;
    }
}