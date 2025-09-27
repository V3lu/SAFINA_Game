using UnityEngine;

[CreateAssetMenu(fileName = "FireballBaseSO", menuName = "Scriptable Objects/FireballBaseSO")]
public class FireballBaseSO : ScriptableObject
{
    [SerializeField] GameObject _fireballExplosionPrefab;
    [SerializeField] float _range = 7f;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _baseDamageLowest = 2;
    [SerializeField] float _baseDamageHighest = 12;


    public GameObject FireballExplosionPrefab => _fireballExplosionPrefab;
    public float Range => _range;
    public float Speed => _speed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}

