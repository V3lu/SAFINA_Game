using UnityEngine;

[CreateAssetMenu(fileName = "SeleniteWalkerProjectileSO", menuName = "Scriptable Objects/SeleniteWalkerProjectileSO")]
public class SeleniteWalkerProjectileSO : ScriptableObject
{
    [SerializeField] GameObject _seleniteWalkerProjectileExplosionPrefab;
    [SerializeField] float _range = 7f;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _baseDamageLowest = 6;
    [SerializeField] float _baseDamageHighest = 16;


    public GameObject ExplosionPrefab => _seleniteWalkerProjectileExplosionPrefab;
    public float Range => _range;
    public float Speed => _speed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}
