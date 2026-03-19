using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform object1; // Assign your anchor object here in the Inspector
    public Transform object2; // Assign your joint object here in the Inspector

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Set the number of points to 2 (start and end)
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (object1 != null && object2 != null)
        {
            // Update the positions of the line renderer every frame
            lineRenderer.SetPosition(0, object1.position);
            lineRenderer.SetPosition(1, object2.position);
        }
    }
}