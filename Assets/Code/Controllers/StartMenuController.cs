using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("UI Elemenets")]
    [SerializeField] Image _targetImage;
    [SerializeField] Sprite _unhooveredSprite;
    [SerializeField] Sprite _hooveredSprite;

    [Header("Canvases")]
    [SerializeField] Canvas _startMenuCanvas;
    [SerializeField] Canvas _basicAttackChoosingCanvas;
    [SerializeField] Canvas _barsCanvas;

    void Start()
    {
        // Runtime fallbacks — find canvases if not set via Inspector
        if (_startMenuCanvas == null)
        {
            var found = GameObject.Find("StartMenuCanvas");
            if (found != null) _startMenuCanvas = found.GetComponent<Canvas>();
        }
        if (_basicAttackChoosingCanvas == null)
        {
            var found = GameObject.Find("BasicAttackChoosingCanvas");
            if (found != null) _basicAttackChoosingCanvas = found.GetComponent<Canvas>();
        }
        if (_barsCanvas == null)
        {
            var found = GameObject.Find("BarsCanvas");
            if (found != null) _barsCanvas = found.GetComponent<Canvas>();
        }

        // Stop the game (mobs won't spawn, player can't move)
        Time.timeScale = 0f;

        // Ensure starting canvas setup
        if (_basicAttackChoosingCanvas != null)
            _basicAttackChoosingCanvas.gameObject.SetActive(false);
        if (_startMenuCanvas != null)
            _startMenuCanvas.gameObject.SetActive(true);

        // Hide bars but keep GameObjects active so scripts can find them
        if (_barsCanvas != null)
        {
            _barsCanvas.enabled = false;
        }
    }

    public void Hoovered()
    {
        if (_targetImage != null && _hooveredSprite != null)
        {
            _targetImage.sprite = _hooveredSprite;
        }
    }

    public void Unhoovered()
    {
        if (_targetImage != null && _unhooveredSprite != null)
        {
            _targetImage.sprite = _unhooveredSprite;
        }
    }

    public void Selected()
    {
        // Hide start menu and show attack choosing canvas
        if (_startMenuCanvas != null)
            _startMenuCanvas.gameObject.SetActive(false);
        if (_basicAttackChoosingCanvas != null)
            _basicAttackChoosingCanvas.gameObject.SetActive(true);

        // Transition the tutorial to the AttackSelect step
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.TransitionToStep(HUDManager.TutorialStep.AttackSelect);
        }

        // Rebind player HUD controllers (HPBar may have been created after Player.Start())
        if (GameManager.Player != null)
        {
            GameManager.Player.RebindHUDControllers();
        }
    }
}