using Assets.Code.Interfaces;
using System;
using UnityEngine;

public class Selenius : MonoBehaviour, IMob
{
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] VialaTiny _vialaOrb;

    Animator _animator;

    static Transform _playerTransform;

    public Transform Transform { get { return gameObject.transform; } }
    public float MaxHP { get; set; } = 200;
    public float HP { get; set; } = 200;

    public void LooseHP(float hp)
    {
        this.HP -= hp;

        if (this.HP <= 0)
        {
            OnDeath();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
