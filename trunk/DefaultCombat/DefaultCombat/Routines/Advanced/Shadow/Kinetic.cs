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
    // Fucking comment here because some ass broke shit.
    // Kinetic Shadow v1.0 - by Kickazz006
    public static class ShadowKinetic
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowCombat)]
        public static Composite ShadowKineticPull()
        {
            return new PrioritySelector(
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist),
                Spell.Cast("Shadow Strike", ret => BuddyTor.Me.HasBuff("Stealth"))
            );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowCombat)]
        public static Composite ShadowKineticCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),
                
                Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Stealth")),
				
                //***Generel***		
                Spell.Cast("Combat Technique", ret => !BuddyTor.Me.HasBuff("Combat Technique")),
                Spell.Cast("Force of Will", ret => BuddyTor.Me.IsStunned),//CC Break
                Spell.Cast("Force Potency", castWhen => (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Boss1) || (Helpers.Targets.Count() >= 5)),
               //Spell.Cast("Blackout", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.ResourceStat < 90 && !BuddyTor.Me.HasBuff("Shadow's Respite")),
                //Spell.Cast("Force Cloak", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Shadow's Respite")),
                //Spell.Cast("Battle Readiness", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Battle Readiness")),

                //**Defensive**
                Spell.BuffSelf("Kinetic Ward", ret => !BuddyTor.Me.HasBuff("Kinetic Ward")),
                Spell.Cast("Deflection", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50), // or if me is fighting a boss - always cast
                //Spell.Cast("Force Shroud", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 70),

                //**CC**
                //Spell.Cast("Force Lift", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2),
                //Spell.Cast("Force Stun", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2), - only when target is Strong or Boss1
                //Spell.Cast("Low Slash", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 2),

                //*Interrupts*
                Spell.Cast("Mind Snap", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Force Stun", castWhen => (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Boss1) || (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Strong)),
                Spell.Cast("Force Wave", castWhen => Helpers.Targets.Count(t => t.Distance <= .4f) >= 3),

                //Rotation
                Spell.Cast("Spinning Strike", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),
                Spell.Cast("Slow Time", ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Slow Time (Force)")),
                //Spell.CastOnGround("Force in Balance", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.rangeDist, location => BuddyTor.Me.CurrentTarget.Position),
                //Spell.Cast("Mind Crush", castWhen => BuddyTor.Me.HasBuff("Force Strike")),
				Spell.Cast("Tumult"),
				Spell.Cast("Telekinetic Throw", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
				Spell.Cast("Project", castWhen => BuddyTor.Me.ResourceStat > 45),
				Spell.Cast("Force Breach", ret => Helpers.Targets.Count(t => t.Distance <= .4f) >= 3),
                Spell.Cast("Force Breach", castWhen => (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Strong) || (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Boss1) || (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Boss2)),
                Spell.Cast("Whirling Blow", castWhen => Helpers.Targets.Count() >= 3),
                //Spell.Cast("Sever Force", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Sever Force (Force)")),
                //Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Find Weakness")),
                Spell.Cast("Double Strike", castWhen => BuddyTor.Me.ResourceStat > 75),
                Spell.Cast("Saber Strike"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowCombat)]
        public static Composite ShadowKineticOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Stealth", ret => !BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Combat Technique", ret => !BuddyTor.Me.HasBuff("Combat Technique"))
            );
        }
    }
}