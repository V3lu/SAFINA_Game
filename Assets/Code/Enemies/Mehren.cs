using Assets.Code.Interfaces;
using UnityEngine;

public class Mehren : MonoBehaviour, IMob
{
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] MehrenSO _mehrenSO;
    [SerializeField] VialaTiny _vialaOrb;

    Animator _animator;

    static Transform _playerTransform;

    public Transform Transform { get { return gameObject.transform; } }

    public float MaxHP { get; set; }
    public float HP { get; set; }

    void OnEnable()
    {
        this.HP = MaxHP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
    }

    public void LooseHP(float hp)
    {
        this.HP -= hp;

        if (this.HP <= 0)
        {
            _enemyHealthbarController.Sethealth(MaxHP, MaxHP);
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
            _animator.SetInteger("state", 1);
        }
        else
        {
            _animator.SetInteger("state", 0);
        }
    }

    void Movement()
    {
        if (this.HP > 0)
        {
            Vector3 moveDirNormalized = (_playerTransform.position - transform.position).normalized;
            transform.position += moveDirNormalized * _mehrenSO.MovSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }

    public void MoveTo(Vector3 position, float moveSpeed)
    {
        
    }

    public void RestoreHP(float hp)
    {
        
    }

    public void SpecialAction()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.HP = _mehrenSO.HP;
        this.MaxHP = _mehrenSO.HP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (!(_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0))
        {
            Movement();
        }

        if (transform.position.x >= _playerTransform.position.x)
        {
            _animator.SetInteger("directionToLook", 1);
        }
        else
        {
            _animator.SetInteger("directionToLook", 0);
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) < 1f)
        {
            OnDeath();
        }
    }

    public void ResetState()
    {
        _animator.SetInteger("state", 2);
    }
}
