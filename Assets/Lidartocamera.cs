using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidartocamera : MonoBehaviour
{
    public Camera targetCamera; // Assign your camera in the inspector
    public Transform lidarTransform; // Assign your LiDAR's transform in the inspector

    void Update()
    {
        // Check if the T key is pressed
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (targetCamera == null || lidarTransform == null)
            {
                Debug.LogError("Please assign both the camera and LiDAR transform!");
                return;
            }

            // Get the necessary matrices
            Matrix4x4 lidarToWorldMatrix = lidarTransform.localToWorldMatrix;

            // Extract the rotation part (top-left 3x3 matrix)
            Matrix4x4 rotationPart = Matrix4x4.identity;
            for (int i = 0; i < 3; i++) // Copy top-left 3x3 elements
                for (int j = 0; j < 3; j++)
                    rotationPart[i, j] = lidarToWorldMatrix[i, j];

            // Define the custom rotation matrix
            // Define the custom rotation matrix
            Matrix4x4 customRotation = Matrix4x4.identity;
            customRotation[0, 1] = -1; // Row 0, Col 1 = -1
            customRotation[1, 2] = 1;  // Row 1, Col 2 = 1
            customRotation[2, 0] = 1;  // Row 2, Col 0 = 1
            customRotation[0, 0] = 0;  // Row 0, Col 0 = 0
            customRotation[1, 1] = 0;  // Row 1, Col 1 = 0
            customRotation[2, 2] = 0;  // Row 2, Col 2 = 0
            Debug.Log(customRotation);
            // Multiply the rotation parts
            Matrix4x4 modifiedRotation = customRotation * rotationPart;

            // Create the new matrix with the modified rotation
            Matrix4x4 modifiedLidarToWorldMatrix = lidarToWorldMatrix;
            for (int i = 0; i < 3; i++) // Replace top-left 3x3 with the modified rotation
                for (int j = 0; j < 3; j++)
                    modifiedLidarToWorldMatrix[i, j] = modifiedRotation[i, j];

            // Translation part remains unchanged
            Debug.Log("Original LiDAR to World Matrix:\n" + lidarTransform.localToWorldMatrix);
            Debug.Log("Modified LiDAR to World Matrix:\n" + modifiedLidarToWorldMatrix);
            //lidarToWorldMatrix[2, 0] *= -1; lidarToWorldMatrix[2, 1] *= -1; lidarToWorldMatrix[2, 2] *= -1; lidarToWorldMatrix[2, 3] *= 1;
            ///lidarToWorldMatrix[0, 0] *= -1; lidarToWorldMatrix[0, 1] *= -1; lidarToWorldMatrix[0, 2] *= -1; lidarToWorldMatrix[0, 3] *= 1;


            Matrix4x4 worldToCameraMatrix = targetCamera.worldToCameraMatrix; // World to camera

            worldToCameraMatrix[2, 0] *= -1; worldToCameraMatrix[2, 1] *= -1; worldToCameraMatrix[2, 2] *= -1; worldToCameraMatrix[2, 3] *= -1;
            worldToCameraMatrix[1, 0] *= -1; worldToCameraMatrix[1, 1] *= -1; worldToCameraMatrix[1, 2] *= -1; worldToCameraMatrix[1, 3] *= -1;
            Debug.Log("World to Camera Matrix:\n" + worldToCameraMatrix);
            // Combine the matrices: LiDAR local space -> camera space
            Matrix4x4 lidarToCameraMatrix = worldToCameraMatrix * modifiedLidarToWorldMatrix;

            // Multiply the second and third rows by -1
            //lidarToCameraMatrix[1, 0] *= 1; lidarToCameraMatrix[1, 1] *= 1; lidarToCameraMatrix[1, 2] *= 1; lidarToCameraMatrix[1, 3] *= 1;
            //lidarToCameraMatrix[2, 0] *=-1; lidarToCameraMatrix[2, 1] *= -1; lidarToCameraMatrix[2, 2] *= -1; lidarToCameraMatrix[2, 3] *= -1;

            // Format the matrix as MATLAB input
            string matlabMatrix = "[";
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    matlabMatrix += lidarToCameraMatrix[row, col].ToString("F6");
                    if (col < 3) matlabMatrix += ", "; // Add a comma between elements in a row
                }
                if (row < 3) matlabMatrix += ";\n "; // Add a semicolon between rows
            }
            matlabMatrix += "]";

            // Display the final MATLAB-compatible matrix
            Debug.Log("LiDAR to Camera Transformation Matrix (MATLAB format):\n" + matlabMatrix);
        }
    }
}

