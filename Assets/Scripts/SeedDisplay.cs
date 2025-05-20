using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedDisplay : MonoBehaviour
{
 public CropController.CropType crop;

 public Image seedImage;
 public TMP_Text seedAmount;

 public void UpdateDisplay()
 {
    CropInfo info = CropController.instance.GetCropInfo(crop);

    seedImage.sprite = info.seedType;
    seedAmount.text = "x" + info.seedAmount;
 }

 public void SelectSeed()
 {
    Player.instance.SwitchSeed(crop);

    UIController.instance.SwitchSeed(crop);

    UIController.instance.theIC.OpenClose();
 }
}
