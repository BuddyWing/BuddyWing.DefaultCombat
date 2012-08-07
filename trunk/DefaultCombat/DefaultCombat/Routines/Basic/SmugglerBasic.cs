using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Routines.Basic
{
    public class Smuggler
    {
        [Class(CharacterClass.Smuggler)]
        [Behavior(BehaviorType.Combat)]
        public static Composite SmugglerCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(2.8f),

                //Hawker 29/4
                Spell.Cast("Dirty Kick", castWhen => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),
                Spell.Cast("Blaster Whip", castWhen => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),

                //Generel
                Spell.Cast("Crouch", ret => BuddyTor.Me, ret => BuddyTor.Me.CurrentTarget.InLineOfSight && !BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover") && BuddyTor.Me.CurrentTarget.Distance <= 2.8f && BuddyTor.Me.InCombat),
                Spell.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned),//Insignia

                // AOE
                Spell.Cast("Thermal Grenade", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 80),

                //CC
                Spell.Cast("Flash Grenade", onUnit =>
                {
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    return
                        Helpers.Targets.FirstOrDefault(
                            t =>
                            t != previousTarget && (t.Toughness == CombatToughness.Strong)) ??
                        Helpers.Targets.FirstOrDefault(t => t != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3),

                //Offensive
                //Spell.Cast("Flurry of Bolts", castWhen => BuddyTor.Me.CurrentTarget.HasDebuff("Sabotage Charge")),//No Resource needed

                Spell.Cast("Sabotage Charge", castWhen => (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")) && BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Distance <= 2.8f),//Grenade
                Spell.Cast("Vital Shot", ctx => BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak && !BuddyTor.Me.CurrentTarget.HasBuff("Bleeding")),//Bleed
                Spell.Cast("Dirty Kick", castWhen => BuddyTor.Me.CurrentTarget.Distance <= 0.4f && BuddyTor.Me.ResourceStat >= 70),//Stun
                Spell.Cast("Blaster Whip", castWhen => BuddyTor.Me.CurrentTarget.Distance <= 0.4f && BuddyTor.Me.ResourceStat >= 75),//melee attack
                Spell.Cast("Quick Shot", castWhen => BuddyTor.Me.ResourceStat >= 77 && BuddyTor.Me.CurrentTarget.Distance <= 1f),
                Spell.Cast("Charged Burst", castWhen => (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")) && BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Distance <= 2.8f),
                Spell.Cast("Flurry of Bolts"),//No Resource needed

                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
                );
        }

        [Class(CharacterClass.Smuggler)]
        [Behavior(BehaviorType.Pull)]
        public static Composite SmugglerPull()
        {
            return SmugglerCombat();
        }
    }
}