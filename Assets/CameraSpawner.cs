using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraSpawner : MonoBehaviour
{
    public GameObject cameraPrefab; // Reference to the camera prefab
    public Slider verticalFOVSlider; // Slider for Vertical FOV
    public TMP_InputField verticalFOVInput; // Input field for Vertical FOV
    public TMP_InputField sensorSizeXInput, sensorSizeYInput; // Inputs for sensor size
    public TMP_Text intrinsicInfo; // Intrinsic info display

    private int cameraCount = 0; // Counter for naming cameras

    public void SpawnCamera()
    {
        // Instantiate the camera prefab
        GameObject newCamera = Instantiate(cameraPrefab);
        newCamera.name = $"Camera{++cameraCount}";

        // Assign UI elements to the script on the new camera
        var cameraScript = newCamera.GetComponent<CameraDoubleClick>();
        if (cameraScript != null)
        {
            cameraScript.verticalFOVSlider = verticalFOVSlider;
            cameraScript.verticalFOVInput = verticalFOVInput;
            cameraScript.sensorSizeXInput = sensorSizeXInput;
            cameraScript.sensorSizeYInput = sensorSizeYInput;
            cameraScript.intrinsicInfo = intrinsicInfo;
        }

        // Notify SensorManager about the new camera
        SensorManager sensorManager = FindObjectOfType<SensorManager>();
        if (sensorManager != null)
        {
            sensorManager.AddSensor(newCamera);
        }
    }
}
