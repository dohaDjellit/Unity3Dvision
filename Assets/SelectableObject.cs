using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private ObjectInspector inspector;

    void Start()
    {
        // Find the ObjectInspector in the scene
        inspector = FindObjectOfType<ObjectInspector>();
    }

    void Update()
    {
        // Check if this object is currently selected in the inspector
        if (inspector != null && inspector.GetSelectedObject() == gameObject)
        {
            // Delete the object if the Delete key is pressed
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteObject();
            }
        }
    }

    void OnMouseDown()
    {
        // Set this object as the selected object in the inspector when clicked
        if (inspector != null)
        {
            inspector.SetSelectedObject(gameObject);
        }
    }

    private void DeleteObject()
    {
        // Remove the object from the scene
        Debug.Log($"Deleting object: {gameObject.name}");
        Destroy(gameObject);

        // Clear the selection in the inspector
        if (inspector != null)
        {
            inspector.ClearFields(); // Reset the inspector UI
        }
    }
}
