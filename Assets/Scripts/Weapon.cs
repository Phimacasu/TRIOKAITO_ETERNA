using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    private PlayerStats playerStats;
    private Transform player;

    public enum WeaponType { Melee, Bullet }
    public WeaponType weapontype;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                photonView.RPC("ApplyDamageToEnemy", RpcTarget.All, enemy.GetComponent<PhotonView>().ViewID, playerStats.attackDamage);
            }
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                photonView.RPC("ApplyDamageToEnemy", RpcTarget.All, boss.GetComponent<PhotonView>().ViewID, playerStats.attackDamage);
            }
        }
    }

    [PunRPC]
    public void ApplyDamageToEnemy(int enemyViewID, float damage)
    {
        PhotonView enemyPhotonView = PhotonView.Find(enemyViewID);
        PhotonView BossPhotonView = PhotonView.Find(enemyViewID);
        if (enemyPhotonView != null)
        {
            EnemyAI enemy = enemyPhotonView.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            BossController boss = BossPhotonView.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }
    }
}