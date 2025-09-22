using UnityEngine;

[CreateAssetMenu(fileName = "CrystalineSlimeSO", menuName = "Scriptable Objects/CrystalineSlimeSO")]
public class CrystalineSlimeSO : ScriptableObject
{
    [SerializeField] private string _name = "Crystaline Slime";
    [SerializeField] private string _description = "TODO Description";
    [SerializeField] private float _movSpeed = 2;
    [SerializeField] private float _hp = 50;


    public string GetName()
    {
        return _name;
    }

    public string GetDescription()
    {
        return _description;
    }

    public float GetMovSpeed()
    {
        return _movSpeed;
    }

    public float GetHP()
    {
        return _hp;
    }
}
