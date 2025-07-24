using Assets.Code.Interfaces;
using UnityEngine;

public class FireballExplosionAOEDamage : MonoBehaviour
{
    [SerializeField] private FireballBaseSO _fireballBaseSO;


    private CapsuleCollider2D _capsuleCollider2D;
    private bool _firstEnemyHit = true;


    void Start()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        IMob mob = gameObject.GetComponent<IMob>();
        if (mob != null)
        {
            if (_firstEnemyHit)
            {
                mob.LooseHP(_fireballBaseSO.BaseDamageHighest);
                _firstEnemyHit = false;
            }
            else
            {
                mob.LooseHP(Random.Range(_fireballBaseSO.BaseDamageLowest, _fireballBaseSO.BaseDamageHighest));
            }
        }
    }

    public void DisableCollider()
    {
        if(_capsuleCollider2D != null)
        {
            _capsuleCollider2D.enabled = false;
        }
    }
}
