using Assets.Code.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalinePathSO", menuName = "Scriptable Objects/CrystalinePathSO")]
public class CrystalinePathSO : ScriptableObject
{
    private List<IMob> _presentEnemies = new();


    public void AddEnemy(IMob mob)
    {
        _presentEnemies.Add(mob);
    }

    public List<IMob> GetAllEnemies()
    {
        return _presentEnemies;
    }

    public void RemoveEnemyFromList(IMob mob)
    {
        _presentEnemies.Remove(mob);
    }

}
