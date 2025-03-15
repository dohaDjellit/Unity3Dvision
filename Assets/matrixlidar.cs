using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matrixlidar : MonoBehaviour
{
    void Update()
    {
        // Check if the user presses the 'T' key
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Get the LiDAR's local-to-world transformation matrix
            Matrix4x4 lidarToWorldMatrix = transform.localToWorldMatrix;

            // Print the transformation matrix
            Debug.Log("LiDAR to World Matrix:\n" + MatrixToString(lidarToWorldMatrix));
        }
    }

    // Helper function to format the matrix as a string
    private string MatrixToString(Matrix4x4 matrix)
    {
        return $"{matrix[0, 0]:F3}, {matrix[0, 1]:F3}, {matrix[0, 2]:F3}, {matrix[0, 3]:F3}\n" +
               $"{matrix[1, 0]:F3}, {matrix[1, 1]:F3}, {matrix[1, 2]:F3}, {matrix[1, 3]:F3}\n" +
               $"{matrix[2, 0]:F3}, {matrix[2, 1]:F3}, {matrix[2, 2]:F3}, {matrix[2, 3]:F3}\n" +
               $"{matrix[3, 0]:F3}, {matrix[3, 1]:F3}, {matrix[3, 2]:F3}, {matrix[3, 3]:F3}";
    }
}
