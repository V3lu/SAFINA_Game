using Assets.Code.Interfaces;
using Assets.Code.ScriptableObjects;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private FireballBaseSO _fireballBaseSO;


    private Vector3 _direction = Vector3.zero;
    private float _distanceTraveled = 0f;
    private bool _temporaryCollisionSolution = false;  //TODO Make this work properly in the future


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = _fireballBaseSO.Speed * Time.deltaTime;
        transform.position += _direction * moveStep;
        _distanceTraveled += moveStep;

        if (_distanceTraveled >= _fireballBaseSO.Range)
        {
            Destroy(gameObject);
        }

        if (_temporaryCollisionSolution)
        {
            Destroy(gameObject);
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
        GameObject gameObject = collision.gameObject;
        IMob mob = gameObject.GetComponent<IMob>();
        if (mob != null)
        {
            Instantiate(_fireballBaseSO.FireballExplosionPrefab, transform.position, Quaternion.identity);
            _temporaryCollisionSolution = true;
        }
    }
}
