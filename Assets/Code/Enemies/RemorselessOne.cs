using Assets.Code.Interfaces;
using UnityEngine;

public class RemorselessOne : MonoBehaviour, IMob
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] EnemyHealthbarController _enemyHealthbarController;

    float SortingPrecision = 10f;
    int SortingBase = 2000;
    Animator _animator;

    static Transform _playerTransform;

    public Transform Transform { get { return gameObject.transform; } }

    public float MaxHP { get; set; } = 200f;
    public float HP { get; set; } = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        this._animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= _playerTransform.position.x)
        {
            _animator.SetInteger("directionToLook", 1);
        }
        else
        {
            _animator.SetInteger("directionToLook", 0);
        }
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

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = SortingBase +
            Mathf.RoundToInt(-transform.position.y * SortingPrecision);
    }

    public void SpecialAction()
    {
        
    }

    public void MoveTo(Vector3 position, float moveSpeed)
    {
        
    }

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

    public void RestoreHP(float hp)
    {
    }
}
