using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnGrab : MonoBehaviour
{
    private Transform originalParent;
    private bool grabbed = false;

    // Call when the grab starts
    public void OnGrabStart()
    {
        if (grabbed) return;
        originalParent = transform.parent;
        // detach from platform but keep world position
        transform.SetParent(null, true);
        grabbed = true;
    }

    // Call when the grab ends
    public void OnGrabEnd()
    {
        if (!grabbed) return;
        // reattach to platform
        transform.SetParent(originalParent, true);
        grabbed = false;
    }
}