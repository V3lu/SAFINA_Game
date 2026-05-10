using Assets.Code.Interfaces;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Selenius : MonoBehaviour, IMob
{
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] VialaTiny _vialaOrb;
    [SerializeField] EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] SpriteRenderer spriteRenderer;

    Animator _animator;
    float SortingPrecision = 10f;
    private const int SortingBase = 1000;

    static Transform _playerTransform;

    public Transform Transform { get { return gameObject.transform; } }
    public float MaxHP { get; set; } = 200;
    public float HP { get; set; } = 200;

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = SortingBase +
            Mathf.RoundToInt(-transform.position.y * SortingPrecision);
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

    public void MoveTo(Vector3 position, float moveSpeed)
    {
       
    }

    public void RestoreHP(float hp)
    {
        
    }

    public void SpecialAction()
    {
        
    }

    void OnEnable()
    {
        this.HP = MaxHP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
    }

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

    private void OnDeath()
    {
        SceneManager.LoadScene("LoadingScreenBetweenLevels");
    }

    private void Movement()
    {
        if (this.HP > 0)
        {
            Vector3 moveDirNormalized = (_playerTransform.position - transform.position).normalized;
            transform.position += moveDirNormalized * 1.4f * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }
}
