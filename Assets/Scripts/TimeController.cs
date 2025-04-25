using UnityEngine;

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

        StartDay();
    }

    public void StartDay()
    {
        timeActive = true;

        currentTime = dayStart;
    }
}
