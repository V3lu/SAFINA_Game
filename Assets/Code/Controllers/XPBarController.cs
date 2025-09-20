using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarController : MonoBehaviour
{
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private TMP_Text _XPIndicator;
    [SerializeField] private XPSO _XPSO;

    private static double _initialRightMask;

    private PlayerCtrl _playerCtrl;

    void Start()
    {
        _XPIndicator.SetText($"{GameManager.Player.GetCurrentXP()}/{_XPSO.LevelCaps[0]}");
        _initialRightMask = _barRect.rect.width;
        _playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }

    public void AddXP(double XP)
    {
        double updatedXPValue = GameManager.Player.GetCurrentXP() + XP;
        double newRightMask = _initialRightMask - _initialRightMask * (XP / _XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]);
        _initialRightMask -= _initialRightMask * (XP / _XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]);
        Vector4 padding = _mask.padding;
        padding.z = (float)newRightMask;
        _mask.padding = padding;
        _XPIndicator.SetText($"{updatedXPValue}/{_XPSO.LevelCaps[_playerCtrl.GetCurrentLvl()]}");
    }

    public void ResetMaskAfterLevelUp()
    {
        _initialRightMask = _barRect.rect.width;
    }
}
