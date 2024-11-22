using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadTargetScene());
    }
    IEnumerator LoadTargetScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");

        while (!operation.isDone)
        {
            // Update progress bar or animations here if needed
            yield return null;
        }
    }
}