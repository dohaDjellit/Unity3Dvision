using UnityEngine;

public class SelectionMarker : MonoBehaviour
{
    private Transform target; // The sensor the marker follows

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target != null)
        {
            float verticalOffset = 0.1f; // 
            transform.position = target.position + Vector3.up * verticalOffset;
        }
    }
}

