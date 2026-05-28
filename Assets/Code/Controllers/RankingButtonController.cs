using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Code.Utils;
using System.Collections.Generic;

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
        containerRect.sizeDelta = new Vector2(700f, 550f);

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
        titleRect.anchoredPosition = new Vector2(0f, -35f);
        titleRect.sizeDelta = new Vector2(450f, 50f);

        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        if (font != null) titleText.font = font;
        titleText.text = "<color=#FFD700>HIGH SCORES</color>";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;

        // Column header row
        float headerY = -75f;
        CreateRowTexts(containerGO, font, headerY, "#", "TAG", "STATUS", "TIME", 
            new Color(0.7f, 0.7f, 0.7f), 20);

        // Load ranking data
        RankingData data = RankingData.Load();
        List<RankingEntry> top = data.GetTopEntries(10);

        if (top.Count == 0)
        {
            // No records message
            GameObject noDataGO = new GameObject("NoDataText");
            noDataGO.layer = 5;
            RectTransform noDataRect = noDataGO.AddComponent<RectTransform>();
            noDataRect.SetParent(containerGO.transform, false);
            noDataRect.anchorMin = new Vector2(0.5f, 0.5f);
            noDataRect.anchorMax = new Vector2(0.5f, 0.5f);
            noDataRect.anchoredPosition = new Vector2(0f, 20f);
            noDataRect.sizeDelta = new Vector2(400f, 50f);

            TextMeshProUGUI noDataText = noDataGO.AddComponent<TextMeshProUGUI>();
            if (font != null) noDataText.font = font;
            noDataText.text = "<color=#888888>No records yet</color>";
            noDataText.fontSize = 24;
            noDataText.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            for (int i = 0; i < top.Count; i++)
            {
                float rowY = -115f - (i * 35f);
                Color rowColor = (i == 0) ? new Color(1f, 0.84f, 0f) : new Color(0f, 1f, 1f); // Gold for #1, cyan for rest

                int mins = Mathf.FloorToInt(top[i].time / 60f);
                int secs = Mathf.FloorToInt(top[i].time % 60f);
                string timeStr = $"{mins:00}:{secs:00}";
                string statusStr = top[i].won ? "<color=#33FF33>WON</color>" : "<color=#FF3333>DIED</color>";

                CreateRowTexts(containerGO, font, rowY, $"{i + 1}.", top[i].tag, statusStr, timeStr, rowColor, 22);
            }
        }

        // Close button
        GameObject closeButtonGO = new GameObject("CloseButton");
        closeButtonGO.layer = 5;
        RectTransform closeRect = closeButtonGO.AddComponent<RectTransform>();
        closeRect.SetParent(containerGO.transform, false);
        closeRect.anchorMin = new Vector2(0.5f, 0f);
        closeRect.anchorMax = new Vector2(0.5f, 0f);
        closeRect.anchoredPosition = new Vector2(0f, 35f);
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

    /// <summary>
    /// Creates a row with 4 text columns: rank, tag, status, time
    /// </summary>
    void CreateRowTexts(GameObject parent, TMP_FontAsset font, float yPos, 
        string rankStr, string tagStr, string statusStr, string timeStr, Color color, int fontSize)
    {
        // Rank column (left)
        CreateColumnText(parent, font, yPos, -260f, 60f, rankStr, color, fontSize, TextAlignmentOptions.Center);
        // Tag column (mid-left)
        CreateColumnText(parent, font, yPos, -100f, 200f, tagStr, color, fontSize, TextAlignmentOptions.Left);
        // Status column (mid-right)
        CreateColumnText(parent, font, yPos, 80f, 120f, statusStr, color, fontSize, TextAlignmentOptions.Center);
        // Time column (right)
        CreateColumnText(parent, font, yPos, 220f, 120f, timeStr, color, fontSize, TextAlignmentOptions.Center);
    }

    void CreateColumnText(GameObject parent, TMP_FontAsset font, float yPos, float xPos, 
        float width, string text, Color color, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject go = new GameObject("Col_" + text);
        go.layer = 5;
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.SetParent(parent.transform, false);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(xPos, yPos);
        rect.sizeDelta = new Vector2(width, 30f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
    }
}
