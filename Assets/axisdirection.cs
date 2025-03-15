using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AxisDirection : MonoBehaviour
{
    public KeyCode printKey = KeyCode.K; // Set default key to "K"
    public TMP_Text groundTruthText; // UI text element to display ground truth
    public string cylinderTag = "Cylinder"; // Tag to identify cylinders in the scene

    void Update()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(printKey))
        {
            GameObject cylinder = FindCylinder();
            if (cylinder == null)
            {
                Debug.LogError("No cylinder found in the scene!");
                return;
            }

            Transform cylinderTransform = cylinder.transform;

            // Get the cylinder's center (position in world space)
            Vector3 center = cylinderTransform.position;

            // Get the cylinder's axis direction (local Y-axis in world space)
            Vector3 axisDirection = cylinderTransform.up;

            // Get the cylinder's radius (assume uniform scaling)
            float radius = cylinderTransform.localScale.x / 2f;

            // Get the cylinder's height
            float height = cylinderTransform.localScale.y;

            // Format the ground truth data
            string groundTruth = 
                $"Center: ({center.x:G9}, {center.y:G9}, {center.z:G9})\n" +
                $"Axis Direction: ({axisDirection.x:G9}, {axisDirection.y:G9}, {axisDirection.z:G9})\n" +
                $"Radius: {radius:G9}\n" +
                $"Height: {height:G9}";

            // Display the data in the UI
            if (groundTruthText != null)
            {
                groundTruthText.text = groundTruth;
            }
            else
            {
                Debug.LogError("TMP_Text groundTruthText is not assigned!");
            }

            // Log to the console as well
            Debug.Log(groundTruth);

            // Optional: Visualize the axis in the Scene view
            Debug.DrawRay(center, axisDirection * height / 2, Color.red, 1f);
            Debug.DrawRay(center, -axisDirection * height / 2, Color.red, 1f);
        }
    }

    private GameObject FindCylinder()
    {
        GameObject[] cylinders = GameObject.FindGameObjectsWithTag(cylinderTag);
        if (cylinders.Length > 0)
        {
            return cylinders[0]; // Return the first cylinder found in the scene
        }
        return null;
    }
}
