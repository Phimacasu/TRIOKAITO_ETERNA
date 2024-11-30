using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private PlayerController playerController;

    public int damage = 5;

    void Start()
    {
        
        Destroy(gameObject, 4f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.CompareTag("Player") && playerController != null)
        {
            playerController.TakeDamage(damage); 
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall")) 
        {
            Destroy(gameObject);
        }
    }
}
