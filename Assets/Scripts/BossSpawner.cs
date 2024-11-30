using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BossSpawner : MonoBehaviourPunCallbacks
{
    public GameObject prefabToSpawn;
    public float spawnDelay = 60f;
    public Transform spawnPoint;

    void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnPrefabAfterDelay());
        }
    }

    IEnumerator SpawnPrefabAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
    }
}