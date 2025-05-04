using UnityEngine;

public class ShopController : MonoBehaviour
{
    public void OpenClose()
    {

        if(UIController.instance.theIC.gameObject.activeSelf == false)
        {
             gameObject.SetActive(!gameObject.activeSelf);
        }
       
    }
}
