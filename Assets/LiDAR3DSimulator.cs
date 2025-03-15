/*using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class LiDAR3DSimulator : MonoBehaviour
{
    public float verticalFOV = 45f;
    public int verticalBeams = 128;
    public int horizontalBeams = 1000;
    public float maxDistance = 20f;
    public KeyCode saveKey = KeyCode.L;
    public float noiseStdDev = 0.73f;  // Standard deviation for Gaussian noise




    private List<Vector3> scanPoints = new List<Vector3>();
    private static int saveCount = -1;
    private System.Random rand = new System.Random(); // Move Random generator outside the loop

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            Debug.Log("L key pressed");
            SaveLiDARData();
        }
    }

    void SimulateLiDAR()
    {
        scanPoints.Clear();
        float verticalStep = verticalFOV / verticalBeams;
        float horizontalStep = 360f / horizontalBeams;

        for (int h = 0; h < horizontalBeams; h++)
        {
            for (int v = 0; v < verticalBeams; v++)
            {
                float horizontalAngle = h * horizontalStep;
                float verticalAngle = (v * verticalStep) - (verticalFOV / 2);
                Vector3 direction = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * transform.forward;

                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance))
                {
                    if (hit.collider.CompareTag("Sensor"))
                    {
                        Debug.Log($"Ignored sensor: {hit.collider.name}");
                        continue;
                    }

                    // Convert the hit point to local coordinates
                    Vector3 localPoint = transform.InverseTransformPoint(hit.point);

                    // Apply Gaussian noise to the detected point
                    

                    // Rotate point to LiDAR's reference frame
                    Quaternion lidarRotation = transform.rotation;
                    Vector3 rotatedPoint = lidarRotation * localPoint;

                    // Convert to MATLAB-style coordinate system
                    Vector3 matlabPoint = new Vector3(rotatedPoint.z, -rotatedPoint.x, rotatedPoint.y);
                    scanPoints.Add(matlabPoint);

                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red, 0.1f);
                }
            }
        }

        Debug.Log($"Captured {scanPoints.Count} 3D points with Gaussian noise.");
    }

    float GaussianNoise(float mean, float stdDev)
    {
        // Box-Muller transform to generate Gaussian noise
        double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random value
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2); // Standard normal (0,1)
        return (float)(mean + stdDev * randStdNormal);
    }

    public void SaveLiDARData()
    {
        SimulateLiDAR();
        saveCount++;
        string filePath = Application.persistentDataPath + $"/{gameObject.name}_lidar_data{saveCount}.ply";

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("ply");
            writer.WriteLine("format ascii 1.0");
            writer.WriteLine($"element vertex {scanPoints.Count}");
            writer.WriteLine("property float x");
            writer.WriteLine("property float y");
            writer.WriteLine("property float z");
            writer.WriteLine("end_header");

            foreach (var point in scanPoints)
            {
                writer.WriteLine($"{point.x} {point.y} {point.z}");
            }
        }

        Debug.Log($"LiDAR data with noise saved to {filePath}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Vector3 point in scanPoints)
        {
            Gizmos.DrawSphere(point, 0.01f);
        }
    }
}



*/
using UnityEngine;
using System.Collections.Generic;
using System.IO; // Required for file handling

public class LiDAR3DSimulator : MonoBehaviour
{
    // Adjustable parameters
    public float verticalFOV = 45f;         // Vertical Field of View (degrees)
    public int verticalBeams = 128;         // Number of vertical laser beams
    public int horizontalBeams = 1000;      // Number of horizontal beams around 360°
    public float maxDistance = 20f;         // Maximum detection distance (meters)
    public KeyCode saveKey = KeyCode.L;     // Key to save the LiDAR data
    public KeyCode visualizeKey = KeyCode.J;// Key to visualize the scanned points

