
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Interfaces
{
    public interface IMob : IDamagable
    {
        Transform Transform { get; }
        float MaxHP { get; set; }
        void SpecialAction();
        void MoveTo(Vector3 position, float moveSpeed);
    }
}
