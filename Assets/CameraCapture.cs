using UnityEngine;
using System.IO;

public class CameraCapture : MonoBehaviour
{
    public string saveFolder = "CapturedImages"; // Folder to save images
    private static CameraCapture activeCamera;   // Currently active camera

    private int captureCount = -1; // Counter to track the number of captures for this camera

    void Start()
    {
        // Ensure the save folder exists
        string folderPath = Path.Combine(Application.persistentDataPath, saveFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Initialize the capture count based on existing files
        InitializeCaptureCount(folderPath);
    }

    void Update()
    {
        // Capture image from the active camera when "C" is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (activeCamera == this) // Ensure only the active camera responds
            {
                CaptureImage();
            }
            else if (activeCamera == null)
            {
                Debug.LogWarning("No active camera to capture. Double-click a camera to select it.");
            }
        }

        // Capture all cameras when "A" is pressed
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (activeCamera == null || activeCamera == this) // Allow the first call only
            {
                CaptureAllCameras();
            }
        }
    }

    void InitializeCaptureCount(string folderPath)
    {
        // Reset captureCount to start fresh each time the game runs
        captureCount = 0;

        Debug.Log($"Capture count reset for {gameObject.name}: {captureCount}");
    }




    public void CaptureImage()
    {
        // Create a temporary RenderTexture for capturing the camera’s view
        RenderTexture tempRT = new RenderTexture(Screen.width, Screen.height, 24);
        GetComponent<Camera>().targetTexture = tempRT;

        // Render the camera's view to the RenderTexture
        GetComponent<Camera>().Render();

        // Read the RenderTexture contents into a Texture2D
        RenderTexture.active = tempRT;
        Texture2D image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();

        // Reset camera target texture and release the RenderTexture
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(tempRT);

        // Save the image as PNG with the camera's name and screenshot number
        string fileName = $"{gameObject.name}_screen{captureCount++}.png"; // Increment and use captureCount
        string filePath = Path.Combine(Application.persistentDataPath, saveFolder, fileName);
        File.WriteAllBytes(filePath, image.EncodeToPNG());

        // Clean up
        Destroy(image);

        Debug.Log($"Captured image from {gameObject.name} saved at: {filePath}");
    }

    public static void SetActiveCamera(CameraCapture camera)
    {
        activeCamera = camera;
        Debug.Log($"Active camera set to: {camera.gameObject.name}");
    }

    private void CaptureAllCameras()
    {
        CameraCapture[] cameras = FindObjectsOfType<CameraCapture>();

        foreach (CameraCapture camera in cameras)
        {
            camera.CaptureImage();
        }

        Debug.Log($"Captured all {cameras.Length} cameras.");
    }
}
