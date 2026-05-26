using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Persistent HUD manager that survives scene transitions.
/// Bootstraps itself automatically via [RuntimeInitializeOnLoadMethod].
/// - Creates HPBar at runtime (hearts-based health display)
/// - Manages BarsCanvas persistence across scenes (DontDestroyOnLoad)
/// - Shows StartMenu only on CrystalinePath (the first gameplay scene)
/// </summary>
public class HUDManager : MonoBehaviour
{
    static HUDManager _instance;
    
    // Persistent references
    Canvas _barsCanvas;
    HPBarController _hpBarController;
    
    // Start menu state
    bool _gameStarted = false;
    Canvas _startMenuCanvas;
    Canvas _basicAttackChoosingCanvas;
    
    // Scene names
    const string CRYSTALLINE_PATH = "CrystalinePath";
    const string LOADING_SCREEN = "LoadingScreenBetweenLevels";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_instance != null) return;
        
        var go = new GameObject("[HUDManager]");
        _instance = go.AddComponent<HUDManager>();
        DontDestroyOnLoad(go);
    }

    public static HUDManager Instance => _instance;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Skip HUD setup for loading screen
        if (scene.name == LOADING_SCREEN)
        {
            if (_barsCanvas != null)
                _barsCanvas.gameObject.SetActive(false);
            return;
        }

        StartCoroutine(SetupSceneDelayed(scene));
    }

    /// <summary>
    /// Delayed setup to ensure all scene objects are initialized first.
    /// </summary>
    IEnumerator SetupSceneDelayed(Scene scene)
    {
        // Wait one frame so all Awake/Start calls in the scene have run
        yield return null;

        SetupBarsCanvas(scene);
        SetupHPBar();

        // Show start menu only for CrystalinePath and only if game hasn't started yet
        if (scene.name == CRYSTALLINE_PATH && !_gameStarted)
        {
            SetupStartMenu(scene);
        }
        else
        {
            // Game already started (returning from loading screen or already chose attack)
            // Make sure BarsCanvas is visible and time is running
            if (_barsCanvas != null)
            {
                _barsCanvas.gameObject.SetActive(true);
                _barsCanvas.enabled = true;
            }
        }
    }

    /// <summary>
    /// Finds or takes ownership of BarsCanvas. On first scene, DontDestroyOnLoad it.
    /// On subsequent scenes, destroy the scene's duplicate and keep the persistent one.
    /// </summary>
    void SetupBarsCanvas(Scene scene)
    {
        if (_barsCanvas != null)
        {
            // We already have a persistent BarsCanvas — destroy any duplicates from the new scene
            _barsCanvas.gameObject.SetActive(true);
            
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                if (rootObj.name == "BarsCanvas" && rootObj != _barsCanvas.gameObject)
                {
                    Debug.Log($"[HUDManager] Destroying duplicate BarsCanvas from scene '{scene.name}'");
                    Destroy(rootObj);
                }
            }
            return;
        }

        // First time — find the scene's BarsCanvas and take ownership
        foreach (var rootObj in scene.GetRootGameObjects())
        {
            if (rootObj.name == "BarsCanvas")
            {
                _barsCanvas = rootObj.GetComponent<Canvas>();
                if (_barsCanvas != null)
                {
                    DontDestroyOnLoad(_barsCanvas.gameObject);
                    Debug.Log($"[HUDManager] Took ownership of BarsCanvas from scene '{scene.name}'");
                    return;
                }
            }
        }

        // BarsCanvas not found — create one from scratch
        Debug.LogWarning("[HUDManager] No BarsCanvas found in scene, creating one.");
        CreateBarsCanvas();
    }

    /// <summary>
    /// Creates a minimal BarsCanvas if none exists in the scene.
    /// </summary>
    void CreateBarsCanvas()
    {
        var canvasGO = new GameObject("BarsCanvas");
        canvasGO.layer = 5; // UI layer

        _barsCanvas = canvasGO.AddComponent<Canvas>();
        _barsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _barsCanvas.sortingOrder = 0;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        DontDestroyOnLoad(canvasGO);
    }

    /// <summary>
    /// Adds HPBar to the BarsCanvas if it doesn't already have one.
    /// HPBar is a nested canvas with HorizontalLayoutGroup that spawns heart icons.
    /// </summary>
    void SetupHPBar()
    {
        if (_barsCanvas == null) return;

        // Check if HPBar already exists
        _hpBarController = _barsCanvas.GetComponentInChildren<HPBarController>();
        if (_hpBarController != null) return;

        // Load Heart prefab from Resources
        GameObject heartPrefab = Resources.Load<GameObject>("UI/Heart");
        if (heartPrefab == null)
        {
            Debug.LogError("[HUDManager] Could not load Heart prefab from Resources/UI/Heart!");
            return;
        }

        // Create HPBar GameObject
        var hpBarGO = new GameObject("HPBar");
        hpBarGO.layer = 5; // UI layer
        hpBarGO.tag = "HPBar";

        // RectTransform — anchor top-left, offset right and down
        var rect = hpBarGO.AddComponent<RectTransform>();
        rect.SetParent(_barsCanvas.transform, false);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(100, -50);
        rect.sizeDelta = new Vector2(0, 107f);
        rect.pivot = new Vector2(0, 1);

        // HorizontalLayoutGroup — spaces hearts evenly
        var layoutGroup = hpBarGO.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 40;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;

        // ContentSizeFitter — auto-width based on number of hearts
        var sizeFitter = hpBarGO.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        // HPBarController — manages heart drain/restore
        _hpBarController = hpBarGO.AddComponent<HPBarController>();
        // Use reflection or serialized field helper to set the heart prefab
        SetPrivateField(_hpBarController, "_heartPrefab", heartPrefab);
        SetPrivateField(_hpBarController, "_startingHearts", 7);

        Debug.Log("[HUDManager] HPBar created and added to BarsCanvas.");

        // Rebind player HUD controllers since HPBar was created after Player.Start()
        if (GameManager.Player != null)
        {
            GameManager.Player.RebindHUDControllers();
        }
    }

    /// <summary>
    /// Creates the StartMenuCanvas with a Begin button.
    /// Pauses the game until the player clicks Begin, then shows attack selection.
    /// </summary>
    void SetupStartMenu(Scene scene)
    {
        // Pause the game
        Time.timeScale = 0f;

        // Hide BarsCanvas until attack is chosen
        if (_barsCanvas != null)
        {
            _barsCanvas.gameObject.SetActive(true);
            _barsCanvas.enabled = false;
        }

        // Find the BasicAttackChoosingCanvas in the scene (already exists in CrystalinePath)
        _basicAttackChoosingCanvas = null;
        foreach (var rootObj in scene.GetRootGameObjects())
        {
            if (rootObj.name == "BasicAttackChoosingCanvas")
            {
                _basicAttackChoosingCanvas = rootObj.GetComponent<Canvas>();
                break;
            }
        }
        // Also check DontDestroyOnLoad objects
        if (_basicAttackChoosingCanvas == null)
        {
            var found = GameObject.Find("BasicAttackChoosingCanvas");
            if (found != null)
                _basicAttackChoosingCanvas = found.GetComponent<Canvas>();
        }

        if (_basicAttackChoosingCanvas != null)
        {
            _basicAttackChoosingCanvas.gameObject.SetActive(false);
        }

        // Load begin button sprites from Resources
        Sprite idleSprite = LoadSpriteFromResources("UI/ButtonBeginIdle");
        Sprite hoverSprite = LoadSpriteFromResources("UI/ButtonBeginHover");

        // Create StartMenuCanvas
        var startMenuGO = new GameObject("StartMenuCanvas");
        startMenuGO.layer = 5;

        _startMenuCanvas = startMenuGO.AddComponent<Canvas>();
        _startMenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _startMenuCanvas.sortingOrder = 10; // Render above everything

        var scaler = startMenuGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.referencePixelsPerUnit = 100;

        startMenuGO.AddComponent<GraphicRaycaster>();

        // Create BeginButton
        var buttonGO = new GameObject("BeginButton");
        buttonGO.layer = 5;

        var buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.SetParent(_startMenuCanvas.transform, false);
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(94, 27);
        buttonRect.localScale = new Vector3(5, 5, 1);

        // Image component for the button sprite
        var buttonImage = buttonGO.AddComponent<Image>();
        if (idleSprite != null)
            buttonImage.sprite = idleSprite;
        buttonImage.raycastTarget = true;

        // StartMenuController component
        var menuController = buttonGO.AddComponent<StartMenuController>();
        SetPrivateField(menuController, "_targetImage", buttonImage);
        SetPrivateField(menuController, "_unhooveredSprite", idleSprite);
        SetPrivateField(menuController, "_hooveredSprite", hoverSprite);
        SetPrivateField(menuController, "_startMenuCanvas", _startMenuCanvas);
        SetPrivateField(menuController, "_basicAttackChoosingCanvas", _basicAttackChoosingCanvas);
        SetPrivateField(menuController, "_barsCanvas", _barsCanvas);

        // EventTrigger for hover/click events
        var eventTrigger = buttonGO.AddComponent<EventTrigger>();

        // PointerEnter → Hoovered
        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener(_ => menuController.Hoovered());
        eventTrigger.triggers.Add(enterEntry);

        // PointerExit → Unhoovered
        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener(_ => menuController.Unhoovered());
        eventTrigger.triggers.Add(exitEntry);

        // PointerClick → Selected
        var clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        clickEntry.callback.AddListener(_ => menuController.Selected());
        eventTrigger.triggers.Add(clickEntry);

        // Ensure EventSystem exists
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }

        Debug.Log("[HUDManager] StartMenu created for CrystalinePath.");
    }

    /// <summary>
    /// Called by the skill choose controllers when an attack is selected.
    /// Marks the game as started so the start menu won't show again.
    /// </summary>
    public void OnAttackSelected()
    {
        _gameStarted = true;

        if (_startMenuCanvas != null)
        {
            Destroy(_startMenuCanvas.gameObject);
            _startMenuCanvas = null;
        }

        if (_barsCanvas != null)
        {
            _barsCanvas.enabled = true;
        }
    }

    /// <summary>
    /// Loads a sprite from Resources. Handles both single sprites and sprite sheets.
    /// </summary>
    Sprite LoadSpriteFromResources(string path)
    {
        // Try loading as a Sprite directly
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null) return sprite;

        // Try loading as Texture2D and getting all sprites from it
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites != null && sprites.Length > 0)
            return sprites[0];

        Debug.LogWarning($"[HUDManager] Could not load sprite from Resources/{path}");
        return null;
    }

    /// <summary>
    /// Helper to set private serialized fields via reflection.
    /// Used to programmatically assign values that would normally be set in the Inspector.
    /// </summary>
    static void SetPrivateField(object target, string fieldName, object value)
    {
        var type = target.GetType();
        var field = type.GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"[HUDManager] Could not find field '{fieldName}' on {type.Name}");
        }
    }
}
