using Assets.Code.Interfaces;
using Assets.Scripts;
using UnityEngine;
using static PlayerCtrl;

public class SeleniteGeode : MonoBehaviour, IMob
{
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] SeleniteWalkerSO _seleniteWalkerSO;
    [SerializeField] CrystalinePathSO _crystalinePathSO;
    [SerializeField] VialaTiny _vialaOrb;
    [SerializeField] SeleniteWalkerProjectile _projectile;
    [SerializeField] float _projectileMaxMoveSpeed;
    [SerializeField] float _projectileMaxHeight;
    [SerializeField] AnimationCurve _trajectoryAnimationCurve;
    [SerializeField] AnimationCurve _axisCorrectionAnimationCurve;
    [SerializeField] AnimationCurve __projectileSpeedAnimationCurve;

    GameObject _playerReference;
    Animator _animator;
    float _attackProjectileSpawnTimer;

    enum Actions
    {
        Attack,
        Escape,
        Approach
    }
    enum DistancesFromPlayer
    {
        AttackDistance,
        EscapeDistance,
        ApproachDistance
    }

    Actions _currentAction;
    DistancesFromPlayer _distanceFromPlayer;


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
        this.HP = _seleniteWalkerSO.HP;
        this.MaxHP = _seleniteWalkerSO.HP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _animator = this.GetComponent<Animator>();
        this._playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
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
        _attackProjectileSpawnTimer -= Time.deltaTime;
        
        if (_attackProjectileSpawnTimer <= 0)
        {
            if (transform.position.x >= _playerReference.transform.position.x)
            {
                _animator.SetInteger("state", 3);
            }
            else
            {
                _animator.SetInteger("state", 2);
            }
            _attackProjectileSpawnTimer = _seleniteWalkerSO.AttackSpeed;
            SeleniteWalkerProjectile seleniteWalkerProjectile = ObjectPoolManager.SpawnObject(_projectile, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Projectiles);
            seleniteWalkerProjectile.InitializeProjectile(_playerReference.transform ,_playerReference.transform.position, _projectileMaxMoveSpeed, _projectileMaxHeight, this.transform.position);
            _trajectoryAnimationCurve.preWrapMode = WrapMode.Clamp;
            _trajectoryAnimationCurve.postWrapMode = WrapMode.Clamp;
            seleniteWalkerProjectile.InitializeAnimationCurves(_trajectoryAnimationCurve, _axisCorrectionAnimationCurve, __projectileSpeedAnimationCurve);
        }
        
    }
    
    void OnEscape()
    {
        print("escape");
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            _animator.SetInteger("state", 4);
        }
        else
        {
            _animator.SetInteger("state", 5);
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

    void OnApproach()
    {
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            _animator.SetInteger("state", 1);
        }
        else
        {
            _animator.SetInteger("state", 0);
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

    void DetermineDistanceAndAction()
    {
        float magnitude = (_playerReference.transform.position - transform.position).magnitude;
        if(magnitude < _seleniteWalkerSO.MinDistToPlayer)
        {
            _currentAction = Actions.Escape;
            _distanceFromPlayer = DistancesFromPlayer.EscapeDistance;
        }
        else if(magnitude > _seleniteWalkerSO.MinDistToPlayer && magnitude < _seleniteWalkerSO.MaxDistToPlayer)
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
            _playerReference.GetComponent<PlayerCtrl>().LooseHP(5);
        }
    }
    public void ResetState()
    {
        _animator.SetInteger("state", 15);
    }


    
}
