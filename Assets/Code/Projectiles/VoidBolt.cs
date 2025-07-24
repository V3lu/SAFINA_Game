using Assets.Code.Interfaces;
using UnityEngine;

public class VoidBolt : MonoBehaviour
{
    [SerializeField] private VoidBoltBaseSO _voidBoltBaseSO;


    private int _stepCounter = 0;
    private float _distanceTraveled = 0f;
    private Vector3 _direction = Vector3.zero;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_distanceTraveled >= _voidBoltBaseSO.GetRange())
        {
            Destroy(gameObject);
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
            VoidBoltExplosionVFX voidboltExplosion = Instantiate(_voidBoltBaseSO.GetVoidBoltExplosionPrefab(), transform.position + _direction * 0.5f, Quaternion.identity).GetComponent<VoidBoltExplosionVFX>();
            voidboltExplosion.SetTarget(_direction);
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
        GameObject gameObject = collision.gameObject;
        IMob mob = gameObject.GetComponent<IMob>();
        if (mob != null)
        {
            mob.LooseHP(Random.Range(_voidBoltBaseSO.GetBaseDamageLowest(), _voidBoltBaseSO.GetBaseDamageHighest()));
        }
    }
}
