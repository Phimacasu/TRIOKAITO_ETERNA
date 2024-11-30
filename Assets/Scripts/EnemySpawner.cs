using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    [Header("Spawner Settings")]
    public GameObject meleeEnemyPrefab;
    //public GameObject shooterEnemyPrefab; 
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public float initialSpawnInterval = 5f;
    public float minSpawnInterval = 1f;
    public float spawnAcceleration = 0.1f;
    public int enemiesPerSpawn = 3;
    public int maxTotalEnemies = 20;

    [Header("Spawn Ratios")]
    [Range(0f, 1f)]
    public float shooterSpawnChance = 0.3f;

    private float currentSpawnInterval;
    private int currentEnemyCount = 0;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        if (PhotonNetwork.IsMasterClient) 
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (currentEnemyCount < maxTotalEnemies)
            {
                SpawnEnemiesBatch();
            }

            yield return new WaitForSeconds(currentSpawnInterval);

            currentSpawnInterval = Mathf.Max(currentSpawnInterval - spawnAcceleration, minSpawnInterval);
        }
    }

    void SpawnEnemiesBatch()
    {
        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            if (currentEnemyCount >= maxTotalEnemies)
                break;

            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab = meleeEnemyPrefab;

      
        Vector2 spawnPosition = new Vector2(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );

        spawnPosition += (Vector2)transform.position;

        PhotonNetwork.Instantiate(enemyPrefab.name, spawnPosition, Quaternion.identity);

        // Increment the count of enemies
        currentEnemyCount++;
    }

    public void EnemyDestroyed()
    {
        currentEnemyCount = Mathf.Max(currentEnemyCount - 1, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
