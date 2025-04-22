using UnityEngine;

public class AreaSwitcher : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string sceneToLoad;
    public string transitionName;

    [Header("Optional Start Point")]
    public Transform startPoint;

    void Start()
    {
        if (PlayerPrefs.HasKey("Transition") && PlayerController.instance != null)
        {
            if (PlayerPrefs.GetString("Transition") == transitionName && startPoint != null)
            {
                PlayerController.instance.transform.position = startPoint.position;
                PlayerPrefs.DeleteKey("Transition");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetString("Transition", transitionName);

            SceneLoader loader = FindObjectOfType<SceneLoader>();
            if (loader != null)
            {
                loader.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("SceneLoader not found in the scene!");
            }
        }
    }
}
