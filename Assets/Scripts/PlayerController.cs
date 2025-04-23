using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody2D theRB;
    public float moveSpeed;

    public InputActionReference moveInput, actionInput;

    public Animator anim;

    public enum ToolType
    {
        plough,
        wateringCan,
        seeds,
        basket
    }
    public ToolType currentTool;

    public float toolWaitTime = 0.5f;
    private float toolWaitCounter;

    public Transform toolIndicator;
    public float toolRange = 3f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (UIController.instance != null)
        {
            UIController.instance.SwitchTool((int)currentTool);
        }
    }

    void Update()
    {
        if (toolWaitCounter > 0)
        {
            toolWaitCounter -= Time.deltaTime;
            theRB.linearVelocity = Vector2.zero;
        }
        else
        {
            theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;

            if (theRB.linearVelocity.x < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (theRB.linearVelocity.x > 0f)
            {
                transform.localScale = Vector3.one;
            }
        }

        bool hasSwitchedTool = false;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentTool++;
            if ((int)currentTool >= 4) currentTool = ToolType.plough;
            hasSwitchedTool = true;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) { currentTool = ToolType.plough; hasSwitchedTool = true; }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { currentTool = ToolType.wateringCan; hasSwitchedTool = true; }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { currentTool = ToolType.seeds; hasSwitchedTool = true; }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) { currentTool = ToolType.basket; hasSwitchedTool = true; }

        if (hasSwitchedTool)
        {
            UIController.instance.SwitchTool((int)currentTool);
        }

        anim.SetFloat("speed", theRB.linearVelocity.magnitude);

        if (GridController.instance != null)
        {
            if (actionInput.action.WasPressedThisFrame())
        {
            UseTool();
        }

        

        toolIndicator.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        toolIndicator.position = new Vector3(toolIndicator.position.x, toolIndicator.position.y, 0f);

        if (Vector3.Distance(toolIndicator.position, transform.position) > toolRange)
        {
            Vector2 direction = toolIndicator.position - transform.position;
            direction = direction.normalized * toolRange;
            toolIndicator.position = transform.position + new Vector3(direction.x, direction.y, 0f);
        }

        toolIndicator.position = new Vector3(
            Mathf.FloorToInt(toolIndicator.position.x) + 0.5f,
            Mathf.FloorToInt(toolIndicator.position.y) + 0.5f,
            0f
        );
        } else 
        {
            toolIndicator.position = new Vector3(0f, 0f, -20f);
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }

    void UseTool()
    {
        GrowBlock block = GridController.instance.GetBlock(toolIndicator.position.x - 0.5f, toolIndicator.position.y - 0.5f);
        toolWaitCounter = toolWaitTime;

        if (block != null)
        {
            switch (currentTool)
            {
                case ToolType.plough:
                    block.PloughSoil();
                    anim.SetTrigger("usePlough");
                    break;
                case ToolType.wateringCan:
                    block.WaterSoil();
                    anim.SetTrigger("useWateringcan");
                    break;
                case ToolType.seeds:
                    block.PlantCrop();
                    break;
                case ToolType.basket:
                    block.HarvestCrop();
                    break;
            }
        }
    }

    public void HardFreeze()
    {
        if (theRB != null)
        {
            theRB.linearVelocity = Vector2.zero;
            theRB.isKinematic = true;
            theRB.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (anim != null)
        {
            anim.SetFloat("speed", 0f);
            anim.speed = 0f;
        }

        if (moveInput != null) moveInput.action.Disable();
        if (actionInput != null) actionInput.action.Disable();
    }

    public void Unfreeze()
    {
        if (theRB != null)
        {
            theRB.isKinematic = false;
            theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (anim != null)
        {
            anim.speed = 1f;
        }

        if (moveInput != null) moveInput.action.Enable();
        if (actionInput != null) actionInput.action.Enable();
    }
}

