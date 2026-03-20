using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryController : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(PlayThenLoad());
    }

    private IEnumerator PlayThenLoad()
    {
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene("load_scene_1");
    }
}
