using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 2f;
    public Rigidbody2D rb;
    public Animator animator;
    public Vector2 idleDirection = Vector2.zero;
    public bool IsMovingEnabled { get; set; }
    Vector2 moveDirection = Vector2.zero;
    public InputAction playerControls;

    private InputAction move;
    private InputAction interact;

    public LayerMask grassLayer;

    public event Action OnEncounter;

    void Start()
    {
        IsMovingEnabled = true;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleUpdate()
    {
        if (IsMovingEnabled)
        {
            //float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");
            moveDirection = playerControls.ReadValue<Vector2>();

            if(moveDirection.sqrMagnitude > 0)
            {
                idleDirection = playerControls.ReadValue<Vector2>();
                if (moveDirection.x != 0) moveDirection.y = 0;
                if (moveDirection.y != 0) moveDirection.x = 0;
            }

            animator.SetFloat("Idle Horizontal", idleDirection.x);
            animator.SetFloat("Idle Vertical", idleDirection.y);

            animator.SetFloat("Horizontal", moveDirection.x);
            animator.SetFloat("Vertical", moveDirection.y);
            animator.SetFloat("Speed", moveDirection.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        if (IsMovingEnabled)
        {
            float walkingSpeed = animator.GetFloat("Speed");
            if (walkingSpeed > 0)
            {
                CheckForEncounters();
            }
            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        }
        else
        {
            moveDirection.x = 0;
            moveDirection.y = 0;
            animator.SetFloat("Speed", 0);
        }
       
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            float encounter = UnityEngine.Random.Range(1, 101);
            if (encounter < 3)
            {
                OnEncounter();
            }
        }
    }
}
