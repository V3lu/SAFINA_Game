using UnityEngine;

[CreateAssetMenu(fileName = "SeleniteWalkerSO", menuName = "Scriptable Objects/SeleniteWalkerSO")]
public class SeleniteWalkerSO : ScriptableObject
{
    [SerializeField] float _baseDamageLowest = 2;
    [SerializeField] float _baseDamageHighest = 16;
    [SerializeField] string _name = "Selenite Walker";
    [SerializeField] string _description = "TODO Description";
    [SerializeField] float _movSpeed = 1.3f;
    [SerializeField] float _hp = 16;
    [SerializeField] float _attackSpeed = 0.5f;
    [SerializeField] float _attackRangeDistanceToPlayerMin = 100f;
    [SerializeField] float _attackRangeDistanceToPlayerMax = 300f;

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
