using Assets.Code.Interfaces;
using Assets.Scripts;
using UnityEngine;
using static PlayerCtrl;

public class SeleniteGeode : MonoBehaviour, IMob
{
    [SerializeField] private GameObject _hitPrefab;
    [SerializeField] private EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] private SeleniteWalkerSO _seleniteWalkerSO;
    [SerializeField] private CrystalinePathSO _crystalinePathSO;
    [SerializeField] private VialaTiny _vialaOrb;
    [SerializeField] private SeleniteWalkerProjectile _projectile;
    [SerializeField] private float _projectileMaxMoveSpeed;
    [SerializeField] private float _projectileMaxHeight;
    [SerializeField] private AnimationCurve _trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve _axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve __projectileSpeedAnimationCurve;

    private GameObject _playerReference;
    private Animator _animator;
    private SpriteRenderer spriteRenderer;
    private float _attackProjectileSpawnTimer;

    private enum Actions
    {
        Attack,
        Escape,
        Approach
    }

    private enum DistancesFromPlayer
    {
        AttackDistance,
        EscapeDistance,
        ApproachDistance
    }

    private Actions _currentAction;
    private DistancesFromPlayer _distanceFromPlayer;


    public Transform Transform { get { return gameObject.transform; } }
    public float MaxHP { get; set; }
    public float HP { get; set; }

    public void MoveTo(Vector3 position, float moveSpeed)
    {
        
    }

    public void RestoreHP(float hp)
    {
        this.HP += hp;
    }

    public void SpecialAction()
    {
        
    }

    private void OnEnable()
    {
        this.HP = MaxHP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
    }

    private void Start()
    {
        this.HP = _seleniteWalkerSO.HP;
        this.MaxHP = _seleniteWalkerSO.HP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _animator = this.GetComponent<Animator>();
        this._playerReference = GameObject.FindGameObjectWithTag("Player");
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (this.HP > 0)
        {
            DetermineDistanceAndAction();

            switch (_currentAction)
            {
                case Actions.Escape:
                    OnEscape();
                    break;
                case Actions.Approach:
                    OnApproach();
                    break;
                case Actions.Attack:
                    OnAttack();
                    break;
            }
        }
    }

    public void LooseHP(float hp)
    {
        this.HP -= hp;

        if (this.HP <= 0)
        {
            _enemyHealthbarController.Sethealth(MaxHP, MaxHP);
            _crystalinePathSO.RemoveEnemyFromList(this);
            OnDeath();
        }
        else
        {
            _enemyHealthbarController.Sethealth(HP, MaxHP);
        }

        ObjectPoolManager.SpawnObject(_hitPrefab, gameObject.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.VFXs);
    }

    private void OnDeath()
    {
        switch (_currentAction)
        {
            case Actions.Escape:
                _animator.SetInteger("death", 2);
                break;
            case Actions.Approach:
                _animator.SetInteger("death", 0);
                break;
            case Actions.Attack:
                _animator.SetInteger("death", 1);
                break;
        }

    }

    private void OnAttack()
    {
        _attackProjectileSpawnTimer -= Time.deltaTime;
        
        if (_attackProjectileSpawnTimer <= 0)
        {
            _animator.SetInteger("state", 1);
            _attackProjectileSpawnTimer = _seleniteWalkerSO.AttackSpeed;
            SeleniteWalkerProjectile seleniteWalkerProjectile = ObjectPoolManager.SpawnObject(_projectile, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
            seleniteWalkerProjectile.InitializeProjectile(_playerReference.transform ,_playerReference.transform.position, _projectileMaxMoveSpeed, _projectileMaxHeight, this.transform.position);
            _trajectoryAnimationCurve.preWrapMode = WrapMode.Clamp;
            _trajectoryAnimationCurve.postWrapMode = WrapMode.Clamp;
            seleniteWalkerProjectile.InitializeAnimationCurves(_trajectoryAnimationCurve, _axisCorrectionAnimationCurve, __projectileSpeedAnimationCurve);
        }
        
    }
    
    private void OnEscape()
    {
        _animator.SetInteger("state", 2);
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (this.HP > 0)
        {
            Vector3 moveDirNormalized = -((_playerReference.transform.position - transform.position).normalized);
            transform.position += moveDirNormalized * _seleniteWalkerSO.MovSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }

    private void OnApproach()
    {
        _animator.SetInteger("state", 0);
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (this.HP > 0)
        {
            Vector3 moveDirNormalized = (_playerReference.transform.position - transform.position).normalized;
            transform.position += moveDirNormalized * _seleniteWalkerSO.MovSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }

    private void DetermineDistanceAndAction()
    {
        float magnitude = (_playerReference.transform.position - transform.position).magnitude;
        if(magnitude < _seleniteWalkerSO.MinDistToPlayer)
        {
            _currentAction = Actions.Escape;
            _distanceFromPlayer = DistancesFromPlayer.EscapeDistance;
            _animator.SetInteger("state", 2);
        }
        else if(magnitude > _seleniteWalkerSO.MinDistToPlayer && magnitude < _seleniteWalkerSO.MaxDistToPlayer)
        {
            _currentAction = Actions.Attack;
            _distanceFromPlayer = DistancesFromPlayer.AttackDistance;
            _animator.SetInteger("state", 1);
        }
        else
        {
            _currentAction = Actions.Approach;
            _distanceFromPlayer = DistancesFromPlayer.ApproachDistance;
            _animator.SetInteger("state", 0);
        }

    }

    public void ResetState()
    {
        _animator.SetInteger("death", 3);
        _animator.SetInteger("state", 0);
    }

}
