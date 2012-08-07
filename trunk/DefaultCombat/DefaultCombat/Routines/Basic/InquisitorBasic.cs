using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Basic
{
    public class Inquisitor
    {
        [Class(CharacterClass.Inquisitor)]
        [Behavior(BehaviorType.Combat)]
        public static Composite InquisitorCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(0.4f),

                //Generel
                Spell.Cast("Unbreakable Will", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
                Spell.Cast("Recklessness", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.ResourceStat >= 60),

                //CC
                Spell.Cast("Whirlwind", onUnit =>
                {
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    return
                        Helpers.Targets.FirstOrDefault(
                            t =>
                            t != previousTarget && (t.Toughness != CombatToughness.Player) && !t.IsDead) ??
                        Helpers.Targets.FirstOrDefault(t => t != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3),

                //Offensive
                Spell.Cast("Saber Strike", castWhen => BuddyTor.Me.ResourceStat < 25 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),
                Spell.Cast("Force Lightning", castWhen => BuddyTor.Me.ResourceStat >= 30 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),//-30F/3S channeled
                Spell.WaitForCast(),
                Spell.Cast("Shock", castWhen => BuddyTor.Me.ResourceStat >= 45),//Instant
                Spell.Cast("Electrocute", castWhen => (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Boss1) || (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Strong)),
                Spell.Cast("Overload", castWhen => Helpers.Targets.Count(t => t.Distance <= 0.8) >= 3),//Knockback
                Spell.Cast("Thrash",castWhen => BuddyTor.Me.ResourceStat >= 25 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f && !AbilityManager.CanCast("Shock", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Force Lightning", BuddyTor.Me.CurrentTarget)),

                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 0.4f)
                );
        }

        [Class(CharacterClass.Inquisitor)]
        [Behavior(BehaviorType.Pull)]
        public static Composite InquisitorPull()
        {
            return InquisitorCombat();
        }
    }
}