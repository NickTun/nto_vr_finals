using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BPLAController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject handle;
    [SerializeField] private GameObject world;

    [Header("Settings")]
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float accelerationX = 20f;
    [SerializeField] private float accelerationY = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float gravity = 5f;
    [SerializeField] private float tempCoof = 0.1f;
    [SerializeField] private float weatherIntensity = 0.001f;
    public float ropeLength = 0.4f;
    [SerializeField] private float baseTemp = 24f;
    [SerializeField] private float consumptionCoof = 0.1f;

    [Header("Dynamic Values")]
    [SerializeField] private float fuel = 100f;
    public float currentTemp;
    public float handleDistance;

    [Header("GUI")]
    [SerializeField] private TMP_Text outsideTempObj;
    [SerializeField] private TMP_Text insideTempObj;
    [SerializeField] private TMP_Text fuelPrecentageObj;
    [SerializeField] private TMP_Text currentHeightObj;
    [SerializeField] private TMP_Text windSpeed;
    [SerializeField] private Transform windArrow;
    
    private Transform joystickTransform;
    private Transform handleTransform;
    private Transform worldTransform;
    private Rigidbody worldRb;
    private SpringJoint handleSpringJoint;

    private void Awake()
    {
        joystickTransform = joystick.GetComponent<Transform>();
        handleTransform = handle.GetComponent<Transform>();
        worldTransform = world.GetComponent<Transform>();
        worldRb = world.GetComponent<Rigidbody>();
        handleSpringJoint = handle.GetComponent<SpringJoint>();
        currentTemp = baseTemp;

        handleSpringJoint.minDistance = ropeLength;
        LeaveWind();
    }

    private Vector3 Move()
    {
        handleDistance = Vector3.Distance(
            handleTransform.position,
            handleSpringJoint.connectedBody.position
        );

        Vector3 rot = joystickTransform.localEulerAngles;

        float x = rot.x > 180f ? rot.x - 360f : rot.x;
        float z = rot.z > 180f ? rot.z - 360f : rot.z;

        float moveX = Mathf.Clamp(z / maxAngle, -1f, 1f);
        float moveZ = Mathf.Clamp(-x / maxAngle, -1f, 1f); 
        float heatMovement = Mathf.Min(ropeLength - handleDistance + handleDistance/10, 0);
        float moveY = -currentTemp*tempCoof;

        Vector3 gravityMove = new Vector3(0, gravity, 0);
        Vector3 move = new Vector3(0,0,0);

        float tempAtHeight = baseTemp + worldTransform.position.y/2;
        
        if(fuel > 0) {
            currentTemp = Mathf.Min(Mathf.Lerp(currentTemp, tempAtHeight, weatherIntensity) - heatMovement, 100);
            fuel = Mathf.Max(0, fuel - (new Vector3(moveX, heatMovement, moveZ).magnitude)*consumptionCoof);
            move = new Vector3(moveX*accelerationX, moveY*accelerationY, moveZ*accelerationX);
        }

        worldRb.AddForce(gravityMove + move, ForceMode.Acceleration);

        UpdateText(tempAtHeight);

        // Debug.Log(moveY + " " + currentTemp + " " + fuel);
            
        return move;
    }

    private void UpdateText(float outsideTemp) {
        outsideTempObj.text = Mathf.Round(outsideTemp * 100f) / 100f + "°C";
        insideTempObj.text = Mathf.Round(currentTemp * 100f) / 100f + "°C";
        fuelPrecentageObj.text = Mathf.Round(fuel * 100f) / 100f + "%";
        currentHeightObj.text = -Mathf.Round(worldTransform.position.y * 100f) / 100f + "m";
    }

    private void FixedUpdate()
    {
        Move();

        Vector3 velocity = worldRb.velocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontal.magnitude > maxSpeed)
        {
            horizontal = horizontal.normalized * maxSpeed;
        }
    }

    private void ApplyVector(Transform obj, Vector3 direction, float heightMultiplier = 1f)
    {
        if (direction == Vector3.zero) return;
        obj.rotation = Quaternion.LookRotation(direction);
    }

    public void ApplyWind(Vector3 wind) {
        worldRb.AddForce(-wind, ForceMode.Acceleration);
        float speed = wind.magnitude;
        windSpeed.text = Mathf.Round(speed * 100f) / 100f + "м/с";
        Vector3 newScale = windArrow.localScale;
        newScale.z = speed/200;
        windArrow.localScale = newScale;
        ApplyVector(windArrow, wind);
    }


    public void LeaveWind() {
        windSpeed.text = "0м/c";
        Vector3 newScale = windArrow.localScale;
        newScale.z = 0f;
        windArrow.localScale = newScale;
        ApplyVector(windArrow, new Vector3(0,0,0));
    }
}

