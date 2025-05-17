using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class TimeController : MonoBehaviour
{

    public static TimeController instance;

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

    public float currentTime; 

    public float dayStart, dayEnd;

    public float timeSpeed = .25f;

    private bool timeActive;

    public int currentDay = 1;

    public string dayEndScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = dayStart;

        timeActive = true;
    }

    // Update is called once per frame
    void Update()
    {

        if(timeActive)
        {
            currentTime += Time.deltaTime * timeSpeed;

             if(currentTime > dayEnd)
            {
            currentTime = dayEnd;
            EndDay();
            }

            if(UIController.instance != null)
            {
            UIController.instance.UpdateTimeText(currentTime);
            }
        }

    } 

    public void EndDay()
    {
        timeActive = false;

        currentDay++;

        GridInfo.instance.GrowCrop();
        

        PlayerPrefs.SetString("Transition", "Wake Up");

        //StartDay();
        SceneManager.LoadScene(dayEndScene);
    }

    public void StartDay()
    {
        timeActive = true;

        currentTime = dayStart;
        AudioManager.instance.PlaySFX(6);
    }
}
