using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarController : MonoBehaviour
{
    [SerializeField] RectTransform _barRect;
    [SerializeField] RectMask2D _mask;
    [SerializeField] TMP_Text _XPIndicator;
    [SerializeField] XPSO _XPSO;

    static double _initialRightMask;
    static double _currentRightMask;

    PlayerCtrl _playerCtrl;

    void Start()
    {
        _XPIndicator.SetText($"{GameManager.Player.GetCurrentXP()}/{_XPSO.LevelCaps[0]}");
        _initialRightMask = _barRect.rect.width;
        _currentRightMask = _initialRightMask;
        _playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }

    public void AddXP(long XP)
    {
        double updatedXPValue = GameManager.Player.GetCurrentXP() + XP;
        double newRightMask = _currentRightMask - _initialRightMask * ((double)XP / (double)_XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]);
        _currentRightMask -= _initialRightMask * ((double)XP / (double)_XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]);
        Vector4 padding = _mask.padding;
        padding.z = (float)newRightMask;
        _mask.padding = padding;
        _XPIndicator.SetText($"{updatedXPValue}/{_XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]}");
    }

    public void LevelUp()
    {
        double updatedXPValue = GameManager.Player.GetCurrentXP();
        double newRightMask = _currentRightMask;
        Vector4 padding = _mask.padding;
        padding.z = (float)newRightMask;
        _mask.padding = padding;
        _XPIndicator.SetText($"{updatedXPValue}/{_XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]}");
    }

    public void ResetMaskAfterLevelUp()
    {
        _initialRightMask = _barRect.rect.width;
        _currentRightMask = _initialRightMask;
    }
}
