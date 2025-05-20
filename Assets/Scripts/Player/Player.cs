using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;  // <--- This is crucial!

    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    public PlayerStats Stats => stats;

    private PlayerAnimations animations;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        animations = GetComponent<PlayerAnimations>();
    }

    public void ResetPlayer()
    {
        stats.ResetPlayer();
        animations.ResetPlayer();
    }

    public void SwitchSeed(CropController.CropType newSeed)
{
    // You can implement this when you add crop selection/planting logic
    // For now, this just makes the code compile
}

// Freezes the player (e.g., to disable movement/input during scene transitions or menus)
public void HardFreeze()
{
    // You can implement this to stop all player movement/input
    // For now, this just makes the code compile
}

// Unfreezes the player (restores movement/input)
public void Unfreeze()
{
    // You can implement this to re-enable player control
    // For now, this just makes the code compile
}
}
