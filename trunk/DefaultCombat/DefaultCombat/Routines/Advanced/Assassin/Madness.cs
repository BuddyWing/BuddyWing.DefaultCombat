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
    // Madness Assassin v1.0 - Cystacae/Edited by Neo93
    // Known Bugs: Can't Maul on Stealth Pull.  Waiting for some help on this.
    public static class AssassinMadness
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinMadness)]
        public static Composite AssassinMadnessPull()
        {
            return new PrioritySelector(
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist),
                Spell.Cast("Maul", ret => BuddyTor.Me.HasBuff("Stealth")),
                Spell.CastOnGround("Death Field", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist, location => BuddyTor.Me.CurrentTarget.Position)
            );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinMadness)]
        public static Composite AssassinMadnessCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                Spell.Cast("Force Speed", ret => BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist && BuddyTor.Me.CurrentTarget.Distance <= 4f),
                Spell.Cast("Lightning Charge", ret => !BuddyTor.Me.HasBuff("Lightning Charge")),
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
                Spell.Cast("Overload", ret => BuddyTor.Me.CurrentTarget.IsCasting),

                //Rotation
                Spell.Cast("Maul", ret => BuddyTor.Me.HasBuff("Stealth")),
                Spell.CastOnGround("Death Field", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist, location => BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Crushing Darkness", castWhen => BuddyTor.Me.HasBuff("Raze") && !BuddyTor.Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                Spell.Cast("Discharge", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Shocked (Force)")),
                Spell.Cast("Assassinate", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),
                Spell.Cast("Creeping Terror", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Creeping Terror (Force)")),
                Spell.Cast("Maul", castWhen => BuddyTor.Me.HasBuff("Exploit Weakness")),
                Spell.Cast("Lacerate", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),
                Spell.Cast("Thrash", castWhen => BuddyTor.Me.ResourceStat > 45),
                Spell.Cast("Saber Strike"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
            );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Assassin, SkillTreeId.AssassinMadness)]
        public static Composite AssassinMadnessOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Stealth", castWhen => !BuddyTor.Me.HasBuff("Stealth")),
                Spell.Cast("Lightning Charge", ret => !BuddyTor.Me.HasBuff("Lightning Charge"))
            );
        }

    }
}