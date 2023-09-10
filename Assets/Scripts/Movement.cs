using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2f;
    public Rigidbody2D rb;
    public Animator animator;
    public Vector2 idleDirection = Vector2.zero;

    Vector2 moveDirection = Vector2.zero;

    public LayerMask grassLayer;


    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
        {
            idleDirection.x = horizontal;
        }
        moveDirection.x = horizontal;
        if (moveDirection.y > 0.01 || moveDirection.y < -0.01)
        {
            idleDirection.x = 0;
            moveDirection.x = 0;
        }

        if (vertical != 0)
        {
            idleDirection.y = vertical;
        }
        moveDirection.y = vertical;
        if (moveDirection.x > 0.01 || moveDirection.x < -0.01)
        {
            idleDirection.y = 0;
            moveDirection.y = 0;
        }
        
        animator.SetFloat("Idle Horizontal", idleDirection.x);
        animator.SetFloat("Idle Vertical", idleDirection.y);

        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        float walkingSpeed = animator.GetFloat("Speed");
        if (walkingSpeed > 0)
        {
            CheckForEncounters();
        }
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            float encounter = Random.Range(1, 101);
            if (encounter > 50 && encounter < 52)
            {
                Debug.Log("wild pokemon");
            }
        }
    }
}
