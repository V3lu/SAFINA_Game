using Assets.Code.Interfaces;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Code.Utils;

public class PlayerCtrl : MonoBehaviour, IDamagable
{
    [SerializeField] CrystalinePathSO _crystalinePathSO;
    [SerializeField] XPSO _XPSO;
    [SerializeField] GameObject _fireballPrefab;
    [SerializeField] GameObject _energyBlastPrefab;
    [SerializeField] GameObject _voidBoltPrefab;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _movSpeed;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Damage Settings")]
    [SerializeField] float _invincibilityDuration = 1.0f;
    private float _invincibilityTimer = 0f;
    private SpriteRenderer _spriteRenderer;
    private float SortingPrecision = 10f;
    private int SortingBase = 1000;
    private Vector3 moveInput;
    private float _dashDistance = 2f;
    private bool _isDead = false;
    private bool _deathTriggered = false;


    static float _attackProjectileSpawnTimer;
    static Rigidbody2D _rb;
    static Animator _animator;
    static long _playerXPTotal = 0;
    static long _playerXPCurrent = 0;
    static int _playerLvl = 0;
    static XPBarController _xpBarController;
    static HPBarController _hpBarController;
    static AudioClip _fireballSFX;
    static AudioClip[] _energyBlastSFXs;    
    private int _maxDashCharges = 2;
    private int _currentDashCharges = 2;
    private float _dashCooldownDuration = 2f;
    private List<float> _dashCooldowns = new List<float>();
    static AudioClip[] _voidBoltSFXs;
    static AudioClip _playerDamageSFX;
    static AudioClip _coinSFX;

    public static ChosenBasicAttact AttackType = ChosenBasicAttact.NotChosen;

    public float HP { get; set; }

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = SortingBase +
            Mathf.RoundToInt(-transform.position.y * SortingPrecision);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformDash();
        }
    }

    private void PerformDash()
    {
        if (_isDead) return;
        if (_currentDashCharges <= 0) return;

        // Zużywamy 1 ładunek dasha i dodajemy mu czas odnowienia
        _currentDashCharges--;
        _dashCooldowns.Add(_dashCooldownDuration);

        Vector3 mousePosition = Vector3.zero;
        Debug.Log("JUMPING TO MOUSE POSITION: " + mousePosition);
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            mousePosition = Mouse.current.position.ReadValue();
        }
#else
        mousePosition = Input.mousePosition;
