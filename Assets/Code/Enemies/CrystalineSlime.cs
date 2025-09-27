
using Assets.Code.Interfaces;
using Assets.Scripts;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class CrystalineSlime : MonoBehaviour, IMob
{
    [SerializeField] GameObject _hitPrefab;
    [SerializeField] EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] CrystalineSlimeSO _crystalineSlimeSO;
    [SerializeField] CrystalinePathSO _crystalinePathSO;
    [SerializeField] VialaTiny _vialaOrb;

    Animator _animator;

    static Transform _playerTransform;

    public float HP { get; set; }
    public Transform Transform { get { return gameObject.transform; } }

    public float MaxHP { get; set; }

    void Start()
    {
        this.HP = _crystalineSlimeSO.HP;
        this.MaxHP = _crystalineSlimeSO.HP;
        _enemyHealthbarController.Sethealth(HP, MaxHP);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if(!(_animator.GetInteger("state") == 1 || _animator.GetInteger("state") == 0))
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
            _crystalinePathSO.RemoveEnemyFromList(this);
            OnDeath();
        }
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
            transform.position += moveDirNormalized * _crystalineSlimeSO.MovSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, 0, 0);
        }
    }

    public void RestoreHP(float hp)
    {
        this.HP += hp;
    }

    public void resetState()
    {
        _animator.SetInteger("state", 2);
    }
    public void SpecialAction()
    {
        throw new System.NotImplementedException();
    }

    public void MoveTo(Vector3 position, float moveSpeed)
    {
        throw new System.NotImplementedException();
    }


}
