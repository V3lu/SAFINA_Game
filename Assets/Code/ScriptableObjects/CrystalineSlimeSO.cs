using UnityEngine;

[CreateAssetMenu(fileName = "CrystalineSlimeSO", menuName = "Scriptable Objects/CrystalineSlimeSO")]
public class CrystalineSlimeSO : ScriptableObject
{
    [SerializeField] private string _name = "Crystaline Slime";
    [SerializeField] private string _description = "TODO Description";
    [SerializeField] private float _movSpeed = 2;
    [SerializeField] private float _hp = 50;


    public string Name => _name;
    public string Description => _description;
    public float MovSpeed => _movSpeed;
    public float HP => _hp;
}
