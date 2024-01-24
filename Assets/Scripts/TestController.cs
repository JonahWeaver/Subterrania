using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float scale = .75f;
    public bool isGrounded = false;
    private Rigidbody2D rb;

    public LayerMask groundLayer;
    //public BoxCollider groundDetector;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, .39f, groundLayer);

        // Horizontal movement
        float move = Input.GetAxis("Horizontal");
        float mult = isGrounded ? 1f : 1f;
        rb.velocity = new Vector3(move * speed, rb.velocity.y *mult, 0f);

        // Flip character sprite based on direction of movement
        if (move > 0)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(-scale, scale, scale);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        //else if(isGrounded)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, 0f);
        //}

        // Update animator parameters
        //animator.SetFloat("Speed", Mathf.Abs(move));
        //animator.SetBool("IsGrounded", isGrounded);
    }
}
