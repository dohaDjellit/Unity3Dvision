using UnityEngine;

public class SelectableLiDAR : MonoBehaviour
{
    private LiDARParameterAdjuster parameterAdjuster;
    private ObjectInspector objectInspector;

    private static GameObject selectedLiDAR; // Tracks the currently selected LiDAR

    void Start()
    {
        // Find the LiDARParameterAdjuster and ObjectInspector scripts in the scene
        parameterAdjuster = FindObjectOfType<LiDARParameterAdjuster>();
        objectInspector = FindObjectOfType<ObjectInspector>();

        if (parameterAdjuster == null)
        {
            Debug.LogError("LiDARParameterAdjuster script not found in the scene!");
        }
        if (objectInspector == null)
        {
            Debug.LogError("ObjectInspector script not found in the scene!");
        }
    }
/*
    void Update()
    {
        // Handle delete functionality for the currently selected LiDAR
        if (selectedLiDAR == gameObject && Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteObject();
        }
    }*/

    void OnMouseDown()
    {
        // Handle transform adjustments (position, rotation, scale)
        if (objectInspector != null)
        {
            objectInspector.SetSelectedObject(gameObject);
            Debug.Log($"Object {gameObject.name} selected for transform adjustments.");
        }

        // Handle LiDAR parameter adjustments
        LiDAR3DSimulator lidar = GetComponent<LiDAR3DSimulator>();
        if (lidar != null && parameterAdjuster != null)
        {
            parameterAdjuster.SelectLidar(lidar);
            Debug.Log($"LiDAR {lidar.name} selected for parameter adjustments.");
        }

        // Set this LiDAR as the selected one
        selectedLiDAR = gameObject;
    }
    /*
    private void DeleteLiDAR()
    {
        Debug.Log($"Deleting LiDAR: {gameObject.name}");
        Destroy(gameObject);

        // Clear the selection in the ObjectInspector and LiDARParameterAdjuster
        if (objectInspector != null)
        {
            objectInspector.ClearFields();
        }
        if (parameterAdjuster != null)
        {
            parameterAdjuster.DeselectLidar();
        }

        // Clear the selected LiDAR reference
        selectedLiDAR = null;
    }*/
}
