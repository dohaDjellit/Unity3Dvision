using UnityEngine;

public class SensorDataManager : MonoBehaviour
{
    public KeyCode saveAllKey = KeyCode.S; // Key to save all sensor data

    void Update()
    {
        // Listen for the save all key press
        if (Input.GetKeyDown(saveAllKey))
        {
            SaveAllSensorsData();
        }
    }

    public void SaveAllSensorsData()
    {
        // Save all camera data
        CameraCapture[] cameras = FindObjectsOfType<CameraCapture>();
        foreach (CameraCapture camera in cameras)
        {
            Debug.Log($"Saving data for camera: {camera.gameObject.name}");
            camera.CaptureImage(); // Call CaptureImage to save camera data
        }

        Debug.Log($"Saved data from {cameras.Length} cameras.");

        // Save all LiDAR data
        LiDAR3DSimulator[] lidars = FindObjectsOfType<LiDAR3DSimulator>();
        foreach (LiDAR3DSimulator lidar in lidars)
        {
            Debug.Log($"Saving data for LiDAR: {lidar.gameObject.name}");
            lidar.SaveLiDARData(); // Call SaveLiDARData to save LiDAR data
        }

        Debug.Log($"Saved data from {lidars.Length} LiDARs.");
    }
}
