using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadStartScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadStartScene()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("StartGame");
    }
}
