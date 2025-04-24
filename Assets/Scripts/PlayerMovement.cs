using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float climbSpeed = 5;
    [SerializeField] float jumpForce = 5;
    [SerializeField] Vector2 deathKick = new Vector2(0, 15);
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform Gun;
    float gravityScaleAtStart;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D boxCollider;
    bool isAlive = true;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb.gravityScale;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive)
            return; // if the player is dead, stop updating
        Run();
        FlipSprite();
        climbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive)
            return;
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if(!isAlive)
            return;
        if(!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if(value.isPressed)
            Jump();
    }
    void OnFire(InputValue value)
    {
        if(!isAlive)
            return;
        Instantiate(bulletPrefab, Gun.position, Quaternion.identity);
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        animator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > Mathf.Epsilon);
    }
    void FlipSprite()
    {
        // if the player has horizontal speed
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
            // reverse the current scaling of x axis
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1);
    }
    void climbLadder()
    {
        if(!boxCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            animator.SetBool("isClimbing", false);
            rb.gravityScale = gravityScaleAtStart;
            return;
        }
        rb.gravityScale = 0;
        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }
    void Die()
    {
        if(capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("Die");
            rb.velocity = deathKick;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
