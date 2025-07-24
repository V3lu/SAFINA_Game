using UnityEngine;

[CreateAssetMenu(fileName = "FireballBaseSO", menuName = "Scriptable Objects/FireballBaseSO")]
public class FireballBaseSO : ScriptableObject
{
    [SerializeField] private GameObject _fireballExplosionPrefab;
    [SerializeField] private float _range = 7f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _baseDamageLowest = 2;
    [SerializeField] private float _baseDamageHighest = 12;


    public GameObject FireballExplosionPrefab => _fireballExplosionPrefab;
    public float Range => _range;
    public float Speed => _speed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}

