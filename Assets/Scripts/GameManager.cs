using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;    
    void Start()
    {
        
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected or PlayerPrefab is not set!");
        }
    }
}
