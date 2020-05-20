using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLoadScene : MonoBehaviour
{
    public void LoadTimeTrial() {
        SceneManager.LoadSceneAsync("ForestTrack");
    }

    public void Quit() {
        Application.Quit();
    }
}
