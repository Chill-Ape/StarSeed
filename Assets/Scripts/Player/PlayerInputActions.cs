using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 1f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeHeight = 0.5f; // How high the player jumps during dodge

    [Header("Block Settings")]
    [SerializeField] private float blockDuration = 0.5f;
    [SerializeField] private float damageReduction = 0.75f; // 75% damage reduction
    [SerializeField] private ParticleSystem blockEffect; // Drag your block animation here

    private InputAction clickAttackAction;
    private InputAction blockAction;
    private InputAction dodgeAction;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        // Create the input actions
        clickAttackAction = new InputAction("ClickAttack", binding: "<Mouse>/leftButton");
        blockAction = new InputAction("Block", binding: "<Keyboard>/b");
        dodgeAction = new InputAction("Dodge", binding: "<Keyboard>/n");

        playerAttack = GetComponent<PlayerAttack>();
        
        // Pass the settings to PlayerAttack
        playerAttack.InitializeDodgeSettings(dodgeDistance, dodgeDuration, dodgeHeight);
        playerAttack.InitializeBlockSettings(blockDuration, damageReduction, blockEffect);
    }

    private void OnEnable()
    {
        // Enable the actions
        clickAttackAction.Enable();
        blockAction.Enable();
        dodgeAction.Enable();

        // Subscribe to the actions
        clickAttackAction.performed += ctx => playerAttack.Attack();
        blockAction.performed += ctx => playerAttack.Block();
        dodgeAction.performed += ctx => playerAttack.Dodge();
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        clickAttackAction.performed -= ctx => playerAttack.Attack();
        blockAction.performed -= ctx => playerAttack.Block();
        dodgeAction.performed -= ctx => playerAttack.Dodge();

        // Disable the actions
        clickAttackAction.Disable();
        blockAction.Disable();
        dodgeAction.Disable();
    }
} 