#endif

        if (Camera.main != null)
        {
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = transform.position.z;

            Vector3 dashDirection = (worldMousePosition - transform.position).normalized;
            transform.position += dashDirection * _dashDistance;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public enum ChosenBasicAttact
    {
        Void = 0,
        Energy = 1,
        Fire = 2,
        NotChosen = 3
    }
    public enum AnimState
    {
        WalkRight = 0,
        WalkLeft = 1,
        EnergyRight = 2,
        EnergyLeft = 3,
        FireRight = 4,
        FireLeft = 5,
        VoidRight = 6,
        VoidLeft = 7
    }
    public enum Directions
    {
        Left = 0,
        Right = 1
    }


    public void LooseHP(float hp)
    {
        if (_isDead) return;

        this.HP -= hp;
        Debug.Log($"{this.HP} HP remaining");
        if (_hpBarController != null)
        {
            _hpBarController.DrainHearts(Mathf.RoundToInt(hp));
        }

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRedOnDamage());
            StartCoroutine(HitStop(0.08f)); // Creates a massive tactile impact!
            PlaySFX(_playerDamageSFX, 1f, 0f, 0); // Priority 0 so it never gets culled
        }

        if (this.HP <= 0)
        {
            this.HP = 0;
            _isDead = true;
        }
    }

    private IEnumerator FlashRedOnDamage()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator HitStop(float duration)
    {
        // "Hit Stop" is a classic 2D indie technique. Freezing the game for a split second gives massive weight to taking damage.
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.05f; 
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }

    public void RestoreHP(float hp)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        var xpBarObj = GameObject.FindGameObjectWithTag("XPBar");
        if (xpBarObj != null)
            _xpBarController = xpBarObj.GetComponent<XPBarController>();
        else
            Debug.LogWarning("[PlayerCtrl] XPBar not found. It may be created later by HUDManager.");

        var hpBarObj = GameObject.FindGameObjectWithTag("HPBar");
        if (hpBarObj != null)
        {
            _hpBarController = hpBarObj.GetComponent<HPBarController>();
        }
        else
        {
            Debug.LogWarning("[PlayerCtrl] HPBar not found. It may be created later by HUDManager.");
        }
        this.HP = 7;
        if (_hpBarController != null)
        {
            _hpBarController.RestoreAllHearts();
        }

        // Load sound effects if they haven't been loaded yet
        if (_fireballSFX == null)
        {
            _fireballSFX = Resources.Load<AudioClip>("Audio/Fireball/fireball");
            Debug.Log($"[PlayerCtrl] Fireball SFX loaded: {_fireballSFX != null}");
        }
        if (_energyBlastSFXs == null || _energyBlastSFXs.Length == 0)
        {
            _energyBlastSFXs = Resources.LoadAll<AudioClip>("Audio/EnergyBlast");
            Debug.Log($"[PlayerCtrl] EnergyBlast SFXs loaded count: {(_energyBlastSFXs != null ? _energyBlastSFXs.Length : 0)}");
        }
        if (_voidBoltSFXs == null || _voidBoltSFXs.Length == 0)
        {
            _voidBoltSFXs = Resources.LoadAll<AudioClip>("Audio/VoidBolt");
            Debug.Log($"[PlayerCtrl] VoidBolt SFXs loaded count: {(_voidBoltSFXs != null ? _voidBoltSFXs.Length : 0)}");
        }
        if (_playerDamageSFX == null)
        {
            _playerDamageSFX = Resources.Load<AudioClip>("Audio/sfx_damage_player");
            Debug.Log($"[PlayerCtrl] Player damage SFX loaded: {_playerDamageSFX != null}");
        }
        if (_coinSFX == null)
        {
            _coinSFX = Resources.Load<AudioClip>("Audio/sfx_coin_single2");
            Debug.Log($"[PlayerCtrl] Coin SFX loaded: {_coinSFX != null}");
        }
    }

    private void PlaySFX(AudioClip clip, float volume = 1f, float pitchRandomness = 0f, int priority = 128)
    {
        if (clip != null)
        {
            // Use camera's Z coordinate to ensure the sound is close to the AudioListener and audible in 2D
            Vector3 playPos = transform.position;
            if (Camera.main != null)
            {
                playPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            }
            else
            {
                Debug.LogWarning("[PlayerCtrl] Camera.main not found when playing SFX. Playing at player position.");
            }
            AudioUtil.PlaySFX(clip, playPos, volume, pitchRandomness, priority);
        }
        else
        {
            Debug.LogWarning("[PlayerCtrl] PlaySFX called with null clip!");
        }
    }

    private void PlayRandomSFX(AudioClip[] clips, float volume = 1f, float pitchRandomness = 0f, int priority = 128)
    {
        if (clips != null && clips.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, clips.Length);
            PlaySFX(clips[index], volume, pitchRandomness, priority);
        }
    }

    public static void PlayPersistentSFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioUtil.PlayPersistentSFX(clip, position, volume);
    }

    // Update is called once per frame
    void Update()
    {
        // Obsługa niezależnych czasów odnowienia skoku
        for (int i = _dashCooldowns.Count - 1; i >= 0; i--)
        {
            _dashCooldowns[i] -= Time.deltaTime;
            if (_dashCooldowns[i] <= 0)
            {
                _dashCooldowns.RemoveAt(i);
                if (_currentDashCharges < _maxDashCharges)
                {
                    _currentDashCharges++;
                }
            }
        }
        // Check for death — trigger death sequence once
        if (_isDead && !_deathTriggered)
        {
            _deathTriggered = true;
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.TriggerDeathSequence(TimeManager._time);
            }
            return;
        }
        if (_isDead) return;

        // Development cheat: Pressing ']' adds 1 XP
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.rightBracketKey.wasPressedThisFrame)
        {
            GainXP(1);
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            GainXP(1);
        }
#endif

        // Development cheat: Pressing '[' jumps to the next map
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            JumpToNextMap();
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            JumpToNextMap();
        }
