using AOSharp.Common.GameData;
using AOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill
{
    interface IAttackHandler
    {
        bool IsActive(int playfield, Vector3 position);

        IOrderedEnumerable<SimpleChar> PrioritizeTargets(IEnumerable<SimpleChar> possibleTargets);

        bool TargetsIncludePlayers();
        bool TargetsIncludePets();
    }
}
