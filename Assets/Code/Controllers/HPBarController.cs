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
        for (int i = 0; i < amount; i++)
        {
            if (_currentHearts > 0)
            {
                _currentHearts--;
                _heartAnimators[_currentHearts].SetTrigger("Drain");
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                break;
            }
        }
    }

    public void RestoreAllHearts()
    {
        foreach (var anim in _heartAnimators)
        {
            anim.Play("HeartFull", 0, 0f);
        }
        _currentHearts = _heartAnimators.Count;
    }
}
