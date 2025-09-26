using UnityEngine;

[CreateAssetMenu(fileName = "SeleniteGeodeProjectileSO", menuName = "Scriptable Objects/SeleniteGeodeProjectileSO")]
public class SeleniteGeodeProjectileSO : ScriptableObject
{
    [SerializeField] private GameObject _seleniteGeodeProjectileExplosionPrefab;
    [SerializeField] private float _range = 7f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _baseDamageLowest = 6;
    [SerializeField] private float _baseDamageHighest = 16;


    public GameObject ExplosionPrefab => _seleniteGeodeProjectileExplosionPrefab;
    public float Range => _range;
    public float Speed => _speed;
    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}
