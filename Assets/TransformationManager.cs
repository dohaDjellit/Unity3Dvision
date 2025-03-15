using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TransformationManager : MonoBehaviour
{
    public TMP_InputField cameraIndexInput; // Input for Camera -> World
    public TMP_InputField lidarIndexInput; // Input for LiDAR -> World
    public TMP_InputField lidarToCameraCameraInput; // Input for Camera index (LiDAR -> Camera)
    public TMP_InputField lidarToCameraLidarInput; // Input for LiDAR index (LiDAR -> Camera)

    public TMP_Text cameraToWorldText; // Text to display Camera -> World matrix
    public TMP_Text lidarToWorldText; // Text to display LiDAR -> World matrix
    public TMP_Text lidarToCameraText; // Text to display LiDAR -> Camera matrix

    public string sensorTag = "Sensor"; // Tag to identify sensors in the scene

    public void ApplyCameraToWorld()
    {
        if (int.TryParse(cameraIndexInput.text, out int cameraIndex))
        {
            GameObject camera = FindSensor($"Camera{cameraIndex}");
            if (camera != null && camera.TryGetComponent(out Camera targetCamera))
            {
                Matrix4x4 cameraToWorldMatrix = targetCamera.worldToCameraMatrix;

                // Compute Camera -> World as the inverse of World -> Camera
                cameraToWorldMatrix = cameraToWorldMatrix.inverse;
                cameraToWorldMatrix[2, 0] *= -1; cameraToWorldMatrix[2, 1] *= -1; cameraToWorldMatrix[2, 2] *= -1; cameraToWorldMatrix[2, 3] *= -1;
                cameraToWorldMatrix[1, 0] *= -1; cameraToWorldMatrix[1, 1] *= -1; cameraToWorldMatrix[1, 2] *= -1; cameraToWorldMatrix[1, 3] *= -1;
                cameraToWorldText.text = FormatMatrix(cameraToWorldMatrix);
                Debug.Log($"Camera -> World Matrix:\n{cameraToWorldMatrix}");
            }
            else
            {
                cameraToWorldText.text = "Camera not found!";
            }
        }
        else
        {
            cameraToWorldText.text = "Invalid Camera Index!";
        }
    }

    public void ApplyLidarToWorld()
    {
        if (int.TryParse(lidarIndexInput.text, out int lidarIndex))
        {
            GameObject lidar = FindSensor($"3DLidar{lidarIndex}");

            if (lidar != null)
            {
                Transform lidarTransform = lidar.transform;
                Matrix4x4 lidarToWorldMatrix = lidarTransform.localToWorldMatrix;

                // Apply custom rotation logic
                Matrix4x4 rotationPart = ExtractRotation(lidarToWorldMatrix);
                Matrix4x4 customRotation = GetCustomRotation();
                Matrix4x4 modifiedRotation = customRotation * rotationPart;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        lidarToWorldMatrix[i, j] = modifiedRotation[i, j];

                lidarToWorldText.text = FormatMatrix(lidarToWorldMatrix);
                Debug.Log($"LiDAR -> World Matrix:\n{lidarToWorldMatrix}");
            }
            else
            {
                lidarToWorldText.text = "LiDAR not found!";
            }
        }
        else
        {
            lidarToWorldText.text = "Invalid LiDAR Index!";
        }
    }

    public void ApplyLidarToCamera()
    {
        if (int.TryParse(lidarToCameraCameraInput.text, out int cameraIndex) &&
            int.TryParse(lidarToCameraLidarInput.text, out int lidarIndex))
        {
            GameObject Lidar1 = FindSensor($"3DLidar{cameraIndex}");
            GameObject lidar = FindSensor($"3DLidar{lidarIndex}");

            if (Lidar1 != null && lidar != null )
            {
                Transform lidarTransform = lidar.transform;
                Transform lidar1Transform = Lidar1.transform;
                Matrix4x4 lidarToWorldMatrix = lidarTransform.localToWorldMatrix;

                // Apply custom rotation logic
                Matrix4x4 rotationPart = ExtractRotation(lidarToWorldMatrix);
                Matrix4x4 customRotation = GetCustomRotation();
                Matrix4x4 modifiedRotation = customRotation * rotationPart;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        lidarToWorldMatrix[i, j] = modifiedRotation[i, j];
                
                Matrix4x4 lidar1ToWorldMatrix = lidar1Transform.localToWorldMatrix;

                // Apply custom rotation logic
                Matrix4x4 rotationPart1 = ExtractRotation(lidar1ToWorldMatrix);
                Matrix4x4 customRotation1 = GetCustomRotation();
                Matrix4x4 modifiedRotation1 = customRotation1 * rotationPart1;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        lidar1ToWorldMatrix[i, j] = modifiedRotation1[i, j];

                // Compute LiDAR -> Camera
                //Matrix4x4 worldToCameraMatrix = targetCamera.worldToCameraMatrix;
                //worldToCameraMatrix[2, 0] *= -1; worldToCameraMatrix[2, 1] *= -1; worldToCameraMatrix[2, 2] *= -1; worldToCameraMatrix[2, 3] *= -1;
                //worldToCameraMatrix[1, 0] *= -1; worldToCameraMatrix[1, 1] *= -1; worldToCameraMatrix[1, 2] *= -1; worldToCameraMatrix[1, 3] *= -1;
                
                Matrix4x4 lidarToCameraMatrix = lidar1ToWorldMatrix.inverse * lidarToWorldMatrix;

                lidarToCameraText.text = FormatMatrix(lidarToCameraMatrix);
                Debug.Log($"LiDAR -> Camera Matrix:\n{lidarToCameraMatrix}");
            }
            else
            {
                lidarToCameraText.text = "Camera or LiDAR not found!";
            }
        }
        else
        {
            lidarToCameraText.text = "Invalid Indices!";
        }
    }

    private GameObject FindSensor(string sensorName)
    {
        // Find all objects tagged as "sensor"
        GameObject[] sensors = GameObject.FindGameObjectsWithTag(sensorTag);

        // Search for the object with the matching name
        foreach (GameObject sensor in sensors)
        {
            if (sensor.name == sensorName)
            {
                return sensor;
            }
        }

        return null; // Sensor not found
    }

    private Matrix4x4 ExtractRotation(Matrix4x4 matrix)
    {
        Matrix4x4 rotationPart = Matrix4x4.identity;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                rotationPart[i, j] = matrix[i, j];
        return rotationPart;
    }

    private Matrix4x4 GetCustomRotation()
    {
        Matrix4x4 customRotation = Matrix4x4.identity;
        customRotation[0, 1] = -1;
        customRotation[1, 2] = 1;
        customRotation[2, 0] = 1;
        customRotation[0, 0] = 0;
        customRotation[1, 1] = 0;
        customRotation[2, 2] = 0;
        return customRotation;
    }

    private string FormatMatrix(Matrix4x4 matrix)
    {
        string formatted = "";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                formatted += matrix[i, j].ToString("F6") + "\t";
            }
            formatted += "\n";
        }
        return formatted;
    }
}
