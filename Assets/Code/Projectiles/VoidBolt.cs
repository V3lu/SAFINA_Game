using Assets.Code.Interfaces;
using UnityEngine;

public class VoidBolt : MonoBehaviour, IProjectile
{
    [SerializeField] private VoidBoltBaseSO _voidBoltBaseSO;


    private int _stepCounter = 0;

    private Vector3 _direction = Vector3.zero;
    private float _distanceTraveled = 0f;

    void Start()
    {
        
    }

    private void ResetAfterPoolReturn()
    {
        _direction = Vector3.zero;
        _distanceTraveled = 0f;
        _stepCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_distanceTraveled >= _voidBoltBaseSO.GetRange())
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Projectiles);
            ResetAfterPoolReturn();
        }

        if (_stepCounter >= 40)
        {
            float moveStep = _voidBoltBaseSO.GetLoweredSpeed() * Time.deltaTime;
            _stepCounter++;
            transform.position += _direction * moveStep;
            _distanceTraveled += moveStep;
        }
        else
        {
            float moveStep = _voidBoltBaseSO.GetInitialSpeed() * Time.deltaTime;
            _stepCounter++;
            transform.position += _direction * moveStep;
            _distanceTraveled += moveStep;
        }

        if (_stepCounter % 40 == 0)
        {
            GameObject voidboltExplosion = ObjectPoolManager.SpawnObject(_voidBoltBaseSO.GetVoidBoltExplosionPrefab(), transform.position + _direction * 0.5f, Quaternion.identity);
            if (voidboltExplosion.TryGetComponent<IVFX>(out var vfx))
            {
                if (vfx is VoidBoltExplosionVFX explosion)
                {
                    explosion.SetTarget(_direction);
                }
            }
        }
    }

    public void SetTarget(Transform target)
    {
        _direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IMob>(out var mob))
        {
            mob.LooseHP(Random.Range(_voidBoltBaseSO.GetBaseDamageLowest(), _voidBoltBaseSO.GetBaseDamageHighest()));
        }
    }
}
