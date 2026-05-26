using UnityEngine;

[CreateAssetMenu(fileName = "LebolianSpawnSO", menuName = "Scriptable Objects/LebolianSpawnSO")]
public class LebolianSpawnSO : ScriptableObject
{
    [SerializeField] string _name = "Lebolian Spawn";
    [SerializeField] string _description = "TODO Description";
    [SerializeField] float _movSpeed = 1.3f;
    [SerializeField] float _hp = 40;


    public string Name => _name;
    public string Description => _description;
    public float MovSpeed => _movSpeed;
    public float HP => _hp;
}
