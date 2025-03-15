using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ObjectInspector : MonoBehaviour
{
    public TMP_InputField posXField, posYField, posZField;
    public TMP_InputField rotXField, rotYField, rotZField;
    public TMP_InputField scaleXField, scaleYField, scaleZField;

    private GameObject selectedObject;

    // This method is called to set the selected object and update the input fields
    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        UpdateInspectorFields();
    }

    // Update input fields with the selected object's current values
    private void UpdateInspectorFields()
    {
        if (selectedObject == null)
        {
            ClearFields();
            return;
        }

        // Update position fields
        Vector3 pos = selectedObject.transform.position;
        if (posXField != null) posXField.text = pos.x.ToString("F2");
        if (posYField != null) posYField.text = pos.y.ToString("F2");
        if (posZField != null) posZField.text = pos.z.ToString("F2");

        // Update rotation fields
        Vector3 rot = selectedObject.transform.eulerAngles;
        if (rotXField != null) rotXField.text = rot.x.ToString("F2");
        if (rotYField != null) rotYField.text = rot.y.ToString("F2");
        if (rotZField != null) rotZField.text = rot.z.ToString("F2");

        // Update scale fields
        Vector3 scale = selectedObject.transform.localScale;
        if (scaleXField != null) scaleXField.text = scale.x.ToString("F2");
        if (scaleYField != null) scaleYField.text = scale.y.ToString("F2");
        if (scaleZField != null) scaleZField.text = scale.z.ToString("F2");
    }

    // Apply changes from input fields to the selected object's parameters
    public void ApplyChanges()
    {
        if (selectedObject == null) return;

        // Apply position
        if (float.TryParse(posXField.text, out float posX) &&
            float.TryParse(posYField.text, out float posY) &&
            float.TryParse(posZField.text, out float posZ))
        {
            selectedObject.transform.position = new Vector3(posX, posY, posZ);
        }

        // Apply rotation
        if (float.TryParse(rotXField.text, out float rotX) &&
            float.TryParse(rotYField.text, out float rotY) &&
            float.TryParse(rotZField.text, out float rotZ))
        {
            selectedObject.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
        }

        // Apply scale
        if (float.TryParse(scaleXField.text, out float scaleX) &&
            float.TryParse(scaleYField.text, out float scaleY) &&
            float.TryParse(scaleZField.text, out float scaleZ))
        {
            selectedObject.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }

    // Clear the input fields
    public void ClearFields()
    {
        if (posXField != null) posXField.text = "";
        if (posYField != null) posYField.text = "";
        if (posZField != null) posZField.text = "";

        if (rotXField != null) rotXField.text = "";
        if (rotYField != null) rotYField.text = "";
        if (rotZField != null) rotZField.text = "";

        if (scaleXField != null) scaleXField.text = "";
        if (scaleYField != null) scaleYField.text = "";
        if (scaleZField != null) scaleZField.text = "";

        Debug.Log("Inspector fields cleared.");
    }

    // Getter for the selected object
    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }
}

