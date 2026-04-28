using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// REMINDER: Make sure to manually add and apply the "HPBar" tag to the HPBar GameObject in the Editor!
public class HPBarController : MonoBehaviour
{
    private List<Animator> _heartAnimators;
    private int _currentHearts;

    public int CurrentHearts => _currentHearts;

    void Start()
    {
        _heartAnimators = new List<Animator>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Animator anim = transform.GetChild(i).GetComponent<Animator>();
            if (anim != null)
            {
                _heartAnimators.Add(anim);
            }
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
