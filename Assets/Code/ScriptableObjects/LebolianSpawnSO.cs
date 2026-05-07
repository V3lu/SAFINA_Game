using UnityEngine;

[CreateAssetMenu(fileName = "LebolianSpawnSO", menuName = "Scriptable Objects/LebolianSpawnSO")]
public class LebolianSpawnSO : ScriptableObject
{
    [SerializeField] string _name = "Lebolian Spawn";
    [SerializeField] string _description = "TODO Description";
    [SerializeField] float _movSpeed = 2;
    [SerializeField] float _hp = 50;


    public string Name => _name;
    public string Description => _description;
    public float MovSpeed => _movSpeed;
    public float HP => _hp;
}
