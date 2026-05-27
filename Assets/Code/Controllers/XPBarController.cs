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

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int currentLvl = GameManager.Player != null ? GameManager.Player.GetCurrentLvl() : 0;
        double currentXP = GameManager.Player != null ? GameManager.Player.GetCurrentXP() : 0;

        _initialRightMask = _barRect.rect.width;
        
        double levelCap = _XPSO.LevelCaps[currentLvl];
        double xpRatio = levelCap > 0 ? (currentXP / levelCap) : 0;
        _currentRightMask = _initialRightMask - (_initialRightMask * xpRatio);

        Vector4 padding = _mask.padding;
        padding.z = (float)_currentRightMask;
        _mask.padding = padding;

        _XPIndicator.SetText($"{currentXP}/{levelCap}");
    }

    public void AddXP(long XP)
    {
        if (GameManager.Player == null) return;

        int currentLvl = GameManager.Player.GetCurrentLvl();
        double updatedXPValue = GameManager.Player.GetCurrentXP() + XP;
        double newRightMask = _currentRightMask - _initialRightMask * ((double)XP / (double)_XPSO.LevelCaps[currentLvl]);
        _currentRightMask -= _initialRightMask * ((double)XP / (double)_XPSO.LevelCaps[currentLvl]);
        Vector4 padding = _mask.padding;
        padding.z = (float)newRightMask;
        _mask.padding = padding;
        _XPIndicator.SetText($"{updatedXPValue}/{_XPSO.LevelCaps[currentLvl]}");
    }

    public void LevelUp()
    {
        if (GameManager.Player == null) return;

        int currentLvl = GameManager.Player.GetCurrentLvl();
        double updatedXPValue = GameManager.Player.GetCurrentXP();
        double newRightMask = _currentRightMask;
        Vector4 padding = _mask.padding;
        padding.z = (float)newRightMask;
        _mask.padding = padding;
        _XPIndicator.SetText($"{updatedXPValue}/{_XPSO.LevelCaps[currentLvl]}");
    }

    public void ResetMaskAfterLevelUp()
    {
        _initialRightMask = _barRect.rect.width;
        _currentRightMask = _initialRightMask;
    }
}
