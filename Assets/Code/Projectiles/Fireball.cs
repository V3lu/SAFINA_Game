using Assets.Code.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball : MonoBehaviour, IProjectile
{
    [SerializeField] private FireballBaseSO _fireballBaseSO;


    private bool _temporaryCollisionSolution = false;  //TODO Make this work properly in the future
    private Vector3 _direction = Vector3.zero;
    private float _distanceTraveled = 0f;

    void Start()
    {
        
    }

    private void ResetAfterPoolReturn()
    {
        _direction = Vector3.zero;
        _distanceTraveled = 0f;
        _temporaryCollisionSolution = false;
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = _fireballBaseSO.Speed * Time.deltaTime;
        transform.position += _direction * moveStep;
        _distanceTraveled += moveStep;

        if (_distanceTraveled >= _fireballBaseSO.Range)
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Projectiles);
            ResetAfterPoolReturn();
        }

        if (_temporaryCollisionSolution)
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Projectiles);
            ResetAfterPoolReturn();
        }
    }

    public void SetTarget(Transform target)
    {
        _direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.TryGetComponent<IMob>(out var mob))
            {
                mob.LooseHP(Random.Range(_fireballBaseSO.BaseDamageLowest, _fireballBaseSO.BaseDamageHighest));
            }
            GameObject explosionPrefab = _fireballBaseSO.FireballExplosionPrefab;
            ObjectPoolManager.SpawnObject(explosionPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.VFXs);
            _temporaryCollisionSolution = true;
        }
    }
}
