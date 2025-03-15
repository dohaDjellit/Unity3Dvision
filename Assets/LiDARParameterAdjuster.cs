using UnityEngine;
using TMPro;

public class LiDARParameterAdjuster : MonoBehaviour
{
    public TMP_InputField horizontalBeamsInput;  // Input for horizontal beams
    public TMP_InputField verticalBeamsInput;    // Input for vertical beams
    public TMP_InputField maxDistanceInput;      // Input for maximum distance
    public TMP_InputField verticalFOVInput;      // Input for vertical FOV

    private LiDAR3DSimulator currentLidar;       // Currently selected LiDAR

    public void SelectLidar(LiDAR3DSimulator lidar)
    {
        // Update the reference to the currently selected LiDAR
        currentLidar = lidar;

        // Populate input fields with the selected LiDAR's parameters
        horizontalBeamsInput.text = lidar.horizontalBeams.ToString();
        verticalBeamsInput.text = lidar.verticalBeams.ToString();
        maxDistanceInput.text = lidar.maxDistance.ToString("F2");
        verticalFOVInput.text = lidar.verticalFOV.ToString("F2");

        Debug.Log($"LiDAR {lidar.name} selected. Parameters displayed.");
    }

    public void ApplyParameters()
    {
        if (currentLidar != null)
        {
            // Update the LiDAR's parameters based on the input field values
            if (int.TryParse(horizontalBeamsInput.text, out int horizontalBeams))
                currentLidar.horizontalBeams = Mathf.Max(1, horizontalBeams); // Ensure at least 1 beam

            if (int.TryParse(verticalBeamsInput.text, out int verticalBeams))
                currentLidar.verticalBeams = Mathf.Max(1, verticalBeams); // Ensure at least 1 beam

            if (float.TryParse(maxDistanceInput.text, out float maxDist))
                currentLidar.maxDistance = Mathf.Max(0.1f, maxDist); // Ensure positive distance

            if (float.TryParse(verticalFOVInput.text, out float verticalFOV))
                currentLidar.verticalFOV = Mathf.Clamp(verticalFOV, 0f, 90f); // Clamp vertical FOV to <= 90°

            Debug.Log($"Updated parameters for LiDAR: {currentLidar.name}");
        }
        else
        {
            Debug.LogWarning("No LiDAR selected to update parameters.");
        }
    }

    public void SaveSelectedLiDAR()
    {
        if (currentLidar != null)
        {
            currentLidar.SaveLiDARData();
            Debug.Log($"Saved data for LiDAR: {currentLidar.name}");
        }
        else
        {
            Debug.LogWarning("No LiDAR selected to save data.");
        }
    }

    // Clear the current selection and input fields
    public void DeselectLidar()
    {
        currentLidar = null;

        // Clear input fields
        if (horizontalBeamsInput != null) horizontalBeamsInput.text = "";
        if (verticalBeamsInput != null) verticalBeamsInput.text = "";
        if (maxDistanceInput != null) maxDistanceInput.text = "";
        if (verticalFOVInput != null) verticalFOVInput.text = "";

        Debug.Log("LiDAR deselected. Input fields cleared.");
    }
}
