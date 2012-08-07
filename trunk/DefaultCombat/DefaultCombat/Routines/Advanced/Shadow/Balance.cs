using System;
using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Routines
{
    // Balance Shadow v1.0 - Cystacae/Edited by Neo93
    // Known Bugs: I am nearly sure Base Shadow file interferes with proper rotation as well as Telekinetic Throw (Similar to Madness Assassin).  Force Breach and Mind Crush share same "Crushed (Force)" debuff name.  Can't Shadow Strike on Stealth Pull.  Waiting for some help on this.
    public static class ShadowBalance
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowBalance)]
        public static Composite ShadowBalancePull()
        {
            return new PrioritySelector(
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist),
                Spell.Cast("Shadow Strike", ret => BuddyTor.Me.HasBuff("Stealth")),
                Spell.CastOnGround("Force in Balance", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist, location => BuddyTor.Me.CurrentTarget.Position)
            );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowBalance)]
        public static Composite ShadowBalanceCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),
                
                //***Generel***		
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),//Speed
                Spell.Cast("Force Technique", ret => !BuddyTor.Me.HasBuff("Force Technique")),
                Spell.Cast("Force of Will", ret => BuddyTor.Me.IsStunned),//CC Break
                Spell.Cast("Force Potency", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Force Potency")),
                Spell.Cast("Blackout", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.ResourceStat < 90 && !BuddyTor.Me.HasBuff("Shadow's Respite")),
                Spell.Cast("Force Cloak", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Shadow's Respite")),
                Spell.Cast("Battle Readiness", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Battle Readiness")),

                //**Defensive**
                Spell.Cast("Deflection", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 80),
                Spell.Cast("Force Shroud", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 70),

                //**CC**
                Spell.Cast("Force Lift", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2),
                Spell.Cast("Force Stun", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2),
                Spell.Cast("Low Slash", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2),

                //*Interrupts*
                Spell.Cast("Mind Snap", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Force Stun", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Force Wave", ret => BuddyTor.Me.CurrentTarget.IsCasting),

                //Rotation
                Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Stealth")),
                Spell.CastOnGround("Force in Balance", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist, location => BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Mind Crush", castWhen => BuddyTor.Me.HasBuff("Force Strike")),
                Spell.Cast("Force Breach", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                Spell.Cast("Spinning Strike", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),
                Spell.Cast("Sever Force", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Sever Force (Force)")),
                Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Find Weakness")),
                Spell.Cast("Whirling Blow", castWhen => Helpers.Targets.Count() >= 3),
                Spell.Cast("Double Strike", castWhen => BuddyTor.Me.ResourceStat > 45),
                Spell.Cast("Saber Strike"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowBalance)]
        public static Composite ShadowBalanceOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Stealth", ret => !BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Force Technique", ret => !BuddyTor.Me.HasBuff("Force Technique"))
            );
        }
    }
}