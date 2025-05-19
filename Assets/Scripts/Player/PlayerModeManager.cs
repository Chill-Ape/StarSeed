using UnityEngine;

public class PlayerModeManager : MonoBehaviour
{
    public static PlayerModeManager instance;

    [Header("Player Components")]
    public PlayerController farmingController;
    public RPGPlayerMovement rpgController;

    [Header("Current Mode")]
    public bool isInRPGMode = false;

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
    }

    private void Start()
    {
        // Initialize with RPG mode by default
        SwitchToRPGMode();
    }

    public void SwitchToRPGMode()
    {
        isInRPGMode = true;
        if (farmingController != null) farmingController.enabled = false;
        if (rpgController != null) rpgController.enabled = true;
    }

    public void SwitchToFarmingMode()
    {
        isInRPGMode = false;
        if (farmingController != null) farmingController.enabled = true;
        if (rpgController != null) rpgController.enabled = false;
    }

    // Call this when entering different areas
    public void OnEnterArea(string areaType)
    {
        if (areaType.ToLower() == "rpg")
        {
            SwitchToRPGMode();
        }
        else if (areaType.ToLower() == "farming")
        {
            SwitchToFarmingMode();
        }
    }
}