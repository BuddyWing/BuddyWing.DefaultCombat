using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    //Neo93
    public static class SorcererCorruption
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererCorruption)]
        public static Composite SorcererCorruptionPull()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.IsInGroup, new Action(ret => RunStatus.Success)),
                    new Decorator(ret => !Global.IsInGroup, SorcererCorruptionCombat())
                );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererCorruption)]
        public static Composite SorcererCorruptionCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.RangeDist),
                //Generel
                Spell.Cast("Unbreakable Will", castWhen => BuddyTor.Me.IsStunned),
                Spell.Cast("Recklessness", castWhen => Helpers.Tank.HealthPercent <= Global.HighHealth),//Buff for 2 spells

                //Spell.Cast("Purge", on => Helpers.DebuffTarget, ret => Helpers.DebuffTarget.Debuffs.Count() >= 2),//Cleanse (2 debuffs)
                Spell.Cast("Static Barrier", on => Helpers.Tank, ret => !Helpers.Tank.HasBuff("Static Barrier") && !Helpers.Tank.HasDebuff("Deionized")),//Shield
                Spell.Cast("Resurgence", on => Helpers.Tank, ret => (AbilityManager.HasAbility("Reconstruct") && !Helpers.Tank.HasBuff("Reconstruct")) ),//HoT & Armorbuff
                Spell.Cast("Consumption", ret => (BuddyTor.Me.HasBuff("Force Surge") || BuddyTor.Me.Debuffs.FirstOrDefault(d => d.Name == "Consumption").Stacks < 2) && (BuddyTor.Me.HealthPercent >= 90 || (BuddyTor.Me.EnergyMax - BuddyTor.Me.Energy) >= 50)),//Restores Force
                Spell.CastOnGround("Revivification", ret => Helpers.Group.Where(p => p.HealthPercent <= Global.HighHealth && p.Distance <= 0.4f).Count() >= 2 && BuddyTor.Me.HasBuff("Force Bending"), on => Helpers.Group.FirstOrDefault(p => p.HealthPercent <= Global.HighHealth).Position),//AoE Heal
                Spell.Cast("Static Barrier", on => Helpers.LowestHealthPlayer, ret => !Helpers.LowestHealthPlayer.HasBuff("Static Barrier") && !Helpers.LowestHealthPlayer.HasDebuff("Deionized") && Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth),//Shield
                Spell.Cast("Dark Heal", on => Helpers.LowestHealthPlayer, ret => Helpers.LowestHealthPlayer.HealthPercent <= Global.OhShit),//mid heal - low casttime
                Spell.Cast("Dark Infusion", on => Helpers.LowestHealthPlayer, ret => Helpers.LowestHealthPlayer.HealthPercent <= Global.LowHealth),
                Spell.Cast("Resurgence", on => Helpers.LowestHealthPlayer, ret => Helpers.LowestHealthPlayer.HealthPercent <= Global.HighHealth && !Helpers.LowestHealthPlayer.HasBuff("Resurgence")),//Low Heal & HoT
                Spell.Cast("Innervate", on => Helpers.LowestHealthPlayer, ret => Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth),//Low Heal & HoT
                new Decorator(when => !Global.IsInGroup,
                    new PrioritySelector
                    (
                        Spell.WaitForCast(),
                        new Decorator(ret => Helpers.LowestHealthPlayer.HealthPercent <= 80, new Action(delegate { return RunStatus.Failure; })),
                        Spell.Cast("Crushing Darkness"),
                        Spell.Cast("Shock"),
                        Spell.Cast("Force Lightning"),
                        Spell.Cast("Lightning Strike")
                    )),
                Movement.MoveTo(ret => Helpers.LowestHealthPlayer.Position, Global.RangeDist)
            );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererCorruption)]
        public static Composite SorcererCorruptionOutOfCombat()
        {
            return new Decorator(ret => Helpers.LowestHealthPlayer.HealthPercent <= 95,
                new PrioritySelector(
                    Spell.Cast("Force Speed", ret => BuddyTor.Me.IsMoving),
                    Spell.Cast("Resurgence", on => Helpers.LowestHealthPlayer, j => Helpers.LowestHealthPlayer.HealthPercent <= Global.HighHealth && !Helpers.LowestHealthPlayer.HasBuff("Resurgence")),//Low Heal & HoT
                    Spell.Cast("Innervate", on => Helpers.LowestHealthPlayer, k => Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth)//Low Heal 
                ));
        }

        private static Composite PreCombat()
        {
            foreach (var p in Helpers.Group)
            {
                new PrioritySelector(
                    Spell.Cast("Static Barrier", on => p, ret => !p.HasBuff("Static Barrier") && !p.HasDebuff("Deionized"))
                );
            }
            return SorcererCorruptionCombat();
        }


    }
}