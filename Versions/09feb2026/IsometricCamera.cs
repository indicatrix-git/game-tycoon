using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    private float panSpeed = 15.0f;
    private float zoomSpeed = 50;
    private float zoomSmoothness = 1;

    private float minZoom = 2;
    private float maxZoom = 40;

    private float currentZoom;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        Vector2 panPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(panPosition.x, 0, panPosition.y) * panSpeed * Time.deltaTime;

        currentZoom = Mathf.Clamp(currentZoom - Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, zoomSmoothness * Time.deltaTime);
    }
}
