using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterAI : MonoBehaviour
{
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public float shootingRange = 3f;
    public float fireRate = 1f;
    public int damage = 20;

    public float health, maxHealth = 3f;

    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private PlayerController playerController;
    private PlayerStats playerStats;

    public float separationRadius = 1.5f;
    public float separationForce = 2f;

    private float lastShotTime;

    private Animator animator;

    private bool isFacingRight = true;

    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        playerStats = player.GetComponent<PlayerStats>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            FacePlayer();

            if (distanceToPlayer > shootingRange)
            {

                animator.SetBool("isWalking", true);
                animator.SetBool("isShooting", false);

                Vector2 chaseDirection = (player.position - transform.position).normalized;
                Vector2 avoidance = CalculateSeparation();
                Vector2 finalDirection = chaseDirection + avoidance;
                finalDirection.Normalize();

                transform.position += (Vector3)(finalDirection * chaseSpeed * Time.deltaTime);
            }
            else
            {

                animator.SetBool("isWalking", false);
                animator.SetBool("isShooting", true);
                ShootAtPlayer();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isShooting", false);
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
        health -= dmg;
        if (health <= 0)
        {
            playerStats.GainExperience(10);
            StartCoroutine(PlayDieAnimation()); 
        }
    }

    private IEnumerator PlayDieAnimation()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject); 
    }

    private void ShootAtPlayer()
    {
        if (Time.time - lastShotTime >= fireRate)
        {
            lastShotTime = Time.time;

           
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Vector2 shootDirection = (player.position - firePoint.position).normalized;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = shootDirection * 10f;

            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg; 
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
