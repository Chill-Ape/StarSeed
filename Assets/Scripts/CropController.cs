using UnityEngine;
using System.Collections.Generic;

public class CropController : MonoBehaviour
{

    public static CropController instance;
    

    private void Awake()
    {
       if(instance == null)
       {
        instance = this;
        DontDestroyOnLoad(gameObject);
       }
       else
       {
        Destroy(gameObject);
       }


    }

        public enum CropType
       {
            pumpkin,
            lettuce,
            carrot,
            hay,
            patato,
            strawberry,
            tomato,
            avocado 
       }

        public List<CropInfo> cropList = new List<CropInfo>();

        public CropInfo GetCropInfo(CropType cropToGet)
        {
            int position = -1;

            for(int i = 0; i < cropList.Count; i++)
            {
                if (cropList[i].cropType == cropToGet)
                {
                    position = i;
                    
                }
            }
            
            if(position >= 0)
            {
                return cropList[position];
            } else
            {
                return null;
            }
            
        }

        public void UseSeed(CropType seedToUse)
        {
            foreach(CropInfo crop in cropList)
            {
                if(crop.cropType == seedToUse)
                {
                     crop.seedAmount--;
                }
            }
        }

        public void AddCrop(CropType cropToAdd)
        {
            foreach(CropInfo crop in cropList)
            {
                if(crop.cropType == cropToAdd)
                {
                    crop.cropAmount++;
                }
            }
        }

}

 [System.Serializable]

 public class CropInfo
 {
    public CropController.CropType cropType;
    public Sprite finalCrop, seedType, planted, growStage1, growStage2, ripe;

    public int seedAmount, cropAmount;

    [Range(0f, 100f)]
    public float growthFailChance;

    public float seedPrice, cropPrice;

 }
