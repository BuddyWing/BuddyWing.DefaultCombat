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
    // Deception Assassin v1.0 - Cystacae/Edited by Neo93
    // Known Bugs:  Can't get behind on pull, Also same as Madness Assassin that randomly Force Lightning may interfere.  I only say may because I have yet to see it but Madness code is relatively the same.
    public static class AssassinDarkness
    {

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinDarkness)]
        public static Composite AssassinDarknessPull()
        {
            return new PrioritySelector(
                //Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.meleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist),
                Spell.Cast("Maul", ret => BuddyTor.Me.HasBuff("Stealth"))
            );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinDarkness)]
        public static Composite AssassinDarknessCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                //Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.meleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Spell.Cast("Surging Charge", ret => !BuddyTor.Me.HasBuff("Surging Charge")),
                Spell.Cast("Unbreakable Will", ret => BuddyTor.Me.IsStunned),
                Spell.Cast("Recklessness", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Recklessness")),
                //Spell.Cast("Force Cloak", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Dark Embrace")),
                Spell.Cast("Overcharge Saber", castWhen => BuddyTor.Me.InCombat && !BuddyTor.Me.HasBuff("Overcharge")),

                //**Defensive**
                Spell.Cast("Deflection", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 80),
                Spell.Cast("Force Shroud", ret => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 70),

                //**CC**
                Spell.Cast("Whirlwind", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 3),
                Spell.Cast("Electrocute", on => Helpers.ccTarget(), castWhen => Helpers.Targets.Count() >= 3),

                //*Interrupts*
                Spell.Cast("Jolt", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Electrocute", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Low Slash", ret => BuddyTor.Me.CurrentTarget.IsCasting),
                Spell.Cast("Overload", ret => BuddyTor.Me.CurrentTarget.IsCasting),


                //Rotation
                Spell.Cast("Maul", castWhen => BuddyTor.Me.HasBuff("Stealth")),
				Spell.Cast("Force Lightning", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
                Spell.Cast("Shock", ret => BuddyTor.Me.HasBuff("Induction") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Induction").Stacks > 1),
                Spell.Cast("Assassinate", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),
                Spell.Cast("Maul", castWhen => BuddyTor.Me.HasBuff("Exploit Weakness")),
                Spell.Cast("Discharge", ret => BuddyTor.Me.HasBuff("Static Charge") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Static Charge").Stacks > 0),
                Spell.Cast("Lacerate", castWhen => Helpers.Targets.Count() >= 3),
                Spell.Cast("Voltaic Slash", castWhen => BuddyTor.Me.ResourceStat > 45),
                Spell.Cast("Saber Strike"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinDarkness)]
        public static Composite AssassinDarknessOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Stealth", castWhen => !BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Surging Charge", ret => !BuddyTor.Me.HasBuff("Surging Charge"))
            );
        }
    }
}