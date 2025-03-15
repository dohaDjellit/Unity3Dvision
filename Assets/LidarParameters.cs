using UnityEngine;
using TMPro;

public class LidarParameters : MonoBehaviour
{
    public TMP_InputField horizontalResolutionInput;
    public TMP_InputField verticalBeamsInput;
    public TMP_InputField maxDistanceInput;
    public TMP_InputField verticalFOVInput;

    private LiDAR3DSimulator currentLidar; // The currently selected LiDAR

    void Update()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedLidar();
        }
    }

    void DetectClickedLidar()
    {
        // Perform a raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObject = hit.transform.gameObject;

            // Check the name of the clicked object
            if (clickedObject.name.Contains("Lidar3D")) // Assuming LiDAR prefabs contain "LiDAR" in their name
            {
                LiDAR3DSimulator lidar = clickedObject.GetComponent<LiDAR3DSimulator>();
                if (lidar != null)
                {
                    SelectLidar(lidar);
                }
                else
                {
                    Debug.LogWarning("Clicked object is named as LiDAR but does not have LiDAR3DSimulator attached.");
                }
            }
            else
            {
                Debug.Log("Clicked object is not a LiDAR.");
            }
        }
    }

    void SelectLidar(LiDAR3DSimulator lidar)
    {
        // Set the current lidar and update the input fields
        currentLidar = lidar;

        // Populate the input fields with the LiDAR parameters
        horizontalResolutionInput.text = lidar.horizontalBeams.ToString(); // Horizontal beams
        verticalBeamsInput.text = lidar.verticalBeams.ToString(); // Vertical beams
        maxDistanceInput.text = lidar.maxDistance.ToString(); // Maximum distance
        verticalFOVInput.text = lidar.verticalFOV.ToString(); // Vertical field of view

        Debug.Log($"Selected LiDAR: {lidar.name}");
    }

    public void ApplyParameters()
    {
        if (currentLidar != null)
        {
            // Update the LiDAR parameters based on the input field values
            if (int.TryParse(horizontalResolutionInput.text, out int horizontalBeams))
                currentLidar.horizontalBeams = horizontalBeams;

            if (int.TryParse(verticalBeamsInput.text, out int verticalBeams))
                currentLidar.verticalBeams = verticalBeams;

            if (float.TryParse(maxDistanceInput.text, out float maxDist))
                currentLidar.maxDistance = maxDist;

            if (float.TryParse(verticalFOVInput.text, out float verticalFOV))
                currentLidar.verticalFOV = verticalFOV;

            Debug.Log($"Updated LiDAR: {currentLidar.name}");
        }
        else
        {
            Debug.LogWarning("No LiDAR selected to update parameters.");
        }
    }
}
