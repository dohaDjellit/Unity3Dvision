using UnityEngine;

public class SelectableSensor : MonoBehaviour
{
    private ObjectInspector inspector;

    void Start()
    {
        // Find the ObjectInspector in the scene
        inspector = FindObjectOfType<ObjectInspector>();
    }

    void OnMouseDown()
    {
        // Set this object as the selected object in the inspector when clicked
        if (inspector != null)
        {
            inspector.SetSelectedObject(gameObject);
        }
    }

   
}
