using Assets.Code.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class FireballSkillChooseController : MonoBehaviour, IAutoAttackTypeSelectable
{
    [SerializeField] Image _targetImage;
    [SerializeField] Sprite _unhooveredSprite;
    [SerializeField] Sprite _hooveredSprite;
    [SerializeField] Canvas _canvas;
     

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
