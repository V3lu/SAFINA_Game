using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.ScriptableObjects
{
    public class EnergyBlastBaseSO
    {
        [SerializeField] private float _baseDamageLowest = 6;
        [SerializeField] private float _baseDamageHighest = 10;


        public float BaseDamageLowest => _baseDamageLowest;
        public float BaseDamageHighest => _baseDamageHighest;
    }
}
