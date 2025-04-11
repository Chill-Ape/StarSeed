using UnityEngine;
using UnityEngine.InputSystem;

public class GrowBlock : MonoBehaviour
{
    public enum GrowthStage 
    {

        barren,
        ploughed,
        planted,
        growing1,
        growing2,
        ripe
    }

    public GrowthStage currentStage;

    public SpriteRenderer theSR;
    public Sprite soilTilled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AdvanceStage();
    }

    // Update is called once per frame
    void Update()
    {
        // if(Keyboard.current.eKey.wasPressedThisFrame)
        // {
        //     AdvanceStage();
        //     setSoilSprite();

        // }
    }

    public void AdvanceStage()
    {
        currentStage = currentStage + 1;
        if((int)currentStage >= 6)
        {
            currentStage = GrowthStage.barren;
        }
    }
    public void setSoilSprite()
    {
        if(currentStage == GrowthStage.barren)
        {
            theSR.sprite = null;
        } else
        {
            theSR.sprite = soilTilled;
        }
    }
    
    public void PloughSoil()
    {
        if(currentStage == GrowthStage.barren)
        {
            currentStage = GrowthStage.ploughed;
        }
        setSoilSprite();
    }
}
