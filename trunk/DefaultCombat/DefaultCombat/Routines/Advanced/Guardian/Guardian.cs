using System;
using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class JediGuardian
    {
        //[Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianFocus)]
        //[Behavior(BehaviorType.Combat)]
        public static Composite GuardianFocusCombat()
        {
            return new PrioritySelector(
                Movement.StopInRange(0.4f),
                Spell.WaitForCast(),

                Spell.Cast("Resolute", ret => BuddyTor.Me.IsStunned),

                Spell.Cast("Riposte"),
                Spell.Cast("Blade Storm"),

                Spell.Cast("Warding Call", ret => BuddyTor.Me.HealthPercent <= 40),
                Spell.Cast("Enure", ret => BuddyTor.Me.HealthPercent <= 50),
                Spell.Cast("Saber Ward", ret => BuddyTor.Me.HealthPercent <= 70),

                Spell.Cast("Force Kick", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Force Stasis", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Awe", ret => Helpers.Targets.Count(t => t.IsCasting) >= 2),

                Spell.Cast("Cyclone Slash", ret => Helpers.Targets.Count() >= 2),

                Spell.Cast("Pommel Strike", ret => BuddyTor.Me.CurrentTarget.IsStunned),

                Spell.Cast("Force Sweep"),
                Spell.Cast("Hilt Strike"),
                Spell.Cast("Guardian Slash"),
                Spell.Cast("Slash"),

                Spell.Cast("Enrage"),
                Spell.Cast("Sundering Strike"),
                Spell.Cast("Strike"),

                Spell.Cast("Master Strike"),
                Spell.Cast("Force Leap"),
                Movement.MoveTo(ret=>BuddyTor.Me.CurrentTarget.Position,0.4f)
                );
        }

        //[Class(CharacterClass.Knight, AdvancedClass.Sentinel)]
        //[Behavior(BehaviorType.Pull)]
        //public static Composite KnightPull()
        //{
        //    return Spell.Cast("Force Leap", castWhen => BuddyTor.Me.CurrentTarget.Distance > BuddyTor.Me.MeleeDistance);
        //}
    }
}
