using Assets.Code.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class FireballSkillChooseController : MonoBehaviour, IAutoAttackTypeSelectable
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
        Animator animator = _safina.GetComponent<Animator>();
        int state = animator.GetInteger("State");
        _canvas.gameObject.SetActive(false);

        if (state == 0)
        {
            animator.SetInteger("State", 4);
            PlayerCtrl.AttackType = PlayerCtrl.ChosenBasicAttact.Fire;
        }
        else if (state == 1)
        {
            animator.SetInteger("State", 5);
            PlayerCtrl.AttackType = PlayerCtrl.ChosenBasicAttact.Fire;
        }
    }
}
