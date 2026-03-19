using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    private SpringJoint spring;
    private Transform transform;
    private float distance;
    private float originalDistance;
    [SerializeField] private BPLAController BPLA;

    public void Awake() {
        spring = GetComponent<SpringJoint>();
        transform = GetComponent<Transform>();
        originalDistance = BPLA.ropeLength;
    }

    public void FixedUpdate() {
        distance = Vector3.Distance(
            transform.position,
            spring.connectedBody.position
        );
    }

    public void Activate() {
        spring.minDistance = distance;
    }

    public void Deactivate() {
        spring.minDistance = originalDistance;
    }
}
