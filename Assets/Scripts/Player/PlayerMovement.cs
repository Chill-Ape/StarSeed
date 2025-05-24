using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    public Vector2 MoveDirection => moveDirection;
    
    private PlayerAnimation playerAnimations;
    private PlayerActions actions;
    private Player player;
    private Rigidbody2D rb2D;
    private Vector2 moveDirection;
    
    public bool IsMoving { get; private set; }
    
    private void Awake()
    {
        player = GetComponent<Player>();
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimations = GetComponent<PlayerAnimation>();
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
        rb2D.MovePosition(rb2D.position + moveDirection * (speed * Time.fixedDeltaTime));
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
