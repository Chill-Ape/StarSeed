using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIController : MonoBehaviour
{
    public static UIController instance; 
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        } else
        {
            Destroy(gameObject);
        }

    }

    public GameObject [] toolbarActivatorIcons;

    public TMP_Text timeText;

    public InventoryController theIC;
    public ShopController theShop;

    public Image seedImage;

    public GameObject pauseScreen;
    public string mainMenuScene;

    void Start()
    {
        SwitchTool(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            theIC.OpenClose();
        }
#if UNITY_EDITOR

        if(Keyboard.current.bKey.wasPressedThisFrame)
        {
            theShop.OpenClose();
        }
#endif        
        
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseUnpause();
        }
    }

    public void SwitchTool(int selected)
    {
        foreach(GameObject icon in toolbarActivatorIcons)
        {
            icon.SetActive(false);
        }
        toolbarActivatorIcons[selected].SetActive(true);
    }

    public void UpdateTimeText(float currentTime)
    {
        if(currentTime < 12)
        {
            timeText.text = Mathf.FloorToInt(currentTime) + "AM";
        } else if(currentTime < 13)
        {
           timeText.text = "12:PM";
        } else if(currentTime < 24)
        {
            timeText.text = (Mathf.FloorToInt(currentTime) - 12) + "PM";
        } else if(currentTime < 25)
        {
            timeText.text = "12AM";
        } else
        {
            timeText.text = Mathf.FloorToInt(currentTime -24) + "AM";
        }
    }

    public void SwitchSeed(CropController.CropType crop)
    {
        seedImage.sprite = CropController.instance.GetCropInfo(crop).seedType;
        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void PauseUnpause()
    {
        if(pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);

            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;

        }

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void MainMenu()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);

        Destroy(gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(GridInfo.instance.gameObject);
        Destroy(CropController.instance.gameObject);
        Destroy(TimeController.instance.gameObject);
        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void QuitGame()
    {
        Application.Quit();
        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }
}
