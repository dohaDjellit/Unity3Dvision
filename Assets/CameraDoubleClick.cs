using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class CameraDoubleClick : MonoBehaviour
{
    private float lastClickTime;
    private const float doubleClickThreshold = 0.25f;

    // UI elements
    public TMP_InputField verticalFOVInput;
    public TMP_InputField sensorSizeXInput, sensorSizeYInput;
    public TMP_Text intrinsicInfo;
    public Slider verticalFOVSlider;
    public Button applyButton;

    private static CameraDoubleClick selectedCamera;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        var camera = GetComponent<Camera>();

        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found! Ensure a camera is tagged as 'MainCamera'.");
        }

        // Ensure the camera uses physical properties
        camera.usePhysicalProperties = true;

        // Initialize Vertical FOV Slider
        if (verticalFOVSlider != null)
        {
            verticalFOVSlider.minValue = 20f;
            verticalFOVSlider.maxValue = 120f;
            verticalFOVSlider.value = camera.fieldOfView;
            verticalFOVSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // Initialize Vertical FOV Input Field
        if (verticalFOVInput != null)
        {
            verticalFOVInput.text = camera.fieldOfView.ToString("F1");
            verticalFOVInput.onEndEdit.AddListener(OnFOVInputFieldValueChanged);
        }

        // Initialize Sensor Size Input Fields
        if (sensorSizeXInput != null)
        {
            sensorSizeXInput.text = camera.sensorSize.x.ToString("F1");
            sensorSizeXInput.onEndEdit.AddListener(OnSensorSizeInputFieldValueChanged);
        }
        if (sensorSizeYInput != null)
        {
            sensorSizeYInput.text = camera.sensorSize.y.ToString("F1");
            sensorSizeYInput.onEndEdit.AddListener(OnSensorSizeInputFieldValueChanged);
        }

        // Initialize Apply Button
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(ApplySensorSizeChanges);
        }

        UpdateIntrinsicInfo();
    }

    void Update()
    {
        // Exit camera view when pressing Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainCamera();
        }
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= doubleClickThreshold)
        {
            SelectCamera();
        }
        lastClickTime = Time.time;
    }

    void SelectCamera()
    {
        if (selectedCamera != null && selectedCamera != this)
        {
            selectedCamera.DeselectCamera();
        }

        selectedCamera = this;

        // Notify CameraCapture to set this as the active camera
        CameraCapture cameraCapture = GetComponent<CameraCapture>();
        if (cameraCapture != null)
        {
            CameraCapture.SetActiveCamera(cameraCapture);
        }
        else
        {
            Debug.LogWarning("CameraCapture component not found on this GameObject.");
        }

        // Update UI to reflect the selected camera's properties
        UpdateUIWithCameraProperties();
        SaveIntrinsicParametersAsCsv();

        EnterCameraView();
        Debug.Log($"Selected camera: {gameObject.name}");
    }

    void DeselectCamera()
    {
        Debug.Log($"Deselected camera: {gameObject.name}");
    }

    void EnterCameraView()
    {
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false); // Disable main camera
        }

        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.enabled = true; // Enable this camera
        }
        Debug.Log($"Entered view for: {gameObject.name}");
    }


    public void ReturnToMainCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true); // Enable main camera
        }

        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.enabled = false; // Disable this camera
        }
        Debug.Log($"Returned to main camera from: {gameObject.name}");
    }

    public void OnSliderValueChanged(float newFOV)
    {
        if (selectedCamera == this)
        {
            GetComponent<Camera>().fieldOfView = newFOV;

            // Synchronize input field with slider
            if (verticalFOVInput != null)
            {
                verticalFOVInput.text = newFOV.ToString("F1");
            }

            UpdateIntrinsicInfo();
        }
    }

    public void OnFOVInputFieldValueChanged(string newFOV)
    {
        if (selectedCamera == this && float.TryParse(newFOV, out float fov))
        {
            fov = Mathf.Clamp(fov, verticalFOVSlider.minValue, verticalFOVSlider.maxValue);
            GetComponent<Camera>().fieldOfView = fov;

            // Synchronize slider with input field
            if (verticalFOVSlider != null)
            {
                verticalFOVSlider.value = fov;
            }

            UpdateIntrinsicInfo();
        }
    }

    public void OnSensorSizeInputFieldValueChanged(string _)
    {
        // Automatically apply changes when user presses Enter
        ApplySensorSizeChanges();
    }

    public void ApplySensorSizeChanges()
    {
        if (float.TryParse(sensorSizeXInput.text, out float sensorX) &&
            float.TryParse(sensorSizeYInput.text, out float sensorY))
        {
            GetComponent<Camera>().sensorSize = new Vector2(sensorX, sensorY);
            UpdateIntrinsicInfo();

            Debug.Log($"Sensor size updated: {sensorX} mm x {sensorY} mm");
        }
        else
        {
            Debug.LogWarning("Invalid sensor size input.");
        }
    }

    private void UpdateUIWithCameraProperties()
    {
        var camera = GetComponent<Camera>();

        // Update FOV
        if (verticalFOVInput != null)
        {
            verticalFOVInput.text = camera.fieldOfView.ToString("F1");
        }
        if (verticalFOVSlider != null)
        {
            verticalFOVSlider.value = camera.fieldOfView;
        }

        // Update Sensor Size Inputs
        if (sensorSizeXInput != null)
        {
            sensorSizeXInput.text = camera.sensorSize.x.ToString("F1");
        }
        if (sensorSizeYInput != null)
        {
            sensorSizeYInput.text = camera.sensorSize.y.ToString("F1");
        }

        UpdateIntrinsicInfo();
    }

    private void UpdateIntrinsicInfo()
    {
        var camera = GetComponent<Camera>();
        if (camera == null || intrinsicInfo == null) return;

        float focalLength = camera.focalLength; // Unity Physical Camera focal length in mm
        float sensorWidth = camera.sensorSize.x; // Sensor width in mm
        float sensorHeight = camera.sensorSize.y; // Sensor height in mm
        float imageWidth = Screen.width; // Image width in pixels
        float imageHeight = Screen.height; // Image height in pixels

        // Calculate horizontal and vertical pixel sizes
        float ku = imageWidth / sensorWidth; // Horizontal pixel size in mm/pixel
        float kv = imageHeight / sensorHeight; // Vertical pixel size in mm/pixel

        // Calculate fx and fy in pixels
        float fx = focalLength * ku; // Focal length in pixels (horizontal)
        float fy = focalLength * kv; // Focal length in pixels (vertical)

        // Update the UI with all information
        intrinsicInfo.text = $"Intrinsic Parameters:\n" +
                             $"Focal Length: {focalLength:F2} mm\n" +
                             $"Sensor Size: {sensorWidth:F1} x {sensorHeight:F1} mm\n" +
                             $"Image Size: {imageWidth} x {imageHeight} pixels\n" +
                             $"Pixel Size: {ku:F4} mm/pixel (k_u), {kv:F4} mm/pixel (k_v)\n" +
                             $"fx: {fx:F2} pixels, fy: {fy:F2} pixels";
    }

    private void SaveIntrinsicParametersAsCsv()
    {
        var camera = GetComponent<Camera>();
        if (camera == null) return;

        // Retrieve the necessary parameters
        float focalLength = camera.focalLength; // Focal length in mm
        float sensorWidth = camera.sensorSize.x; // Sensor width in mm
        float sensorHeight = camera.sensorSize.y; // Sensor height in mm
        float imageWidth = Screen.width; // Image width in pixels
        float imageHeight = Screen.height; // Image height in pixels

        // Calculate horizontal and vertical pixel sizes
        float ku = imageWidth/ sensorWidth ; // Horizontal pixel size in mm/pixel
        float kv = imageHeight / sensorHeight ; // Vertical pixel size in mm/pixel

        // Calculate fx and fy in pixels
        float fx = focalLength * ku; // Focal length in pixels (horizontal)
        float fy = focalLength * kv; // Focal length in pixels (vertical)

        // Save the parameters to a CSV file
        string filePath = Path.Combine(Application.persistentDataPath, $"{gameObject.name}_intrinsics.csv");
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Parameter,Value");
            writer.WriteLine($"FocalLength,{focalLength:F2} mm");
            writer.WriteLine($"SensorWidth,{sensorWidth:F1} mm");
            writer.WriteLine($"SensorHeight,{sensorHeight:F1} mm");
            writer.WriteLine($"ImageWidth,{imageWidth} pixels");
            writer.WriteLine($"ImageHeight,{imageHeight} pixels");
            writer.WriteLine($"PixelSize_k_u,{ku:F4} mm/pixel");
            writer.WriteLine($"PixelSize_k_v,{kv:F4} mm/pixel");
            writer.WriteLine($"fx,{fx:F2} pixels");
            writer.WriteLine($"fy,{fy:F2} pixels");
        }

        Debug.Log($"Intrinsic parameters saved to {filePath}");
    }
}


