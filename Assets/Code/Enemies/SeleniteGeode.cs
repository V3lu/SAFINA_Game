using Assets.Code.Interfaces;
using Assets.Scripts;
using UnityEngine;
using static PlayerCtrl;

public class SeleniteGeode : MonoBehaviour, IMob
{
    [SerializeField] private GameObject _hitPrefab;
    [SerializeField] private EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] private SeleniteGeodeSO _seleniteGeodeSO;
    [SerializeField] private CrystalinePathSO _crystalinePathSO;
    [SerializeField] private VialaTiny _vialaOrb;
    [SerializeField] private SeleniteGeodeProjectile _projectile;
    [SerializeField] private float _projectileMaxMoveSpeed;
    [SerializeField] private float _projectileMaxHeight;
    [SerializeField] private AnimationCurve _trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve _axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve __projectileSpeedAnimationCurve;

    private Animator _animator;
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


    private static PlayerCtrl _playerReference;

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

    void OnEnable()
    {
        this.HP = MaxHP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
    }

    void Start()
    {
        this.HP = _seleniteGeodeSO.HP;
        this.MaxHP = _seleniteGeodeSO.HP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        DetermineDistanceAndAction();

        switch (_currentAction)
        {
            case Actions.Escape:
                Movement();
                break;
            case Actions.Approach:
                Movement();
                break;
            case Actions.Attack:
                OnAttack();
                break;
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

    void OnDeath()
    {
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            _animator.SetInteger("state", 11);
        }
        else
        {
            _animator.SetInteger("state", 10);
        }
           
    }

    void OnAttack()
    {
        if (transform.position.x >= _playerReference.transform.position.x)
            _animator.SetInteger("state", 3);
        else
            _animator.SetInteger("state", 2);



        _attackProjectileSpawnTimer -= Time.deltaTime;
        
        if (_attackProjectileSpawnTimer <= 0)
        {
            _attackProjectileSpawnTimer = _seleniteGeodeSO.AttackSpeed;
            SeleniteGeodeProjectile seleniteGeodeProjectile = ObjectPoolManager.SpawnObject(_projectile, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
            seleniteGeodeProjectile.InitializeProjectile(_playerReference.transform.position, _projectileMaxMoveSpeed, _projectileMaxHeight);
            _trajectoryAnimationCurve.preWrapMode = WrapMode.Clamp;
            _trajectoryAnimationCurve.postWrapMode = WrapMode.Clamp;
            seleniteGeodeProjectile.InitializeAnimationCurves(_trajectoryAnimationCurve, _axisCorrectionAnimationCurve, __projectileSpeedAnimationCurve);
        }
        
    }
    void Movement()
    {
        switch (_distanceFromPlayer)
        {
            case DistancesFromPlayer.EscapeDistance:
                if (_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0)
                {
                    if (transform.position.x >= _playerReference.transform.position.x)
                        _animator.SetInteger("state", 0);
                    else
                        _animator.SetInteger("state", 1);
                }

                if (this.HP > 0)
                {
                    Vector3 moveDirNormalized = -((_playerReference.transform.position - transform.position).normalized);
                    transform.position += moveDirNormalized * _seleniteGeodeSO.MovSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += new Vector3(0, 0, 0);
                }
                break;
            case DistancesFromPlayer.AttackDistance:
                if (_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0)
                {
                    if (transform.position.x >= _playerReference.transform.position.x)
                        _animator.SetInteger("state", 1);
                    else
                        _animator.SetInteger("state", 0);
                }

                transform.position += new Vector3(0, 0, 0);
                break;
            case DistancesFromPlayer.ApproachDistance:
                if (_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0)
                {
                    if (transform.position.x >= _playerReference.transform.position.x)
                        _animator.SetInteger("state", 1);
                    else
                        _animator.SetInteger("state", 0);
                }

                if (this.HP > 0)
                {
                    Vector3 moveDirNormalized = (_playerReference.transform.position - transform.position).normalized;
                    transform.position += moveDirNormalized * _seleniteGeodeSO.MovSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += new Vector3(0, 0, 0);
                }
                break;
        }
    }

    void DetermineDistanceAndAction()
    {
        float magnitude = (_playerReference.transform.position - transform.position).magnitude;
        if(magnitude < _seleniteGeodeSO.MinDistToPlayer)
        {
            _currentAction = Actions.Escape;
            _distanceFromPlayer = DistancesFromPlayer.EscapeDistance;
        }
        else if(magnitude > _seleniteGeodeSO.MinDistToPlayer && magnitude < _seleniteGeodeSO.MaxDistToPlayer)
        {
            _currentAction = Actions.Attack;
            _distanceFromPlayer = DistancesFromPlayer.AttackDistance;
        }
        else
        {
            _currentAction = Actions.Approach;
            _distanceFromPlayer = DistancesFromPlayer.ApproachDistance;
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform == _playerReference.transform)
        {
            _playerReference.LooseHP(5);
        }
    }
    public void ResetState()
    {
        _animator.SetInteger("state", 15);
    }


    
}
