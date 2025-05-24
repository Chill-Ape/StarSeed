using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration = 50f; // Add acceleration for smoother movement

    public Vector2 MoveDirection => moveDirection;
    
    private PlayerAnimation playerAnimations;
    private PlayerActions actions;
    private Player player;
    private Rigidbody2D rb2D;
    private Vector2 moveDirection;
    private Vector2 currentVelocity;
    
    public bool IsMoving { get; private set; }
    
    private void Awake()
    {
        player = GetComponent<Player>();
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimations = GetComponent<PlayerAnimation>();
        
        // Configure Rigidbody2D for better movement
        rb2D.gravityScale = 0f;
        rb2D.linearDamping = 0f;
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    
    private void Update()
    {
        ReadMovement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (player.Stats.Health <= 0) return;
        
        // Smoothly interpolate to target velocity
        Vector2 targetVelocity = moveDirection * speed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        
        // Apply movement
        rb2D.linearVelocity = currentVelocity;
    }
    
    private void ReadMovement()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
        IsMoving = moveDirection != Vector2.zero;
        playerAnimations.SetMoveBoolTransition(IsMoving);
    }
    
    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
        // Reset velocity when disabled
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            currentVelocity = Vector2.zero;
        }
    }

    private void UpdateAnimation()
    {
        playerAnimations.SetMoveBoolTransition(IsMoving);
    }

    private void UpdateMovement()
    {
        // ... other code ...
        IsMoving = moveDirection != Vector2.zero; // Set the IsMoving property
        // ... other code ...
    }
}
