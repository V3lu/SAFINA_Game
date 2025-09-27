using UnityEngine;

[CreateAssetMenu(fileName = "EnergyBlastBaseSO", menuName = "Scriptable Objects/EnergyBlastBaseSO")]
public class EnergyBlastBaseSO : ScriptableObject
{
    [SerializeField] float _baseDamageLowest = 6;
    [SerializeField] float _baseDamageHighest = 10;


    public float BaseDamageLowest => _baseDamageLowest;
    public float BaseDamageHighest => _baseDamageHighest;
}
