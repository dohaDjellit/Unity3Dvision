using UnityEngine;

public class SceneCameraControl : MonoBehaviour
{
    public float zoomSpeed = 10f;         // Speed of zooming
    public float minZoom = 5f;            // Minimum zoom distance
    public float maxZoom = 150f;          // Maximum zoom distance
    public float panSpeed = 0.5f;         // Speed of panning
    public float rotationSpeed = 3f;    // Speed of rotation

    private Camera cam;
    private Vector3 rotationOrigin;       // Rotation center for orbiting

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("SceneCameraControl script must be attached to a Camera.");
        }
        rotationSpeed = 300f;
        // Set the initial rotation origin (target point) as the position in front of the camera
        rotationOrigin = transform.position + transform.forward * 10f;
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleRotation();
    }

    // Function to handle zooming in and out with the scroll wheel
    private void HandleZoom()
    {
        if (cam != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float newSize = cam.orthographic ? cam.orthographicSize - scroll * zoomSpeed : cam.fieldOfView - scroll * zoomSpeed;
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

            if (cam.orthographic)
            {
                cam.orthographicSize = newSize;
            }
            else
            {
                cam.fieldOfView = newSize;
            }
        }
    }

    // Function to handle panning with the middle mouse button
    private void HandlePan()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button pressed
        {
            float h = -Input.GetAxis("Mouse X") * panSpeed;
            float v = -Input.GetAxis("Mouse Y") * panSpeed;
            transform.Translate(new Vector3(h, v, 0), Space.Self);
        }
    }

    // Function to handle rotation with the right mouse button
    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button pressed
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Rotate the camera around the rotation origin point
            transform.RotateAround(rotationOrigin, Vector3.up, rotationX);     // Horizontal rotation
            transform.RotateAround(rotationOrigin, transform.right, -rotationY); // Vertical rotation
        }
    }
}
