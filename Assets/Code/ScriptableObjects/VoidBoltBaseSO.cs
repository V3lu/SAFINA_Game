using UnityEngine;

[CreateAssetMenu(fileName = "VoidBoltBaseSO", menuName = "Scriptable Objects/VoidBoltBaseSO")]
public class VoidBoltBaseSO : ScriptableObject
{
    [SerializeField] private GameObject _voidBoltExplosionPrefab;
    [SerializeField] private float _range = 8f;
    [SerializeField] private float _initialSpeed = 10f;
    [SerializeField] private float _loweredSpeed = 6f;
    [SerializeField] private float _baseDamageLowest = 4;
    [SerializeField] private float _baseDamageHighest = 8;


    public GameObject VoidBoltExplosionPrefab => _voidBoltExplosionPrefab;
    public float LoweredSpeed => _loweredSpeed;
    public float Range => _range;
    public float InitialSpeed => _initialSpeed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;

}
