using AOSharp.Common.GameData;
using AOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill
{
    internal class DefaultAttackHandler : IAttackHandler
    {
        public bool IsActive(int playfield, Vector3 position)
        {
            return playfield != 152;
        }

        public IOrderedEnumerable<SimpleChar> PrioritizeTargets(IEnumerable<SimpleChar> possibleTargets)
        {
            return possibleTargets.OrderBy(npc => npc.Health);
        }

        public bool TargetsIncludePets()
        {
            return false;
        }

        public bool TargetsIncludePlayers()
        {
            return false;
        }
    }
}
