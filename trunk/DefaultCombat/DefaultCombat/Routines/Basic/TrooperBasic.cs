using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Basic
{
    public class Trooper
    {
        [Class(CharacterClass.Trooper)]
        [Behavior(BehaviorType.Combat)]
        public static Composite TrooperCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(2.8f),

                //Generel
                Spell.Cast("Tenacity", castWhen => BuddyTor.Me.IsStunned),
                Spell.Cast("Recharge Cells", ret => BuddyTor.Me, ret => BuddyTor.Me.ResourceStat <= 6),//+6

                //Offensive
                Spell.Cast("Sticky Grenade", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 10),//AoE
                Spell.Cast("Stockstrike", castWhen => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),//-2
                Spell.Cast("Full Auto", castWhen => BuddyTor.Me.ResourceStat >= 10),//-2
                Spell.Cast("Pulse Cannon", ret => BuddyTor.Me.CurrentTarget.Distance <= 1f),//Frontal
                Spell.Cast("High Impact Bolt", castWhen => (BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Burning")) && BuddyTor.Me.ResourceStat >= 10),//Dmg to incapacitated mobs
                Spell.Cast("Explosive Round", castWhen => BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Hammer Shot", castWhen => BuddyTor.Me.ResourceStat < 10),

                CommonBehaviors.MoveAndStop(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f, true)
                );
        }

        [Class(CharacterClass.Trooper)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite TrooperOutOfCombat()
        {
            return new PrioritySelector(
                Spell.BuffSelf("Plasma Cell")
                );
        }

        [Class(CharacterClass.Trooper)]
        [Behavior(BehaviorType.Pull)]
        public static Composite TrooperPull()
        {
            return TrooperCombat();
        }
    }
}