using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnInfo
    {
        public GameObject enemyPrefab;
        public int amountToSpawn;
    }

    public SpawnInfo[] enemiesToSpawn;
    public Transform[] spawnPoints;
    public float spawnDelay = 1.0f;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private System.Collections.IEnumerator SpawnEnemies()
    {
        foreach (var spawnInfo in enemiesToSpawn)
        {
            for (int i = 0; i < spawnInfo.amountToSpawn; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(spawnInfo.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}