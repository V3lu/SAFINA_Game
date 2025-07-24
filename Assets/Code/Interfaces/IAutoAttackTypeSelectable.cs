using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interfaces
{
    public interface IAutoAttackTypeSelectable
    {
        void Hoovered();
        void Unhoovered();
        void Selected();
    }
}
