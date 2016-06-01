using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using Buddy.Navigation;
using Buddywing;
using Buddy.CommonBot.Settings;
using DefaultCombat.Core;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Helpers
{

    //This shit comes from Joe's
    class Scavenge
    {
        private static TorPlayer Me { get { return BuddyTor.Me; } }

        private static bool HasScav { get { return Me.ProfessionInfos.Any(p => p.Name.Contains("Scavenging")); } }
        private static bool HasBio { get { return Me.ProfessionInfos.Any(p => p.Name.Contains("Bioanalysis")); } }


        public static bool CanHarvestCorpse(TorNpc unit)
        {
            ulong SkillReq = Math.Max(unit.Level - 3, 0) * 8;
            if (unit != null) if (!unit.IsDeleted) if (unit.IsDead && unit.Distance <= 4.0f 
                && ((HasBio && unit.CreatureType == Buddy.Swtor.CreatureType.Creature && PISkill("Bioanalysis") >= SkillReq) || (HasScav && unit.CreatureType == Buddy.Swtor.CreatureType.Droid && PISkill("Scavenging") >= SkillReq))
                && StrongOrGreater(unit) && !unit.IsFriendly) if (!Buddy.CommonBot.Blacklist.Contains(unit.Guid)) 
                    return true;

            return false;
        }

        public static ulong PISkill(string Skill)
        {
            foreach (Buddy.Swtor.Objects.Components.ProfessionInfo pi in BuddyTor.Me.ProfessionInfos) if (pi.Name.Contains(Skill)) return (ulong)pi.CurrentLevel;
            return 0;
        }

        public static bool StrongOrGreater(TorCharacter unit)
        {
            if (unit != null) if (unit.Toughness == CombatToughness.Strong || unit.Toughness == CombatToughness.Boss1 || unit.Toughness == CombatToughness.Boss2 || unit.Toughness == CombatToughness.Player) return true;
            return false;
        }

        public static TorCharacter ScavUnit
        {
            get
            {
                return ObjectManager.GetObjects<TorNpc>().Where(p => CanHarvestCorpse(p)).FirstOrDefault();
            }
        }

        public static Composite ScavengeCorpse
        {
            get
            {
                return new PrioritySelector(
                   new Decorator(ret => ScavUnit != null,
                        new PrioritySelector(
                            Spell.WaitForCast(),
                            CommonBehaviors.MoveAndStop(location => ScavUnit.Position, 0.2f, true),
                            new Action(ret => ScavUnit.Interact())
                            ))
                   );
            }
        }
    }
}
