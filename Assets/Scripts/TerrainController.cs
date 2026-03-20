using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainController : MonoBehaviour
{
    [SerializeField] private Transform origin;
    private Transform pos;

    void Awake() {
        pos = GetComponent<Transform>();
    }

    void FixedUpdate() {
        pos.position = origin.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " touched terrain");
        SceneManager.LoadScene("load_scene_1");
    }
}
