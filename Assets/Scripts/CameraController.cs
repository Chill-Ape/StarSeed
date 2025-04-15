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

    // Start is called before the first frame update
    void Start()
    {

        //target = FindAnyObjectByType<PlayerController>().transform;
        target = PlayerController.instance.transform;

        clampMin.SetParent(null);
        clampMax.SetParent(null);

        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * cam.aspect;
        

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
       
    }

    // Update is called once per frame
    void Update()
    {

        

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, clampMin.position.x + halfWidth, clampMax.position.x - halfWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, clampMin.position.y + halfHeight, clampMax.position.y - halfHeight);

        transform.position = clampedPosition;
        

       
       
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.orthographicSize -= scrollData * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }
}
