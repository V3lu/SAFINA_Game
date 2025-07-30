using Assets.Code.Interfaces;
using UnityEngine;

public class EnergyBlast : MonoBehaviour, IProjectile
{
    [SerializeField] private EnergyBlastBaseSO _energyBlastBaseSO;

    private Transform explosionPlace;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(Transform target)
    {
        explosionPlace = target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IMob>(out var mob))
        {
            mob.LooseHP(Random.Range(_energyBlastBaseSO.BaseDamageLowest, _energyBlastBaseSO.BaseDamageHighest));
        }
    }
}
