using Assets.Code.Interfaces;
using UnityEngine;

public class VoidBoltExplosionVFX : MonoBehaviour, IVFX
{
    private Vector3 _direction = Vector3.zero;

    
    void Start()
    {

    }

    
    void Update()
    {
        
    }
    public void SetTarget(Vector3 target)
    {
        _direction = target;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
