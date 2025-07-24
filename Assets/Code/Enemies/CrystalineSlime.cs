
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


    private Transform _target;


    public float HP { get; set; }
    public Transform Transform { get { return gameObject.transform; } } 


    void Start()
    {
        this.HP = _crystalineSlimeSO.GetHP();
        _enemyHealthbarController.Sethealth(HP, HP);
    }

    void Update()
    {
        Vector3 moveDirNormalized = (_target.position - transform.position).normalized;
        transform.position += moveDirNormalized * _crystalineSlimeSO.GetMovSpeed() * Time.deltaTime;

        if(Vector3.Distance(transform.position, _target.position) < 1f)
        {
            _crystalinePathSO.RemoveEnemyFromList(this);
            Destroy(gameObject);
        }

        if(this.HP <= 0)
        {
            _crystalinePathSO.RemoveEnemyFromList(this);
            Destroy(gameObject);
        }
    }

    public void Spawn(Transform target)
    {
        this._target = target;
    }

    public void LooseHP(float hp)
    {
        this.HP -= hp;
        _enemyHealthbarController.Sethealth(HP, _crystalineSlimeSO.GetHP());
        Instantiate(_hitPrefab, transform.position, Quaternion.identity);
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
