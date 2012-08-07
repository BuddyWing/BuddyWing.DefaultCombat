using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class Powertech
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.BountyHunter, AdvancedClass.Powertech, SkillTreeId.None)]
        public static Composite PowertechCombat()
        {
            return new PrioritySelector(
                Movement.StopInRange(2.8f),

                Spell.BuffSelf("Combustible Gas Cylinder", ret => !BuddyTor.Me.HasBuff("Combustible Gas Cylinder")),

                Spell.Cast("Kolto Overload", onUnit => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent <= 70),

                Spell.Cast("Death From Above"),
                Spell.Cast("Thermal Detonator"),
                Spell.Cast("Flame Thrower"),
                Spell.Cast("Vent Heat", ret => BuddyTor.Me.ResourceStat <= 3),
                Spell.Cast("Incendiary Missile", ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Incendiary Missile")),
                Spell.Cast("High Rail Shot"),
                Spell.Cast("Thermal Sensor Override", ret => AbilityManager.CanCast("Thermal Sensor Override", BuddyTor.Me.CurrentTarget) && AbilityManager.CanCast("Unload", BuddyTor.Me.CurrentTarget)),
                Spell.Cast("Power Shot", ret => BuddyTor.Me.ResourceStat > 1),
                Spell.Cast("Rapid Shots"),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
            );
        }
    }
}
