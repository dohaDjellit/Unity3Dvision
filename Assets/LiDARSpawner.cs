using UnityEngine;

public class LiDARSpawner : MonoBehaviour
{
    public GameObject lidarPrefab; // Prefab for the LiDAR
    private int lidarCount = 0;   // Counter for naming LiDARs

    public void SpawnLiDAR()
    {
        // Increment the counter for each spawned LiDAR
        lidarCount++;

        // Spawn a new LiDAR object
        GameObject newLiDAR = Instantiate(lidarPrefab);

        // Assign a sequential name
        newLiDAR.name = $"3DLidar{lidarCount}";

        // Ensure it has required components
        if (newLiDAR.GetComponent<SelectableLiDAR>() == null)
        {
            newLiDAR.AddComponent<SelectableLiDAR>();
        }

        Debug.Log($"Spawned new LiDAR: {newLiDAR.name}");

        // Notify SensorManager about the new LiDAR
        SensorManager sensorManager = FindObjectOfType<SensorManager>();
        if (sensorManager != null)
        {
            sensorManager.AddSensor(newLiDAR);
        }
    }
}
