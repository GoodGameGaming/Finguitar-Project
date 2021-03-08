using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneHelper : MonoBehaviour
{
    public void SwitchScene()
    {
        //We load the alternative scene
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex == 0 ? 1 : 0);
    }
}
