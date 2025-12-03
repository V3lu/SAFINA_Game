using UnityEngine;

[CreateAssetMenu(fileName = "VoidBoltBaseSO", menuName = "Scriptable Objects/VoidBoltBaseSO")]
public class VoidBoltBaseSO : ScriptableObject
{
    [SerializeField] GameObject _voidBoltExplosionPrefab;
    [SerializeField] float _range = 8f;
    [SerializeField] float _initialSpeed = 10f;
    [SerializeField] float _loweredSpeed = 14f;
    [SerializeField] float _baseDamageLowest = 4;
    [SerializeField] float _baseDamageHighest = 8;


    public GameObject VoidBoltExplosionPrefab => _voidBoltExplosionPrefab;
    public float LoweredSpeed => _loweredSpeed;
    public float Range => _range;
    public float InitialSpeed => _initialSpeed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;

}
