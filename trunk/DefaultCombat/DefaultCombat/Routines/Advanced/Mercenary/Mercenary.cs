using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class Mercenary
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.BountyHunter, AdvancedClass.Mercenary, SkillTreeId.None)]
        public static Composite MercenaryCombat()
        {
            return new PrioritySelector(
                Movement.StopInRange(2.8f),
                Spell.WaitForCast(),

                Spell.Cast("Electro-Dart", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Rocket Punch", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Jet Boost", ret => Helpers.Targets.Count(t => t.IsCasting) > 3),

                Spell.Cast("Determination", ret => BuddyTor.Me.IsStunned),
                Spell.Cast("High Velocity Gas Cylinder", ret => !BuddyTor.Me.HasBuff("High Velocity Gas Cylinder")),
                Spell.Cast("Hunter's Boon", ret => !BuddyTor.Me.HasBuff("Hunter's Boon")),

                Spell.Cast("Kolto Overload", ret => BuddyTor.Me.HealthPercent <= 70),
                Spell.Cast("Energy Shield", ret => BuddyTor.Me.HealthPercent <= 60),
                Spell.Cast("Heroic Moment: On the Trail", ret => BuddyTor.Me.HealthPercent <= 50),

                Spell.Cast("Thermal Sensor Override", ret => BuddyTor.Me.ResourceStat >= 23),
                Spell.Cast("Vent Heat", ret => BuddyTor.Me.ResourceStat >= 80),
                Spell.Cast("Rapid Shots", ret => BuddyTor.Me.ResourceStat > 23 && !AbilityManager.CanCast("Vent Heat", BuddyTor.Me.CurrentTarget)),

                Spell.Cast("Fusion Missile", ret => Helpers.Targets.Count() > 3),
                Spell.Cast("Death From Above", ret => Helpers.Targets.Count() > 3),
                Spell.Cast("Explosive Dart", ret => Helpers.Targets.Count() > 3),
                Spell.Cast("Flame Thrower", ret => Helpers.Targets.Count() > 3),

                Spell.Cast("Tracer Missile", ret => !BuddyTor.Me.HasDebuff("Heat Signature")),

                Spell.Cast("Unload", ret => BuddyTor.Me.HasBuff("Barrage")),
                Spell.Cast("Rail Shot"),
                Spell.Cast("Unload"),
                Movement.MoveTo(ret=>BuddyTor.Me.CurrentTarget.Position,2.8f)

                );
        }
    }
}
