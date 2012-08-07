using System;
using System.Linq;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Advanced.Sniper
{
    //http://swtorlevelguide.com/swtor-sniper-lethality-build/
    public class Lethality
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Agent, AdvancedClass.Sniper, SkillTreeId.SniperLethality)]
        public static Composite LethalityCombat()
        {
            if (BuddyTor.Me.Level < 36)
                return Sniper.Level10to36;

            return new PrioritySelector(
                Movement.StopInRange(Global.RangeDist),
                Spell.WaitForCast(),
                Movement.StopInRange(Global.RangeDist),
                Spell.WaitForCast(),
                Sniper.Cast("Ambush", castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !BuddyTor.Me.CurrentTarget.IsStunned),
                //buff snipe to ensure a crit
                Sniper.Cast("Laze Target",
                    ret =>
                        Global.IsInCover &&
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        AbilityManager.CanCast("Snipe", BuddyTor.Me.CurrentTarget)
                ),
                Sniper.SniperCombat(),
                Sniper.Cast("Rifle Shot", ret => !BuddyTor.Me.CurrentTarget.IsStunned),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
            );
        }

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Agent, AdvancedClass.Sniper, SkillTreeId.SniperLethality)]
        public static Composite LethalityPull()
        {
            if (BuddyTor.Me.Level < 36)
                return Sniper.Level10to36;

            return LethalityCombat();
        }
    }
}
