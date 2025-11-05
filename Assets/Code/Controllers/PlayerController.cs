using Assets.Code.Interfaces;
using Assets.Scripts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour, IDamagable
{
    [SerializeField] CrystalinePathSO _crystalinePathSO;
    [SerializeField] XPSO _XPSO;
    [SerializeField] GameObject _fireballPrefab;
    [SerializeField] GameObject _energyBlastPrefab;
    [SerializeField] GameObject _voidBoltPrefab;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _movSpeed;


    static float _attackProjectileSpawnTimer;
    static float _speedX, _speedY;
    static Rigidbody2D _rb;
    static Animator _animator;
    static long _playerXPTotal = 0;
    static long _playerXPCurrent = 0;
    static int _playerLvl = 0;
    static XPBarController _xpBarController;

    public static ChosenBasicAttact AttackType = ChosenBasicAttact.NotChosen;

    public float HP { get; set; }
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
        this.HP -= hp;
        Debug.Log($"{this.HP} + HP remaining");
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
        _xpBarController = GameObject.FindGameObjectWithTag("XPBar").GetComponent<XPBarController>();
        this.HP = 50;
    }

    // Update is called once per frame
    void Update()
    {
        _speedX = Input.GetAxisRaw("Horizontal") * _movSpeed;
        _speedY = Input.GetAxisRaw("Vertical") * _movSpeed;
        _rb.linearVelocity = new Vector2(_speedX, _speedY);

        Dictionary<GameObject, GameObject> closestEnemyPlusOriginalPrefab = GetClosestEnemy();
        Directions directionToLookAt = Directions.Right;
        if (closestEnemyPlusOriginalPrefab != null)
        {
            directionToLookAt = DirectionToLookForTheClosestEnemy(closestEnemyPlusOriginalPrefab);
        }

        _attackProjectileSpawnTimer -= Time.deltaTime;
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

    public void GainXP(long XP)
    {
        _xpBarController.AddXP(XP);
        _playerXPTotal += XP;
        _playerXPCurrent += XP;

        if (_playerXPCurrent >= _XPSO.LevelCaps[_playerLvl])
        {
            _playerLvl += 1;
            _playerXPCurrent = 0;
            _xpBarController.ResetMaskAfterLevelUp();
            _xpBarController.LevelUp();
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
}
