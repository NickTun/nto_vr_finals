using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AngularSpring : MonoBehaviour
{
    [Tooltip("The target rotation we spring towards")]
    public Quaternion targetRotation = Quaternion.identity;

    [Tooltip("Spring strength (higher = stiffer)")]
    public float spring = 200f;

    [Tooltip("Damper (higher = less oscillation)")]
    public float damper = 20f;

    [Tooltip("Optional: world up instead of fixed target")]
    public bool springToWorldUp = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Optional: capture starting rotation as target
        // targetRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        Quaternion current = transform.rotation;
        Quaternion desired = springToWorldUp ? Quaternion.FromToRotation(transform.up, Vector3.up) * current : targetRotation;

        // Shortest rotation difference
        Quaternion delta = desired * Quaternion.Inverse(current);

        // Convert to axis-angle
        delta.ToAngleAxis(out float angle, out Vector3 axis);

        // Fix angle range (-180° → 180°)
        if (angle > 180f) { angle -= 360f; }

        // Torque = spring * error_angle + damper * angular_velocity
        if (angle != 0f)
        {
            Vector3 torque = axis * (angle * Mathf.Deg2Rad * spring) - rb.angularVelocity * damper;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    // Optional: call this to change target at runtime
    public void SetTargetRotation(Quaternion newTarget)
    {
        targetRotation = newTarget;
    }
}