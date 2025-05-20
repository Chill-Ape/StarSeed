using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.5f;

    [Header("Optional Loading Screen UI")]
    public GameObject loadingScreen;

    private GameObject player;
    private MonoBehaviour playerController;
    private Animator animator;

    void Awake()
    {
        if (FindObjectsOfType<SceneLoader>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {

        Player pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (pc != null)
        {
            pc.HardFreeze();
        }

        // Find player
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Freeze movement
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.isKinematic = true;
            }

            // Get Animator & freeze animation
            animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("MoveX", 0f);  // adjust if your animator uses different param names
                animator.SetFloat("MoveY", 0f);
                animator.SetFloat("Speed", 0f);
                animator.speed = 0f;
            }

            // Disable movement/controller
            playerController = player.GetComponent<MonoBehaviour>(); // Replace with your actual player movement script if needed
            if (playerController != null)
                playerController.enabled = false;
        }

        // Fade to black
        if (fadePanel != null)
            yield return StartCoroutine(Fade(1f));

        // Show loading UI
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // Load new scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(0.3f); // optional pause
        asyncLoad.allowSceneActivation = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reacquire player in new scene
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Restore movement
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.isKinematic = false;

            animator = player.GetComponent<Animator>();
            if (animator != null)
                animator.speed = 1f;

            if (playerController != null)
                playerController.enabled = true;
        }

        // Fade back in
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            StartCoroutine(Fade(0f));
        }

        // Hide loading UI
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

            Player pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
            if (pc != null)
            {
                pc.Unfreeze();
            }


    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadePanel.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = targetAlpha;
    }
}
