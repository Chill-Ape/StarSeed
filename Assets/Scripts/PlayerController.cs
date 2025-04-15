using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
        
    }
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

    public float toolWaitTime = .5f;
    private float toolWaitCounter;

    public Transform toolIndicator;
    public float toolRange = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.instance.SwitchTool((int)currentTool);
    }

    // Update is called once per frame
    void Update()
    {

        if(toolWaitCounter > 0)
        {
            toolWaitCounter -= Time.deltaTime;
            theRB.linearVelocity = Vector2.zero;

        }
        else
        {
              // theRB.linearVelocity = new Vector2(moveSpeed, 0f);
            theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;

            if(theRB.linearVelocity.x <0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if(theRB.linearVelocity.x >0f)
            {
                transform.localScale = Vector3.one;
            }
        }
      
     

        bool hasSwitchedTool = false;

        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentTool ++;

            if((int)currentTool >= 4)
            {
                currentTool = ToolType.plough;
            }
            hasSwitchedTool = true;
        }

        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentTool = ToolType.plough;
            hasSwitchedTool = true;
        }

        if(Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentTool = ToolType.wateringCan;
            hasSwitchedTool = true;
        }

         if(Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentTool = ToolType.seeds;
            hasSwitchedTool = true;
        }

         if(Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentTool = ToolType.basket;
            hasSwitchedTool = true;
        }

        if (hasSwitchedTool == true)
        {
            //FindFirstObjectByType<UIController>().SwitchTool((int)currentTool);

            UIController.instance.SwitchTool((int)currentTool);
        }


         

        if(actionInput.action.WasPressedThisFrame())
        {
            UseTool();

        }
        anim.SetFloat("speed", theRB.linearVelocity.magnitude);
        toolIndicator.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        toolIndicator.position = new Vector3(toolIndicator.position.x, toolIndicator.position.y, 0f);

        if(Vector3.Distance(toolIndicator.position, transform.position) > toolRange)
        {
            Vector2 direction = toolIndicator.position - transform.position;
            direction = direction.normalized * toolRange;
            toolIndicator.position = transform.position + new Vector3(direction.x, direction.y, 0f);

        }

        toolIndicator.position = new Vector3(Mathf.FloorToInt(toolIndicator.position.x) + .5f,
        Mathf.FloorToInt(toolIndicator.position.y) + .5f,
        0f);

    }

            void UseTool()
            {
                GrowBlock block = null;

                //block = FindFirstObjectByType<GrowBlock>();

                //block.PloughSoil();

                block = GridController.instance.GetBlock(toolIndicator.position.x -.5f, toolIndicator.position.y - .5f);

                toolWaitCounter = toolWaitTime;


                if(block != null)
                {
                    switch(currentTool)
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

}
