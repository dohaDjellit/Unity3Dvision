using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for buttons in the scroll view
    public Transform sensorListPanel; // Content object of the scroll view
    public GameObject selectionMarkerPrefab; // Selection marker prefab (optional)

    private Dictionary<GameObject, GameObject> sensorToButtonMap = new Dictionary<GameObject, GameObject>(); // Map each sensor to its button
    private GameObject selectedSensor; // The currently selected sensor
    private GameObject currentMarker; // Selection marker instance

    public void AddSensor(GameObject sensor)
    {
        // Ensure the sensor isn't already added
        if (!sensorToButtonMap.ContainsKey(sensor))
        {
            // Instantiate a button for the sensor
            GameObject buttonObj = Instantiate(buttonPrefab, sensorListPanel);

            // Set the button's text to the sensor's name
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = sensor.name;
            }

            // Add a click listener to select the sensor when its button is clicked
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                // Use a local reference to prevent closure issues
                button.onClick.AddListener(() =>
                {
                    if (sensor != null)
                    {
                        SelectSensor(sensor);
                    }
                    else
                    {
                        Debug.LogWarning("Tried to select a sensor that has been destroyed.");
                    }
                });
            }

            // Map the sensor to its button for easy deletion
            sensorToButtonMap[sensor] = buttonObj;
        }
    }

    private void SelectSensor(GameObject sensor)
    {
        if (sensor == null)
        {
            Debug.LogWarning("Selected sensor is null.");
            return;
        }

        selectedSensor = sensor;

        // Highlight the selected sensor
        Debug.Log($"Selected Sensor: {sensor.name}");
        Renderer renderer = sensor.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }

        // Reset colors for other sensors
        foreach (var kvp in sensorToButtonMap)
        {
            if (kvp.Key != sensor && kvp.Key.TryGetComponent(out Renderer otherRenderer))
            {
                otherRenderer.material.color = Color.white;
            }
        }

        // Add or move the selection marker
        if (currentMarker == null)
        {
            currentMarker = Instantiate(selectionMarkerPrefab);
        }
        currentMarker.GetComponent<SelectionMarker>().SetTarget(sensor.transform);
    }

    public void DeleteSensor(GameObject sensor)
    {
        if (sensorToButtonMap.TryGetValue(sensor, out GameObject buttonObj))
        {
            // Remove the button's click listener
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }

            // Destroy the associated button
            Destroy(buttonObj);

            // Remove the sensor from the dictionary
            sensorToButtonMap.Remove(sensor);

            // Destroy the sensor from the scene
            Destroy(sensor);

            // Clear the selection marker if the deleted sensor was selected
            if (currentMarker != null && sensor == selectedSensor)
            {
                Destroy(currentMarker);
                selectedSensor = null; // Reset selection
            }

            Debug.Log($"Deleted Sensor: {sensor.name}");
        }
        else
        {
            Debug.LogError("Sensor not found in the dictionary!");
        }
    }

    private void Update()
    {
        // Check for Delete key press
        if (Input.GetKeyDown(KeyCode.Delete) && selectedSensor != null)
        {
            DeleteSensor(selectedSensor);
        }
    }
}
