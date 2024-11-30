using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BossController : MonoBehaviourPunCallbacks
{
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public int damage = 20;

    public float health, maxHealth = 3f;

    private Transform player;
    private PlayerController playerController;
    private PlayerStats playerStats;

    public float separationRadius = 1.5f;
    public float separationForce = 2f;

    private bool isFacingRight = true;
    private Animator animator;

    public float speedUpFactor = 2f;
    private bool isSpeedingUp = false;

    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        playerStats = player.GetComponent<PlayerStats>();

        animator = GetComponent<Animator>();

        StartCoroutine(RandomSpeedUp());
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Player[] players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player != photonView.Owner)
            {
                Transform otherPlayer = GameObject.FindGameObjectWithTag("Player").transform;
                if (otherPlayer != null)
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, otherPlayer.position);
                    if (distanceToPlayer <= detectionRange)
                    {
                        FacePlayer(otherPlayer);

                        Vector2 chaseDirection = (otherPlayer.position - transform.position).normalized;

                        Vector2 avoidance = CalculateSeparation();
                        Vector2 finalDirection = chaseDirection + avoidance;

                        finalDirection.Normalize();

                        transform.position += (Vector3)(finalDirection * chaseSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.TakeDamage(damage);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (photonView.IsMine)
        {
            health -= dmg;
            photonView.RPC("SyncHealth", RpcTarget.All, health);

            if (health <= 0)
            {
                playerStats.GainExperience(10);
                StartCoroutine(PlayDieAnimation());
            }
        }
    }

    private Vector2 CalculateSeparation()
    {
        Vector2 separationVector = Vector2.zero;
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject && enemy.CompareTag("Enemy"))
            {
                Vector2 directionAway = (Vector2)(transform.position - enemy.transform.position);
                separationVector += directionAway.normalized / directionAway.magnitude;
            }
        }

        return separationVector * separationForce;
    }

    private void FacePlayer(Transform otherPlayer)
    {
        if (otherPlayer.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (otherPlayer.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private IEnumerator PlayDieAnimation()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1f);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void SyncHealth(float syncedHealth)
    {
        health = syncedHealth;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    private IEnumerator RandomSpeedUp()
    {
        while (true)
        {

            float waitTimeBeforeSpeedUp = Random.Range(5f, 15f);
            yield return new WaitForSeconds(waitTimeBeforeSpeedUp);

            if (!isSpeedingUp)
            {

                float speedUpDuration = Random.Range(3f, 7f);
                chaseSpeed *= speedUpFactor;
                isSpeedingUp = true;

                yield return new WaitForSeconds(speedUpDuration);

                chaseSpeed /= speedUpFactor;
                isSpeedingUp = false;
            }
        }
    }
}
