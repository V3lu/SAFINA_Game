using Assets.Code.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VoidBoltSkillChooseController : MonoBehaviour, IAutoAttackTypeSelectable
{
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite _unhooveredSprite;
    [SerializeField] private Sprite _hooveredSprite;
    [SerializeField] private Canvas _canvas;


    private static GameObject _safina;


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