    public GameObject pointPrefab; // Prefab for visualization (small sphere)
    private List<Vector3> scanPoint = new List<Vector3>();
    private List<Vector3> scanPoints = new List<Vector3>(); // Stores scanned points
    private List<GameObject> spawnedPoints = new List<GameObject>(); // List of spawned point objects
    private static int saveCount = -1;      // Counter to generate unique filenames

    void Update()
    {
        if (Input.GetKeyDown(saveKey)) // Listen for the save key
        {
            Debug.Log("L key pressed"); // Confirm key press detection
            SaveLiDARData();
        }
        if (Input.GetKeyDown(visualizeKey))
        {
            Debug.Log("J key pressed - Visualizing scanned points");
            SimulateLiDAR(); // Generate data before visualization
            VisualizePoints(); // Show points in game scene
        }
    }

    void VisualizePoints()
    {
        // Clear previously spawned points
        foreach (var obj in spawnedPoints)
        {
            Destroy(obj);
        }
        spawnedPoints.Clear();

        // Instantiate spheres at scanned points
        foreach (Vector3 point in scanPoint)
        {
            GameObject pointObj = Instantiate(pointPrefab, point, Quaternion.identity);
            pointObj.transform.localScale = Vector3.one * 0.02f; // Adjust size if needed




            spawnedPoints.Add(pointObj);
        }
    }

    void SimulateLiDAR()
    {
        scanPoints.Clear(); // Clear previous scan data
        scanPoint.Clear();
        float verticalStep = verticalFOV / verticalBeams; // Vertical angular resolution
        float horizontalStep = 360f / horizontalBeams;    // Horizontal angular resolution

        for (int h = 0; h < horizontalBeams; h++) // Horizontal (Yaw)
        {
            for (int v = 0; v < verticalBeams; v++) // Vertical (Pitch)
            {
                // Calculate angles
                float horizontalAngle = h * horizontalStep; // Yaw
                float verticalAngle = (v * verticalStep) - (verticalFOV / 2); // Pitch

                // Calculate ray direction
                Vector3 direction = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * transform.forward;

                // Perform raycast
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance))
                {
                    // Check if the hit object has the "Sensor" tag
                    if (hit.collider.CompareTag("Sensor"))
                    {
                        Debug.Log($"Ignored sensor: {hit.collider.name}");
                        continue; // Skip this object
                    }

                    // Convert the hit point to local coordinates
                    Vector3 localPoint = transform.InverseTransformPoint(hit.point);

                    // Apply Gaussian noise to the detected point


                    // Rotate point to LiDAR's reference frame
                    //Quaternion lidarRotation = transform.rotation;
                    // Vector3 rotatedPoint = lidarRotation * localPoint;

                    // Convert to MATLAB-style coordinate system
                    Vector3 matlabPoint = new Vector3(localPoint.x, localPoint.y, localPoint.z);
                    scanPoints.Add(matlabPoint);

                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red, 0.1f);
                    // Add hit point to the list
                    scanPoint.Add(hit.point);

                    // Debug: Visualize the ray
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red, 0.1f);
                }
            }
        }

        Debug.Log($"Captured {scanPoints.Count} 3D points (excluding sensors).");
    }

    public void SaveLiDARData()
    {
        SimulateLiDAR(); // Generate data before saving
        saveCount++; // Increment the file counter

        // Create a unique filename for each save
        string filePath = Application.persistentDataPath + $"/{gameObject.name}data{saveCount}.ply";

        using (var writer = new StreamWriter(filePath))
        {
            // Write PLY header
            writer.WriteLine("ply");
            writer.WriteLine("format ascii 1.0");
            writer.WriteLine($"element vertex {scanPoints.Count}");
            writer.WriteLine("property float x");
            writer.WriteLine("property float y");
            writer.WriteLine("property float z");
            writer.WriteLine("end_header");

            // Write points
            foreach (var point in scanPoints)
            {
                writer.WriteLine($"{point.x} {point.y} {point.z}");
            }
        }

        Debug.Log($"LiDAR data saved to {filePath}");
    }
}
