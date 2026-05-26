using Assets.Code.Interfaces;
using UnityEngine;

public class RemorselessOneProjectile : MonoBehaviour, IProjectile
{
    bool _temporaryCollisionSolution = false;  //TODO Make this work properly in the future
    Vector3 _direction = Vector3.zero;
    float _distanceTraveled = 0f;

    public void SetTarget(Vector3 position)
    {
        _direction = (position - transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = 5.5f * Time.deltaTime;
        transform.position += _direction * moveStep;
        _distanceTraveled += moveStep;

        if (_distanceTraveled >= 12f)
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

    void ResetAfterPoolReturn()
    {
        _direction = Vector3.zero;
        _distanceTraveled = 0f;
        _temporaryCollisionSolution = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.collider.TryGetComponent<PlayerCtrl>(out var player))
            {
                player.LooseHP(Random.Range(1, 5));
                _temporaryCollisionSolution = true;
            }
        }
    }
}
