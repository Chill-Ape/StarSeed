using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; 
    public Transform target;
    public Transform clampMin, clampMax;

    private Camera cam;
    private float halfWidth, halfHeight;

    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    void Start()
    {
        // Try to find the player if target isn't set
        if (target == null)
        {
            if (Player.instance != null)
            {
                target = Player.instance.transform;
            }
            else
            {
                Debug.LogWarning("No player found for camera to follow!");
            }
        }

        if (clampMin != null) clampMin.SetParent(null);
        if (clampMax != null) clampMax.SetParent(null);

        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * cam.aspect;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (clampMin != null && clampMax != null)
        {
            Vector3 clampedPosition = transform.position;

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, clampMin.position.x + halfWidth, clampMax.position.x - halfWidth);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, clampMin.position.y + halfHeight, clampMax.position.y - halfHeight);

            transform.position = clampedPosition;
        }
        
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.orthographicSize -= scrollData * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }
}