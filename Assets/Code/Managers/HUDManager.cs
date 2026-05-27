using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.LowLevel;

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
    const string LEBOLIA_MORASS = "LeboliaMorass";
    const string LOADING_SCREEN = "LoadingScreenBetweenLevels";

    // Audio elements
    AudioSource _soundtrackSource;
    AudioClip _spaceSoundtrack;
    AudioClip _ruskerdaxSoundtrack;

    // Tutorial elements
    GameObject _tutorialCanvasObj;
    GameObject _textAttackSelect;
    GameObject _textXp;
    GameObject _textJoystick;
    GameObject _textTime;
    GameObject _textPressSpace;
    GameObject _xpBar;
    GameObject _timeTextObj;

    public enum TutorialStep
    {
        StartMenu,
        AttackSelect,
        XpBar,
        Joystick,
        TimeDisplay,
        Finished
    }
    TutorialStep _currentTutorialStep = TutorialStep.StartMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_instance != null) return;
        
        var go = new GameObject("[HUDManager]");
        _instance = go.AddComponent<HUDManager>();
        DontDestroyOnLoad(go);
    }

    public static HUDManager Instance => _instance;

    public Canvas BarsCanvas => _barsCanvas;

    void Awake()
    {
        _soundtrackSource = GetComponent<AudioSource>();
        if (_soundtrackSource == null)
        {
            _soundtrackSource = gameObject.AddComponent<AudioSource>();
        }
        _soundtrackSource.loop = true;
        _soundtrackSource.volume = 0.5f;
        _soundtrackSource.playOnAwake = false;
    }

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
            
            if (_soundtrackSource != null && _soundtrackSource.isPlaying)
            {
                _soundtrackSource.Stop();
                Debug.Log("[HUDManager] Stopped soundtrack for loading screen.");
            }
            return;
        }

        // Immediately deactivate player on start screen to avoid 1-frame visual flash
        if (scene.name == CRYSTALLINE_PATH && !_gameStarted)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerObj.SetActive(false);
                Debug.Log("[HUDManager] Immediately deactivated player on scene loaded.");
            }
        }

        StartCoroutine(SetupSceneDelayed(scene));
    }

    IEnumerator SetupSceneDelayed(Scene scene)
    {
        // Wait one frame so all Awake/Start calls in the scene have run
        yield return null;

        SetupBarsCanvas(scene);
        SetupHPBar();

        // Cache the HPBar and TimeInGame references while they are active
        if (_barsCanvas != null)
        {
            var timeInGameTrans = _barsCanvas.transform.Find("TimeInGame");
            if (timeInGameTrans != null)
            {
                _timeTextObj = timeInGameTrans.gameObject;
            }
        }
        if (_timeTextObj == null)
        {
            _timeTextObj = GameObject.FindGameObjectWithTag("TimeText");
        }

        ApplyPixelifyFontToScene();

        // Show start menu only for CrystalinePath and only if game hasn't started yet
        if (scene.name == CRYSTALLINE_PATH && !_gameStarted)
        {
            FindTutorialObjects();
            SetTutorialStep(TutorialStep.StartMenu);
            SetupStartMenu(scene);
        }
        else
        {
            _currentTutorialStep = TutorialStep.Finished;
            var tutCanvas = GameObject.Find("TutorialCanvas");
            if (tutCanvas != null) tutCanvas.SetActive(false);

            // Game already started (returning from loading screen or already chose attack)
            // Make sure BarsCanvas is visible and time is running
            if (_barsCanvas != null)
            {
                _barsCanvas.gameObject.SetActive(true);
                _barsCanvas.enabled = true;
            }
        }

        // Apply initial visibility for HP bar and timer
        UpdateHeartsAndTimerVisibility();

        // Always rebind the player's HUD controllers to the persistent canvas after setup
        if (GameManager.Player != null)
        {
            GameManager.Player.RebindHUDControllers();
            Debug.Log("[HUDManager] Successfully rebound HUD controllers to player.");
        }

        PlaySceneSoundtrack(scene.name);
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

        // Load begin button and ranking button sprites from Resources
        Sprite idleSprite = LoadSpriteFromResources("UI/ButtonBeginIdle");
        Sprite hoverSprite = LoadSpriteFromResources("UI/ButtonBeginHover");
        Sprite rankingIdleSprite = LoadSpriteFromResources("UI/ButtonRankingIdle");
        Sprite rankingHoverSprite = LoadSpriteFromResources("UI/ButtonRankingHover");

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

        // Create Background Panel (similar to BasicAttackChoosingCanvas Panel)
        var panelGO = new GameObject("Panel");
        panelGO.layer = 5;
        var panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.SetParent(_startMenuCanvas.transform, false);
        
        // Stretch to fill screen
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;
        
        panelGO.AddComponent<CanvasRenderer>();
        var panelImage = panelGO.AddComponent<Image>();
        
        Sprite bgSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
        Color panelColor = new Color(0f, 0f, 0f, 0.6117647f);
        Image.Type panelType = Image.Type.Sliced;
        
        if (_basicAttackChoosingCanvas != null)
        {
            var existingPanel = _basicAttackChoosingCanvas.transform.Find("Panel");
            if (existingPanel != null)
            {
                var existingImage = existingPanel.GetComponent<Image>();
                if (existingImage != null)
                {
                    bgSprite = existingImage.sprite;
                    panelColor = existingImage.color;
                    panelType = existingImage.type;
                }
            }
        }
        
        panelImage.sprite = bgSprite;
        panelImage.color = panelColor;
        panelImage.type = panelType;

        // Create BeginButton
        var buttonGO = new GameObject("BeginButton");
        buttonGO.layer = 5;

        var buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.SetParent(_startMenuCanvas.transform, false);
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, 80f);
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

        // Create RankingButton
        var rankingGO = new GameObject("RankingButton");
        rankingGO.layer = 5;

        var rankingRect = rankingGO.AddComponent<RectTransform>();
        rankingRect.SetParent(_startMenuCanvas.transform, false);
        rankingRect.anchorMin = new Vector2(0.5f, 0.5f);
        rankingRect.anchorMax = new Vector2(0.5f, 0.5f);
        rankingRect.anchoredPosition = new Vector2(0f, -80f);
        rankingRect.sizeDelta = new Vector2(94, 27);
        rankingRect.localScale = new Vector3(5, 5, 1);

        // Image component for the button sprite
        var rankingImage = rankingGO.AddComponent<Image>();
        if (rankingIdleSprite != null)
            rankingImage.sprite = rankingIdleSprite;
        rankingImage.raycastTarget = true;

        // RankingButtonController component
        var rankingController = rankingGO.AddComponent<RankingButtonController>();
        rankingController.Init(rankingImage, rankingIdleSprite, rankingHoverSprite, _startMenuCanvas);

        // EventTrigger for hover/click events
        var rankingTrigger = rankingGO.AddComponent<EventTrigger>();

        // PointerEnter → Hoovered
        var rEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        rEnter.callback.AddListener(_ => rankingController.Hoovered());
        rankingTrigger.triggers.Add(rEnter);

        // PointerExit → Unhoovered
        var rExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        rExit.callback.AddListener(_ => rankingController.Unhoovered());
        rankingTrigger.triggers.Add(rExit);

        // PointerClick → Selected
        var rClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        rClick.callback.AddListener(_ => rankingController.Selected());
        rankingTrigger.triggers.Add(rClick);

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
            _barsCanvas.gameObject.SetActive(true);
        }

        // Activate the Safina character (Player) now that the attack is selected!
        var playerCtrl = FindFirstObjectByType<PlayerCtrl>(FindObjectsInactive.Include);
        if (playerCtrl != null)
        {
            playerCtrl.gameObject.SetActive(true);
            Debug.Log("[HUDManager] Spawned (Activated) Safina after attack select.");
        }

        // Rebind the player in GameManager now that she is active
        GameManager.FindActivePlayer();

        if (GameManager.Player != null)
        {
            GameManager.Player.RebindHUDControllers();
            Debug.Log("[HUDManager] Successfully rebound HUD controllers to player after attack select.");
        }

        // Intercept skill selection controllers' deactivation and override it to proceed with the tutorial
        if (_tutorialCanvasObj != null)
        {
            _tutorialCanvasObj.SetActive(true);
        }
        
        // Keep the game paused during tutorial instructions
        Time.timeScale = 0f;

        // Advance to XP Bar tutorial step
        SetTutorialStep(TutorialStep.XpBar);
    }

    Sprite MakeWhiteTransparent(Sprite original)
    {
        if (original == null) return null;
        Texture2D tex = original.texture;
        if (tex == null) return original;

        try
        {
            Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            newTex.filterMode = FilterMode.Point;

            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].r > 0.98f && pixels[i].g > 0.98f && pixels[i].b > 0.98f)
                {
                    pixels[i] = new Color(0f, 0f, 0f, 0f);
                }
            }

            newTex.SetPixels(pixels);
            newTex.Apply();

            Rect rect = original.rect;
            Vector2 pivot = new Vector2(original.pivot.x / original.rect.width, original.pivot.y / original.rect.height);
            Sprite newSprite = Sprite.Create(newTex, rect, pivot, original.pixelsPerUnit);
            return newSprite;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[HUDManager] Error making white transparent: {ex.Message}");
            return original;
        }
    }

    /// <summary>
    /// Loads a sprite from Resources. Handles both single sprites and sprite sheets.
    /// </summary>
    Sprite LoadSpriteFromResources(string path)
    {
        Sprite sprite = null;

        // Try loading as a Sprite directly
        Sprite loadedSprite = Resources.Load<Sprite>(path);
        if (loadedSprite != null)
        {
            sprite = loadedSprite;
        }
        else
        {
            // Try loading as Texture2D and getting all sprites from it
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            if (sprites != null && sprites.Length > 0)
            {
                sprite = sprites[0];
            }
        }

        if (sprite != null)
        {
            // Set point filtering for crisp pixel art
            if (sprite.texture != null)
            {
                sprite.texture.filterMode = FilterMode.Point;
            }

            // Apply transparency to ranking buttons
            if (path.Contains("ButtonRanking"))
            {
                sprite = MakeWhiteTransparent(sprite);
            }
            return sprite;
        }

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

    TMP_FontAsset _pixelifyFontAsset;

    void LoadPixelifyFont()
    {
        if (_pixelifyFontAsset != null) return;

        Font rawFont = Resources.Load<Font>("PixelifySans");
        if (rawFont == null)
        {
            Debug.LogError("[HUDManager] Failed to load raw PixelifySans font from Resources!");
            return;
        }

        _pixelifyFontAsset = TMP_FontAsset.CreateFontAsset(
            rawFont, 
            90,              // Sampling point size
            9,               // Atlas padding
            GlyphRenderMode.SDFAA, 
            1024,            // Atlas width
            1024,            // Atlas height
            AtlasPopulationMode.Dynamic
        );
        _pixelifyFontAsset.name = "PixelifySans_Asset";
        Debug.Log("[HUDManager] Successfully created dynamic TMP_FontAsset for PixelifySans at runtime.");
    }

    void ApplyPixelifyFontToScene()
    {
        if (_pixelifyFontAsset == null)
        {
            LoadPixelifyFont();
        }
        if (_pixelifyFontAsset == null) return;

        // 1. Apply to TutorialCanvas texts
        var tutorialCanvasObj = GameObject.Find("TutorialCanvas");
        if (tutorialCanvasObj != null)
        {
            var texts = tutorialCanvasObj.GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in texts)
            {
                t.font = _pixelifyFontAsset;
            }
            Debug.Log($"[HUDManager] Applied Pixelify Sans to {texts.Length} text components under TutorialCanvas.");
        }

        // 2. Apply to TimeInGame text under BarsCanvas
        if (_barsCanvas != null)
        {
            var timeInGameTrans = _barsCanvas.transform.Find("TimeInGame");
            if (timeInGameTrans != null)
            {
                var timeText = timeInGameTrans.GetComponent<TMP_Text>();
                if (timeText != null)
                {
                    timeText.font = _pixelifyFontAsset;
                    Debug.Log("[HUDManager] Applied Pixelify Sans to TimeInGame text component.");
                }
            }
            else
            {
                var timeInGameObj = GameObject.Find("TimeInGame");
                if (timeInGameObj != null)
                {
                    var timeText = timeInGameObj.GetComponent<TMP_Text>();
                    if (timeText != null)
                    {
                        timeText.font = _pixelifyFontAsset;
                        Debug.Log("[HUDManager] Applied Pixelify Sans to TimeInGame text component (fallback).");
                    }
                }
            }
        }
    }

    void PlaySceneSoundtrack(string sceneName)
    {
        // Diagnostics
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        Debug.Log($"[HUDManager] Audio Diagnostics - Active Scene: {sceneName}");
        Debug.Log($"[HUDManager] Audio Diagnostics - AudioListener count: {listeners.Length}");
        if (listeners.Length > 0)
        {
            foreach (var listener in listeners)
            {
                Debug.Log($"[HUDManager] Audio Diagnostics - AudioListener GameObject: {listener.gameObject.name}, Enabled: {listener.enabled}, GameObject Active: {listener.gameObject.activeInHierarchy}");
            }
        }
        Debug.Log($"[HUDManager] Audio Diagnostics - AudioListener.volume: {AudioListener.volume}, AudioListener.pause: {AudioListener.pause}");
        Debug.Log($"[HUDManager] Audio Diagnostics - Camera.main: {(Camera.main != null ? Camera.main.gameObject.name : "NULL")}");

        AudioClip targetClip = null;

        if (sceneName == CRYSTALLINE_PATH)
        {
            if (_spaceSoundtrack == null)
            {
                _spaceSoundtrack = Resources.Load<AudioClip>("Audio/Soundtracks/space");
                Debug.Log($"[HUDManager] Loading space soundtrack: {_spaceSoundtrack != null}");
            }
            targetClip = _spaceSoundtrack;
        }
        else if (sceneName == LEBOLIA_MORASS)
        {
            if (_ruskerdaxSoundtrack == null)
            {
                _ruskerdaxSoundtrack = Resources.Load<AudioClip>("Audio/Soundtracks/Ruskerdax - Pondering the Cosmos");
                Debug.Log($"[HUDManager] Loading ruskerdax soundtrack: {_ruskerdaxSoundtrack != null}");
            }
            targetClip = _ruskerdaxSoundtrack;
        }

        if (_soundtrackSource != null)
        {
            if (targetClip != null)
            {
                if (_soundtrackSource.clip != targetClip || !_soundtrackSource.isPlaying)
                {
                    _soundtrackSource.clip = targetClip;
                    _soundtrackSource.Play();
                    Debug.Log($"[HUDManager] Playing soundtrack: {targetClip.name}, isPlaying: {_soundtrackSource.isPlaying}, volume: {_soundtrackSource.volume}, mute: {_soundtrackSource.mute}");
                }
            }
            else
            {
                if (_soundtrackSource.isPlaying)
                {
                    _soundtrackSource.Stop();
                    Debug.Log("[HUDManager] No soundtrack for this scene, stopping audio.");
                }
            }
        }
        else
        {
            Debug.LogError("[HUDManager] _soundtrackSource is NULL!");
        }
    }

    void FindTutorialObjects()
    {
        _tutorialCanvasObj = GameObject.Find("TutorialCanvas");
        if (_tutorialCanvasObj != null)
        {
            foreach (Transform child in _tutorialCanvasObj.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "TextAttackSelect") _textAttackSelect = child.gameObject;
                else if (child.name == "TextXp") _textXp = child.gameObject;
                else if (child.name == "TextJoystick") _textJoystick = child.gameObject;
                else if (child.name == "TextTime") _textTime = child.gameObject;
                else if (child.name == "TextPressSpace") _textPressSpace = child.gameObject;
            }
        }

        // Find XPBar in the scene
        _xpBar = GameObject.Find("XPBar");
        if (_xpBar == null)
        {
            var xpBarObj = GameObject.FindGameObjectWithTag("XPBar");
            if (xpBarObj != null) _xpBar = xpBarObj;
        }
    }

    public void TransitionToStep(TutorialStep step)
    {
        SetTutorialStep(step);
    }

    public bool IsTutorialActive()
    {
        return _currentTutorialStep == TutorialStep.StartMenu ||
               _currentTutorialStep == TutorialStep.AttackSelect ||
               _currentTutorialStep == TutorialStep.XpBar ||
               _currentTutorialStep == TutorialStep.Joystick;
    }

    void UpdateHeartsAndTimerVisibility()
    {
        bool showHeartsAndTimer = (_currentTutorialStep == TutorialStep.TimeDisplay || _currentTutorialStep == TutorialStep.Finished);

        if (_hpBarController != null)
        {
            _hpBarController.gameObject.SetActive(showHeartsAndTimer);
        }
        else
        {
            var hpBarObj = GameObject.FindGameObjectWithTag("HPBar");
            if (hpBarObj != null)
            {
                _hpBarController = hpBarObj.GetComponent<HPBarController>();
                if (_hpBarController != null)
                {
                    _hpBarController.gameObject.SetActive(showHeartsAndTimer);
                }
            }
        }

        if (_timeTextObj != null)
        {
            _timeTextObj.SetActive(showHeartsAndTimer);
        }
        else
        {
            var timeTextObj = GameObject.FindGameObjectWithTag("TimeText");
            if (timeTextObj != null)
            {
                _timeTextObj = timeTextObj;
                _timeTextObj.SetActive(showHeartsAndTimer);
            }
            else if (_barsCanvas != null)
            {
                var timeInGameTrans = _barsCanvas.transform.Find("TimeInGame");
                if (timeInGameTrans != null)
                {
                    _timeTextObj = timeInGameTrans.gameObject;
                    _timeTextObj.SetActive(showHeartsAndTimer);
                }
            }
        }
    }

    void SetTutorialStep(TutorialStep step)
    {
        _currentTutorialStep = step;

        // Apply visibility rules for HP bar and timer
        UpdateHeartsAndTimerVisibility();

        // If not finished, make sure TutorialCanvas is active
        if (step != TutorialStep.Finished && _tutorialCanvasObj != null)
        {
            _tutorialCanvasObj.SetActive(true);
        }

        // Hide all texts by default
        if (_textAttackSelect != null) _textAttackSelect.SetActive(false);
        if (_textXp != null) _textXp.SetActive(false);
        if (_textJoystick != null) _textJoystick.SetActive(false);
        if (_textTime != null) _textTime.SetActive(false);
        if (_textPressSpace != null) _textPressSpace.SetActive(false);

        switch (step)
        {
            case TutorialStep.StartMenu:
                if (_xpBar != null) _xpBar.SetActive(false);
                break;

            case TutorialStep.AttackSelect:
                if (_textAttackSelect != null) _textAttackSelect.SetActive(true);
                if (_xpBar != null) _xpBar.SetActive(false);
                break;

            case TutorialStep.XpBar:
                if (_xpBar != null) _xpBar.SetActive(true);
                if (_textXp != null) _textXp.SetActive(true);
                if (_textPressSpace != null)
                {
                    _textPressSpace.SetActive(true);
                    var txt = _textPressSpace.GetComponent<TMP_Text>();
                    if (txt != null) txt.text = "<color=#FFD700>[ Press any key or click/tap to continue ]</color>";
                }
                Time.timeScale = 0f; // Pause the game
                break;

            case TutorialStep.Joystick:
                if (_xpBar != null) _xpBar.SetActive(true);
                if (_textJoystick != null) _textJoystick.SetActive(true);
                if (_textPressSpace != null)
                {
                    _textPressSpace.SetActive(true);
                    var txt = _textPressSpace.GetComponent<TMP_Text>();
                    if (txt != null) txt.text = "<color=#FFD700>[ Press any key or click/tap to continue ]</color>";
                }
                Time.timeScale = 0f; // Pause the game
                break;

            case TutorialStep.TimeDisplay:
                if (_xpBar != null) _xpBar.SetActive(true);
                if (_textTime != null) _textTime.SetActive(true);
                Time.timeScale = 1f; // Start gameplay!
                StartCoroutine(AutoCloseTutorialAfterSeconds(4f));
                break;

            case TutorialStep.Finished:
                if (_xpBar != null) _xpBar.SetActive(true);
                if (_tutorialCanvasObj != null) _tutorialCanvasObj.SetActive(false);
                break;
        }
    }

    IEnumerator AutoCloseTutorialAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (_currentTutorialStep == TutorialStep.TimeDisplay)
        {
            SetTutorialStep(TutorialStep.Finished);
        }
    }

    void Update()
    {
        if (_currentTutorialStep == TutorialStep.XpBar || _currentTutorialStep == TutorialStep.Joystick)
        {
            bool anythingPressed = false;
#if ENABLE_INPUT_SYSTEM
            // Check keyboard
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
            {
                anythingPressed = true;
            }
            // Check mouse/touch (Pointer handles both)
            if (UnityEngine.InputSystem.Pointer.current != null && UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame)
            {
                anythingPressed = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                anythingPressed = true;
            }
#else
            if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                anythingPressed = true;
            }
#endif

            if (anythingPressed)
            {
                if (_currentTutorialStep == TutorialStep.XpBar)
                {
                    SetTutorialStep(TutorialStep.Joystick);
                }
                else if (_currentTutorialStep == TutorialStep.Joystick)
                {
                    SetTutorialStep(TutorialStep.TimeDisplay);
                }
            }
        }
    }
}
