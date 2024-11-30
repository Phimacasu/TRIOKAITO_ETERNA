using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SmoothCameraFollow : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float dampening;

    private Transform target;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    private IEnumerator FindLocalPlayer()
    {

        while (target == null)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    target = player.transform;
                    break;
                }
            }
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, dampening);
    }
}