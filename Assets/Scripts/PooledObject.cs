using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float coinProbability = 0.7f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dest"))
        {
            ObjectPoolManager.Instance.RecycleTile(gameObject);
        }
    }

    public void SpawnObjects()
    {
        foreach (Transform point in spawnPoints)
        {
            float randomValue = Random.value;

            if (randomValue < coinProbability)
            {
                GameObject coin = ObjectPoolManager.Instance.GetCoin();
                coin.transform.position = point.position;
                coin.transform.SetParent(transform);
            }
            else
            {
                GameObject bomb = ObjectPoolManager.Instance.GetBomb();
                bomb.transform.position = point.position;
                bomb.transform.SetParent(transform);
            }
        }
    }

    public void DeactivateAllObjects()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Coin"))
            {
                ObjectPoolManager.Instance.ReturnCoin(child.gameObject);
            }
            else if (child.CompareTag("Bomb"))
            {
                ObjectPoolManager.Instance.ReturnBomb(child.gameObject);
            }
        }
    }
}