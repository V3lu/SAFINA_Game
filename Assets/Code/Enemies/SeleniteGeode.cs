using Assets.Code.Interfaces;
using UnityEngine;

public class SeleniteGeode : MonoBehaviour, IMob
{
    [SerializeField] private GameObject _hitPrefab;
    [SerializeField] private EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] private SeleniteGeodeSO _seleniteGeodeSO;
    [SerializeField] private CrystalinePathSO _crystalinePathSO;
    [SerializeField] private VialaTiny _vialaOrb;

    private Animator _animator;

    private static Transform _playerTransform;

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
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) < 1f)
        {
            _crystalinePathSO.RemoveEnemyFromList(this);
            OnDeath();
            return; 
        }

        if (_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0)
        {
            Movement();

            if (transform.position.x >= _playerTransform.position.x)
                _animator.SetInteger("state", 1);
            else
                _animator.SetInteger("state", 0);
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
        if (transform.position.x >= _playerTransform.position.x)
        {
            _animator.SetInteger("state", 11);
        }
        else
        {
            _animator.SetInteger("state", 10);
        }
           
    }

    void Movement()
    {
        if (this.HP > 0)
        {
            Vector3 moveDirNormalized = (_playerTransform.position - transform.position).normalized;
            transform.position += moveDirNormalized * _seleniteGeodeSO.MovSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }

    public void ResetState()
    {
        _animator.SetInteger("state", 15);
    }
}
