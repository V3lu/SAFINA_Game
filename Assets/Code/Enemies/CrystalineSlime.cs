
using Assets.Code.Interfaces;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;

public class CrystalineSlime : MonoBehaviour, IMob
{
    [SerializeField] private GameObject _hitPrefab;
    [SerializeField] private EnemyHealthbarController _enemyHealthbarController;
    [SerializeField] private CrystalineSlimeSO _crystalineSlimeSO;
    [SerializeField] private CrystalinePathSO _crystalinePathSO;


    private static Transform _playerTransform;


    public float HP { get; set; }
    public Transform Transform { get { return gameObject.transform; } } 


    void Start()
    {
        this.HP = _crystalineSlimeSO.GetHP();
        _enemyHealthbarController.Sethealth(HP, HP);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 moveDirNormalized = (_playerTransform.position - transform.position).normalized;
        transform.position += moveDirNormalized * _crystalineSlimeSO.GetMovSpeed() * Time.deltaTime;

        if(Vector3.Distance(transform.position, _playerTransform.position) < 1f)
        {
            _crystalinePathSO.RemoveEnemyFromList(this);
            OnDeath();
        }

        if(this.HP <= 0)
        {
            _crystalinePathSO.RemoveEnemyFromList(this);
            OnDeath();
        }
    }

    public void LooseHP(float hp)
    {
        this.HP -= hp;
        _enemyHealthbarController.Sethealth(HP, _crystalineSlimeSO.GetHP());
        ObjectPoolManager.SpawnObject(_hitPrefab, gameObject.transform.position, Quaternion.identity);
    }

    private void OnDeath()
    {
        ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Mobs);
        this.HP = _crystalineSlimeSO.GetHP();
        _enemyHealthbarController.Sethealth(HP, HP);
    }

    public void RestoreHP(float hp)
    {
        this.HP += hp;
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
