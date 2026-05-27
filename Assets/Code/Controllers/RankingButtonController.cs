using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RankingButtonController : MonoBehaviour
{
    [SerializeField] Image _targetImage;
    [SerializeField] Sprite _unhooveredSprite;
    [SerializeField] Sprite _hooveredSprite;
    [SerializeField] Canvas _startMenuCanvas;

    public void Init(Image targetImage, Sprite unhoovered, Sprite hoovered, Canvas startMenuCanvas)
    {
        _targetImage = targetImage;
        _unhooveredSprite = unhoovered;
        _hooveredSprite = hoovered;
        _startMenuCanvas = startMenuCanvas;
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
        if (_startMenuCanvas == null) return;

        // Create Panel Overlay
        GameObject overlayGO = new GameObject("HighscoreOverlay");
        overlayGO.layer = 5; // UI layer
        RectTransform overlayRect = overlayGO.AddComponent<RectTransform>();
        overlayRect.SetParent(_startMenuCanvas.transform, false);
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.anchoredPosition = Vector2.zero;
        overlayRect.sizeDelta = Vector2.zero;

        // Add semi-transparent background image
        Image overlayImg = overlayGO.AddComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.85f);
        overlayImg.raycastTarget = true;

        // Create Panel Container (box)
        GameObject containerGO = new GameObject("Container");
        containerGO.layer = 5;
        RectTransform containerRect = containerGO.AddComponent<RectTransform>();
        containerRect.SetParent(overlayGO.transform, false);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(500f, 350f);

        Image containerImg = containerGO.AddComponent<Image>();
        containerImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        // Add outline to container
        Outline containerOutline = containerGO.AddComponent<Outline>();
        containerOutline.effectColor = new Color(1f, 0.84f, 0f, 1f); // Gold outline
        containerOutline.effectDistance = new Vector2(3f, -3f);

        // Get Pixelify Font Asset
        TMP_FontAsset font = null;
        if (HUDManager.Instance != null)
        {
            var field = typeof(HUDManager).GetField("_pixelifyFontAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                font = field.GetValue(HUDManager.Instance) as TMP_FontAsset;
            }
        }

        // Title text
        GameObject titleGO = new GameObject("TitleText");
        titleGO.layer = 5;
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.SetParent(containerGO.transform, false);
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);
        titleRect.sizeDelta = new Vector2(400f, 50f);

        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        if (font != null) titleText.font = font;
        titleText.text = "<color=#FFD700>HIGH SCORES</color>";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;

        // Content text (Highscore)
        GameObject scoreGO = new GameObject("ScoreText");
        scoreGO.layer = 5;
        RectTransform scoreRect = scoreGO.AddComponent<RectTransform>();
        scoreRect.SetParent(containerGO.transform, false);
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = new Vector2(0f, 10f);
        scoreRect.sizeDelta = new Vector2(450f, 80f);

        TextMeshProUGUI scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
        if (font != null) scoreText.font = font;
        
        int highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText.text = $"Best Survival Time:\n<color=#00FFFF>{highscore} seconds</color>";
        scoreText.fontSize = 24;
        scoreText.alignment = TextAlignmentOptions.Center;

        // Close button
        GameObject closeButtonGO = new GameObject("CloseButton");
        closeButtonGO.layer = 5;
        RectTransform closeRect = closeButtonGO.AddComponent<RectTransform>();
        closeRect.SetParent(containerGO.transform, false);
        closeRect.anchorMin = new Vector2(0.5f, 0f);
        closeRect.anchorMax = new Vector2(0.5f, 0f);
        closeRect.anchoredPosition = new Vector2(0f, 40f);
        closeRect.sizeDelta = new Vector2(200f, 50f);

        TextMeshProUGUI closeText = closeButtonGO.AddComponent<TextMeshProUGUI>();
        if (font != null) closeText.font = font;
        closeText.text = "<color=#FFD700>[ Close ]</color>";
        closeText.fontSize = 28;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.raycastTarget = true;

        // Hover scale and click to close
        EventTrigger trigger = closeButtonGO.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => closeText.text = "<color=#FFFFFF>[ Close ]</color>");
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => closeText.text = "<color=#FFD700>[ Close ]</color>");
        trigger.triggers.Add(exit);

        EventTrigger.Entry click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        click.callback.AddListener(_ => Destroy(overlayGO));
        trigger.triggers.Add(click);
    }
}
