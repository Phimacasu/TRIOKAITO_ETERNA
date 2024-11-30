using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Attack : MonoBehaviourPunCallbacks
{
    public GameObject melee;
    bool isAttacking = false;
    public float attackduration = 0.3f;
    public float attacktimer = 0f;

    public Transform Aim;
    public GameObject bullet;
    public float fireforce = 10f;
    public float shootcd = 0.25f;
    public float shootTimer = 0.5f;

    public Animator animator;

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckMeleeTimer();
            shootTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                OnAttack();
            }

            if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
            {
                OnShoot();
            }
        }
    }

    void OnShoot()
    {
        if (shootTimer > shootcd)
        {
            shootTimer = 0f;

            GameObject IntBullet = PhotonNetwork.Instantiate(bullet.name, Aim.position, Aim.rotation);
            float angle = Mathf.Atan2(-Aim.up.y, -Aim.up.x) * Mathf.Rad2Deg;
            IntBullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            IntBullet.GetComponent<Rigidbody2D>().AddForce(-Aim.up * fireforce, ForceMode2D.Impulse);

            animator.SetTrigger("RangedAttack");
        }
    }

    void OnAttack()
    {
        if (!isAttacking)
        {
            melee.SetActive(true); 
            isAttacking = true;

            animator.SetTrigger("Attack");

            photonView.RPC("SyncAttack", RpcTarget.All);
        }
    }

    [PunRPC]
    void SyncAttack()
    {
        melee.SetActive(true); 
        isAttacking = true;
        attacktimer = 0f;
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            attacktimer += Time.deltaTime;
            if (attacktimer >= attackduration)
            {
                attacktimer = 0f;
                isAttacking = false;
                melee.SetActive(false);
            }
        }
    }
}
