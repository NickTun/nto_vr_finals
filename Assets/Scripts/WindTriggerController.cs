using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTriggerController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Vector3 wind;
    [SerializeField] private BPLAController bplaController;

    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")) {
            bplaController.ApplyWind(wind);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) {
            bplaController.LeaveWind();
        }
    }
}
