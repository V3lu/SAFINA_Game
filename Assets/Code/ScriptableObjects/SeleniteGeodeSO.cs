using UnityEngine;

[CreateAssetMenu(fileName = "SeleniteGeodeSO", menuName = "Scriptable Objects/SeleniteGeodeSO")]
public class SeleniteGeodeSO : ScriptableObject
{
    [SerializeField] private float _baseDamageLowest = 2;
    [SerializeField] private float _baseDamageHighest = 16;
    [SerializeField] private string _name = "Selenite Geode";
    [SerializeField] private string _description = "TODO Description";
    [SerializeField] private float _movSpeed = 1.3f;
    [SerializeField] private float _hp = 16;
    [SerializeField] private float _attackSpeed = 0.5f;
    [SerializeField] private float _attackRangeDistanceToPlayerMin = 100f;
    [SerializeField] private float _attackRangeDistanceToPlayerMax = 300f;

    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
    public float HP => _hp;
    public string Name => _name;
    public string Description => _description;
    public float MovSpeed => _movSpeed;
    public float AttackSpeed => _attackSpeed;
    public float MinDistToPlayer => _attackRangeDistanceToPlayerMin;
    public float MaxDistToPlayer => _attackRangeDistanceToPlayerMax;
}
