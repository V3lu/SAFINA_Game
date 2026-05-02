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
        // Stop the game (mobs won't spawn, player can't move)
        Time.timeScale = 0f;

        // Ensure starting canvas setup
        _basicAttackChoosingCanvas.gameObject.SetActive(false);
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
        _startMenuCanvas.gameObject.SetActive(false);
        _basicAttackChoosingCanvas.gameObject.SetActive(true);
    }
}