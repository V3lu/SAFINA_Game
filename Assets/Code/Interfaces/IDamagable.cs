using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IDamagable
    {
        float HP { get; set; }

        void LooseHP(float hp);
        void RestoreHP(float hp);
    }
}
