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
    public Sprite soilTilled, soilWatered;

    public SpriteRenderer cropSR;
    public Sprite cropPlanted, cropGrowing1, cropGrowing2, cropRipe;

    

    public bool isWatered;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    AdvanceStage();
        //    SetSoilSprite();

        //}
    #if UNITY_EDITOR
        if(Keyboard.current.nKey.wasPressedThisFrame)
        {
            AdvanceCrop();
        }
    #endif
    }


    public void AdvanceStage()
    {
        currentStage = currentStage + 1;
        if((int)currentStage >= 6)
        {
            currentStage = GrowthStage.barren;
        }
    }
    public void SetSoilSprite()
    {
        if(currentStage == GrowthStage.barren)
        {
            theSR.sprite = null;
        } else
        {
            if (isWatered == true)
            {
                theSR.sprite = soilWatered;
            }
            else
            {
                theSR.sprite = soilTilled;
            }

                
        }
    }
    
    public void PloughSoil()
    {
        if(currentStage == GrowthStage.barren)
        {
            currentStage = GrowthStage.ploughed;
        }
        SetSoilSprite();
    }

    public void WaterSoil()
    {
        isWatered = true;

        SetSoilSprite();
    }

    public void PlantCrop()
    {
        if(currentStage == GrowthStage.ploughed && isWatered == true)
        {
            currentStage = GrowthStage.planted;
            UpdateCropSprite();
        }
    }

    void UpdateCropSprite()
    {
        switch(currentStage)
       {
        case GrowthStage.planted:

        cropSR.sprite =cropPlanted;

        break;

        case GrowthStage.growing1:

        cropSR.sprite = cropGrowing1;

        break;

        case GrowthStage.growing2:

        cropSR.sprite = cropGrowing2;

        break;

        case GrowthStage.ripe:

        cropSR.sprite = cropRipe;

        break;
       }
    }

    public void AdvanceCrop()
    {
        if(isWatered == true)
        {
            if(currentStage == GrowthStage.planted || currentStage == GrowthStage.growing1 || currentStage == GrowthStage.growing2)
            {
                currentStage++;
                isWatered = false;
                SetSoilSprite();
                UpdateCropSprite();
            }
        }
    }

    public void HarvestCrop()
    {
        if(currentStage == GrowthStage.ripe)
        {
            currentStage = GrowthStage.ploughed;

            SetSoilSprite();
            cropSR.sprite = null;
        }
    }
}
