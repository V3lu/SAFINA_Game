using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LeboliaMorass");
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float duration = 3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressBar.value = elapsed / duration; // bar fills over 3 seconds
            yield return null;
        }

        progressBar.value = 1f;
        op.allowSceneActivation = true;
    }
}
