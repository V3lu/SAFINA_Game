using UnityEngine;

[CreateAssetMenu(fileName = "MehrenSO", menuName = "Scriptable Objects/MehrenSO")]
public class MehrenSO : ScriptableObject
{
    [SerializeField] string _name = "Mehren";
    [SerializeField] string _description = "TODO Description";
    [SerializeField] float _movSpeed = 2.5f;
    [SerializeField] float _hp = 24;


    public string Name => _name;
    public string Description => _description;
    public float MovSpeed => _movSpeed;
    public float HP => _hp;
}
