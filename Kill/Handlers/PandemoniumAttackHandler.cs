using AOSharp.Common.GameData;
using AOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill.Handlers
{
    internal class PandemoniumAttackHandler : IAttackHandler
    {
        private int[] validPlayfields =
        {
            4328, // Caina
            4329, // Antenora
            4330, // Ptolmea
            4331, // Judecca
            4389, // Caina + Antenora + Ptolemea (ipande)
            4391  // Judecca (ipande)
        };

        private static string pinkName = "Corrupted Xan-Len";
        private static string spiderName = "Corrupted Xan-Kuir";
        private static string blueName = "Corrupted Xan-Cur";
        private static string redName = "Corrupted Hiisi Berserker";
        private static string ameshaVizareshName = "Amesha Vizaresh";

        private List<string> standardTargetNames = new List<string>
        {
            pinkName,
            spiderName,
            blueName,
            redName,
            "Yuttos Pandemonium Geosurvey Dog",
            "Vizaresh",
            "Zodiac"
        };

        public bool IsActive(int playfield, Vector3 position)
        {
            return validPlayfields.Contains(playfield);
        }

        public IOrderedEnumerable<SimpleChar> PrioritizeTargets(IEnumerable<SimpleChar> possibleTargets)
        {
            return possibleTargets
                .Where(target =>
                    target.Name != ameshaVizareshName &&
                    !target.Buffs.Contains(253953) && // beast 1k reflect
                    !target.Buffs.Contains(205607) && // immortal
                    !target.HasBuggedMezz()) // attacking but mezzed - do not attack to let it break
                .OrderByDescending(target => !target.IsMezzed()) // prioritize targets that are not mezzed
                .ThenByDescending(target => target.Name == pinkName && target.DistanceFrom(DynelManager.LocalPlayer) < 20) // prioritize pinks that are close even if they are not fighting us
                .ThenByDescending(target => target.IsAttacking && standardTargetNames.Contains(target.Name)) // prioritize remaining non-boss targets that are already fighting us
                .ThenByDescending(target => target.Name == pinkName) // prioritize all pinks within range after targets that are fighting us have been defeated
                .ThenByDescending(target => standardTargetNames.Contains(target.Name) && target.HealthPercent < 70) // finish off standard mobs that are under 70% rather than prioritizing by type
                .ThenByDescending(target => target.Name == spiderName)
                .ThenByDescending(target => target.Name == blueName && !target.Buffs.Contains(NanoLine.ReflectShield)) // blues sometimes have tms running, don't prioritize them in this case
                .ThenByDescending(target => target.Name == redName)
                .ThenByDescending(target => standardTargetNames.Contains(target.Name)) // prioritize standard targets over bosses
                .ThenBy(target => target.HealthPercent)
                .ThenBy(target => target.DistanceFrom(DynelManager.LocalPlayer));
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
