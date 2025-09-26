using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Interfaces
{
    public interface IProjectile
    {
        void SetTarget(Vector3 position);
    }
}