#endif

        if (_invincibilityTimer > 0)
        {
            _invincibilityTimer -= Time.deltaTime;
        }

        Vector3 move = new Vector3(moveInput.x, moveInput.y);
        _rb.linearVelocity = move * _movSpeed;

        Dictionary<GameObject, GameObject> closestEnemyPlusOriginalPrefab = GetClosestEnemy();
        Directions directionToLookAt = Directions.Right;
        if (closestEnemyPlusOriginalPrefab != null)
        {
            directionToLookAt = DirectionToLookForTheClosestEnemy(closestEnemyPlusOriginalPrefab);
        }

        _attackProjectileSpawnTimer -= Time.deltaTime;

        if (closestEnemyPlusOriginalPrefab != null)
        {
            if (AttackType == ChosenBasicAttact.Fire)
            {
                if (_attackProjectileSpawnTimer <= 0)
                {
                    _attackProjectileSpawnTimer = _attackSpeed;
                    GameObject fireball = ObjectPoolManager.SpawnObject(_fireballPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
                    if (fireball.TryGetComponent<IProjectile>(out var projectile))
                    {
                        projectile.SetTarget(closestEnemyPlusOriginalPrefab.ElementAt(0).Key.transform.position);
                    }
                    // Pitch randomness of keeps it from sounding repetitive
                    PlaySFX(_fireballSFX, 0.2f, 0.3f);
                }
            }
            else if (AttackType == ChosenBasicAttact.Void)
            {
                if (_attackProjectileSpawnTimer <= 0)
                {
                    _attackProjectileSpawnTimer = _attackSpeed;
                    GameObject voidBolt = ObjectPoolManager.SpawnObject(_voidBoltPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
                    if (voidBolt.TryGetComponent<IProjectile>(out var projectile))
                    {
                        projectile.SetTarget(closestEnemyPlusOriginalPrefab.ElementAt(0).Key.transform.position);
                    }
                    PlayRandomSFX(_voidBoltSFXs, 0.4f, 0.1f);
                }
            }
            else if (AttackType == ChosenBasicAttact.Energy)
            {
                if (_attackProjectileSpawnTimer <= 0)
                {
                    _attackProjectileSpawnTimer = _attackSpeed;
                    GameObject energyBlast = ObjectPoolManager.SpawnObject(_energyBlastPrefab, closestEnemyPlusOriginalPrefab.ElementAt(0).Key.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
                    if (energyBlast.TryGetComponent<IProjectile>(out var projectile))
                    {
                        projectile.SetTarget(closestEnemyPlusOriginalPrefab.ElementAt(0).Key.transform.position);
                    }
                    PlayRandomSFX(_energyBlastSFXs, 0.7f, 0.1f);
                }
            }
        }



        if (directionToLookAt == Directions.Left)
        {
            switch (AttackType)
            {
                case ChosenBasicAttact.NotChosen:
                    SetAnimState(AnimState.WalkLeft); break;
                case ChosenBasicAttact.Void:
                    SetAnimState(AnimState.VoidLeft); break;
                case ChosenBasicAttact.Fire:
                    SetAnimState(AnimState.FireLeft); break;
                case ChosenBasicAttact.Energy:
                    SetAnimState(AnimState.EnergyLeft); break;
            }
        }
        else
        {
            switch (AttackType)
            {
                case ChosenBasicAttact.NotChosen:
                    SetAnimState(AnimState.WalkRight); break;
                case ChosenBasicAttact.Void:
                    SetAnimState(AnimState.VoidRight); break;
                case ChosenBasicAttact.Fire:
                    SetAnimState(AnimState.FireRight); break;
                case ChosenBasicAttact.Energy:
                    SetAnimState(AnimState.EnergyRight); break;
            }
        }

    }

    void SetAnimState(AnimState state)
    {
        _animator.SetInteger("State", (int)state);
    }
    Dictionary<GameObject, GameObject> GetClosestEnemy()              
    {
        Dictionary<GameObject, GameObject> activeEnemiesObjects = ObjectPoolManager.GetAllActiveGameObjectsOfThePool(ObjectPoolManager.PoolType.Mobs);
        Dictionary<GameObject, float> mobsDistances = new Dictionary<GameObject, float>();
        if (activeEnemiesObjects.Count() > 0)
        {
            foreach (var kvp in activeEnemiesObjects)
            {
                float distanceOfTheCurrentMobToPlayer = Vector3.Distance(gameObject.transform.position, kvp.Key.transform.position);
                mobsDistances[kvp.Key] = distanceOfTheCurrentMobToPlayer;
            }

            GameObject closestEnemy = mobsDistances.Where(eb => eb.Value == mobsDistances.Values.Min()).FirstOrDefault().Key;
            GameObject originalPrefab = activeEnemiesObjects[closestEnemy];

            return new Dictionary<GameObject, GameObject> { { closestEnemy, originalPrefab } };
            
        }
        else
        {
            return null;
        }

    }
    Directions DirectionToLookForTheClosestEnemy(Dictionary<GameObject, GameObject> closestEnemy)
    {
        if (closestEnemy.ElementAt(0).Key.transform.position.x >= gameObject.transform.position.x)
        {
            return Directions.Right;
        }
        else
        {
            return Directions.Left;
        }
    }

    public bool IsBossActive()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene == "CrystalinePath")
        {
            var selenius = FindAnyObjectByType<Selenius>();
            return selenius != null && selenius.HP > 0;
        }
        else if (currentScene == "LeboliaMorass")
        {
            var remorseless = FindAnyObjectByType<RemorselessOne>();
            return remorseless != null && remorseless.HP > 0;
        }
        return false;
    }

    private void JumpToNextMap()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene == "CrystalinePath")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScreenBetweenLevels");
        }
    }

    public void GainXP(long XP)
    {
        PlaySFX(_coinSFX, 0.4f);
        if (_xpBarController != null)
            _xpBarController.AddXP(XP);
        _playerXPTotal += XP;
        _playerXPCurrent += XP;

        if (_playerXPCurrent >= _XPSO.LevelCaps[_playerLvl])
        {
            _playerLvl += 1;
            _playerXPCurrent = 0;
            if (_xpBarController != null)
            {
                _xpBarController.ResetMaskAfterLevelUp();
                _xpBarController.LevelUp();
            }
        }
    }

    public double GetCurrentXP()
    {
        return _playerXPCurrent;
    }

    public int GetCurrentLvl()
    {
        return _playerLvl;
    }

    public float GetCurrentHP()
    {
        return this.HP;
    }

    public void TakeContactDamage(float damage)
    {
        if (_isDead) return;
        if (_invincibilityTimer <= 0)
        {
            LooseHP(damage);
            _invincibilityTimer = _invincibilityDuration;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<IMob>() != null || collision.gameObject.GetComponentInParent<IMob>() != null)
        {
            TakeContactDamage(1f); // Take 1 damage per hit (adjust as needed)
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        OnTriggerStay2D(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        bool isEnemyProjectile = false;
        // Identify enemy projectile by specific script component to bypass missing tag issues
        if (collider.gameObject.GetComponent<SeleniteWalkerProjectile>() != null ||
            collider.gameObject.GetComponentInParent<SeleniteWalkerProjectile>() != null ||
            collider.gameObject.GetComponent<RemorselessOneProjectile>() != null ||
            collider.gameObject.GetComponentInParent<RemorselessOneProjectile>() != null)
        {
            isEnemyProjectile = true;
        }
        else
        {
            // Fallback safe tag check
            try
            {
                if (collider.gameObject.CompareTag("EnemyProjectile"))
                {
                    isEnemyProjectile = true;
                }
            }
            catch (System.Exception)
            {
                // Ignore missing tag exceptions
            }
        }

        if (isEnemyProjectile || collider.gameObject.GetComponent<IMob>() != null || collider.gameObject.GetComponentInParent<IMob>() != null)
        {
            TakeContactDamage(1f); // Take 1 damage per hit (adjust as needed)
        }
    }

    /// <summary>
    /// Called by HUDManager after HUD is created, to re-bind bar controllers.
    /// </summary>
    public void RebindHUDControllers()
    {
        if (HUDManager.Instance != null && HUDManager.Instance.BarsCanvas != null)
        {
            _xpBarController = HUDManager.Instance.BarsCanvas.GetComponentInChildren<XPBarController>(true);
            _hpBarController = HUDManager.Instance.BarsCanvas.GetComponentInChildren<HPBarController>(true);
        }
        else
        {
            var xpBarObj = GameObject.FindGameObjectWithTag("XPBar");
            if (xpBarObj != null)
                _xpBarController = xpBarObj.GetComponent<XPBarController>();

            var hpBarObj = GameObject.FindGameObjectWithTag("HPBar");
            if (hpBarObj != null)
                _hpBarController = hpBarObj.GetComponent<HPBarController>();
        }

        if (_xpBarController != null)
        {
            _xpBarController.Refresh();
        }
    }

    /// <summary>
    /// Resets all static and instance state for a fresh game start.
    /// Called by HUDManager when restarting after death.
    /// </summary>
    public static void ResetAllState()
    {
        _playerXPTotal = 0;
        _playerXPCurrent = 0;
        _playerLvl = 0;
        AttackType = ChosenBasicAttact.NotChosen;
        _attackProjectileSpawnTimer = 0f;
    }
}
