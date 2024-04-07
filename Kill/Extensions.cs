using AOSharp.Common.GameData;
using AOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill
{
    public static class Extensions
    {
        public static bool InstanceEquals(this SimpleChar source, SimpleChar target)
        {
            if (source == null || source.Identity == null) return false;
            if (target == null || target.Identity == null) return false;

            return source.Identity.Type == target.Identity.Type && source.Identity.Instance == target.Identity.Instance;
        }

        /// <summary>
        /// Checks if a target is mezzed, and if so, if the remaining duration is greater than or equal to requiredRemainingTime
        /// </summary>
        /// <param name="character"></param>
        /// <param name="requiredRemainingTime"></param>
        public static bool IsMezzed(this SimpleChar character, float requiredRemainingTime = 10)
        {
            // check whether a target is mezzed, and if so, if the remaining duration is greater than requiredRemainingTime.
            var mezzNanoLines = new NanoLine[]
            {
                NanoLine.Mezz,
                (NanoLine)893, // lmn
                NanoLine.ProximityRangeDebuff
            };

            foreach (var nanoline in mezzNanoLines)
            {
                if (character.Buffs.Find(nanoline, out Buff mezz) && mezz.RemainingTime >= requiredRemainingTime)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a target that is attacking but is also mezzed. This can be an indication of a server-side bug that makes the target heal
        /// for its out of combat amount.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool HasBuggedMezz(this SimpleChar character)
        {
            return character.IsAttacking && character.IsMezzed();
        }

        public static bool CanMove(this SimpleChar character)
        {
            return character.MovementState != MovementState.Rooted;
        }

        public static bool CanBeAttacked(this SimpleChar character)
        {
            throw new NotImplementedException();
        }
    }
}
