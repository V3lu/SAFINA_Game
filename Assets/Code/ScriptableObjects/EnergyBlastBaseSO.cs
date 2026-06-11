using UnityEngine;

[CreateAssetMenu(fileName = "EnergyBlastBaseSO", menuName = "Scriptable Objects/EnergyBlastBaseSO")]
public class EnergyBlastBaseSO : ScriptableObject
{
    [SerializeField] float _baseDamageLowest = 9;
    [SerializeField] float _baseDamageHighest = 13;


    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}
