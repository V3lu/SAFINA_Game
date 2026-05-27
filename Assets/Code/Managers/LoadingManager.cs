using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    [SerializeField] string _targetScene = "LeboliaMorass";
    [SerializeField] float _loadingDuration = 3f;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(_targetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (elapsed < _loadingDuration)
        {
            elapsed += Time.deltaTime;
            progressBar.value = elapsed / _loadingDuration;
            yield return null;
        }

        progressBar.value = 1f;
        op.allowSceneActivation = true;
    }
}

