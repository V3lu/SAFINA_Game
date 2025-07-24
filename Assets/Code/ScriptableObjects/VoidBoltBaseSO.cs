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


    public GameObject GetVoidBoltExplosionPrefab()
    {
        return _voidBoltExplosionPrefab;
    }

    public float GetLoweredSpeed()
    {
        return _loweredSpeed;
    }

    public float GetRange()
    {
        return _range;
    }

    public float GetInitialSpeed()
    {
        return _initialSpeed;
    }

    public float GetBaseDamageLowest()
    {
        return _baseDamageLowest;
    }

    public float GetBaseDamageHighest()
    {
        return _baseDamageHighest;
    }

}
