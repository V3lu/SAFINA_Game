using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private int _startingHearts = 7;

    private List<Animator> _heartAnimators;
    private int _currentHearts;

    public int CurrentHearts => _currentHearts;

    void Start()
    {
        _heartAnimators = new List<Animator>();

        // Destroy any placeholder children from the Editor
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn new hearts
        if (_heartPrefab != null)
        {
            for (int i = 0; i < _startingHearts; i++)
            {
                GameObject heart = Instantiate(_heartPrefab, transform);
                Animator anim = heart.GetComponent<Animator>();
                if (anim != null)
                {
                    _heartAnimators.Add(anim);
                }
            }
        }
        else
        {
            Debug.LogWarning("Heart Prefab missing! Please assign it in the Inspector on the HPBar GameObject.");
        }

        _currentHearts = _heartAnimators.Count;
    }

    public void DrainHearts(int amount)
    {
        StartCoroutine(DrainHeartsCoroutine(amount));
    }

    private IEnumerator DrainHeartsCoroutine(int amount)
    {
        Debug.Log($"[HPBarController] DrainHeartsCoroutine started with amount: {amount}, current hearts: {_currentHearts}, animators count: {(_heartAnimators != null ? _heartAnimators.Count : 0)}");
        for (int i = 0; i < amount; i++)
        {
            if (_currentHearts > 0)
            {
                _currentHearts--;
                Debug.Log($"[HPBarController] Draining heart index {_currentHearts}");
                if (_heartAnimators != null && _heartAnimators.Count > _currentHearts)
                {
                    _heartAnimators[_currentHearts].SetTrigger("Drain");
                    Debug.Log($"[HPBarController] Set Drain trigger on heart index {_currentHearts}");
                }
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                Debug.Log($"[HPBarController] No more hearts to drain.");
                break;
            }
        }
    }


    public void RestoreAllHearts()
    {
        StopAllCoroutines();
        if (_heartAnimators == null) return;
        foreach (var anim in _heartAnimators)
        {
            if (anim != null)
            {
                anim.ResetTrigger("Drain");
                anim.Play("HeartFull", 0, 0f);
            }
        }
        _currentHearts = _heartAnimators.Count;
    }
}
