using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoader : MonoBehaviour
{
    public void Tutorial() {
        SceneManager.LoadScene("Scene_plato");
    }

    public void Level1() {
        SceneManager.LoadScene("Gorniy_pereval_2");
    }

    public void Level2() {
        SceneManager.LoadScene("Scene_HolodniyFront");
    }

    public void Quit() {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        // This will run in a built game
        #else
            Application.Quit();
        #endif
    }
}
