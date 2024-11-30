using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;
    private bool facingRight = true;
    public int playerhealth = 100;
    private int currenthealth;
    private Vector2 lastMovement;

    public Transform Aim;

    public Animator animator;

    void Start()
    {
        currenthealth = playerhealth;
        lastMovement = Vector2.down;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Inputs();

            
            if (movement.x < 0 && facingRight || movement.x > 0 && !facingRight)
            {
                Flip();
            }
        }
    }

    public void Inputs()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");

        movement.x = movementX;
        movement.y = movementY;

        bool isMoving = movement != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            lastMovement = movement.normalized;
        }
    }

    void FixedUpdate()
    {
     
        if (photonView.IsMine)
        {
            rb.velocity = movement * moveSpeed;

            Vector2 direction = (movement != Vector2.zero) ? movement.normalized : lastMovement;

    
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-direction.x, -direction.y, 0));
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void TakeDamage(int damage)
    {
        currenthealth -= damage;
        Debug.Log("Player Health: " + currenthealth);

        animator.SetBool("isHurt", true);

        StartCoroutine(ResetHurtState());

        if (currenthealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");

        animator.SetTrigger("Die");

        StartCoroutine(DisablePlayerAfterDeath());
    }

    IEnumerator ResetHurtState()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isHurt", false);
    }

    IEnumerator DisablePlayerAfterDeath()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
