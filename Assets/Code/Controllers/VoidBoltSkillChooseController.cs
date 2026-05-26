using Assets.Code.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VoidBoltSkillChooseController : MonoBehaviour, IAutoAttackTypeSelectable
{
    [SerializeField] Image _targetImage;
    [SerializeField] Sprite _unhooveredSprite;
    [SerializeField] Sprite _hooveredSprite;
    [SerializeField] Canvas _canvas;
    [SerializeField] Canvas _barsCanvas;
    [SerializeField] Canvas _tutorialCanvas;

    static GameObject _safina;


    void Start()
    {
        _safina = GameObject.FindGameObjectWithTag("Player");
    }
    public void Hoovered()
    {
        _targetImage.sprite = _hooveredSprite;
    }

    public void Unhoovered()
    {
        _targetImage.sprite = _unhooveredSprite;
    }
    public void Selected()
    {
        _canvas.gameObject.SetActive(false);
        if (_tutorialCanvas != null)
        {
            _tutorialCanvas.gameObject.SetActive(false);
        }
        // Fallback: find BarsCanvas at runtime if not assigned via Inspector
        if (_barsCanvas == null)
        {
            var found = GameObject.Find("BarsCanvas");
            if (found != null) _barsCanvas = found.GetComponent<Canvas>();
        }
        if (_barsCanvas != null)
        {
            _barsCanvas.enabled = true;
        }
        Time.timeScale = 1f;

        // Notify HUDManager that attack was selected
        if (HUDManager.Instance != null)
            HUDManager.Instance.OnAttackSelected();

        Animator animator = _safina.GetComponent<Animator>();
        int state = animator.GetInteger("State");
        if (state == 0)
        {
            animator.SetInteger("State", 6);
            PlayerCtrl.AttackType = PlayerCtrl.ChosenBasicAttact.Void;
        }
        else if (state == 1)
        {
            animator.SetInteger("State", 7);
            PlayerCtrl.AttackType = PlayerCtrl.ChosenBasicAttact.Void;
        }
    }
}
