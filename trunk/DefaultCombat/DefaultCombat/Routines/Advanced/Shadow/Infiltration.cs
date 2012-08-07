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
    // Infiltration Shadow v1.0 - Cystacae/Edited by Neo93
    // Known Bugs:  Can't get behind on pull, and same as madness/balance about their cast spell from base class.
    public static class ShadowInfiltration
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowInfiltration)]
        public static Composite ShadowInfiltrationPull()
        {
            return new PrioritySelector(
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist),
                Spell.Cast("Shadow Strike", ret => BuddyTor.Me.HasBuff("Stealth"))
            );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowInfiltration)]
        public static Composite ShadowInfiltrationCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***		
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),//Speed
                Spell.Cast("Shadow Technique", ret => !BuddyTor.Me.HasBuff("Shadow Technique")),
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
                Spell.Cast("Low Slash", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Force Wave", ret => BuddyTor.Me.CurrentTarget.IsCasting),

                //Rotation
                Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Project", ret => BuddyTor.Me.HasBuff("Circling Shadows") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Circling Shadows").Stacks > 1),
                Spell.Cast("Spinning Strike", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),
                Spell.Cast("Shadow Strike", castWhen => BuddyTor.Me.HasBuff("Find Weakness")),
                Spell.Cast("Force Breach", ret => BuddyTor.Me.HasBuff("Exit Strategy") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Exit Strategy").Stacks > 0),
                Spell.Cast("Whirling Blow", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),
                Spell.Cast("Clairvoyant Strike", castWhen => BuddyTor.Me.ResourceStat > 45),
                Spell.Cast("Saber Strike"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
             );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Consular, AdvancedClass.Shadow, SkillTreeId.ShadowInfiltration)]
        public static Composite ShadowInfiltrationOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Stealth", castWhen => !BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Shadow Technique", ret => !BuddyTor.Me.HasBuff("Shadow Technique"))
            );
        }
    }
}
