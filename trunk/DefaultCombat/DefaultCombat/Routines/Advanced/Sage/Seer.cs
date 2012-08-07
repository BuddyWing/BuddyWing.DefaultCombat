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
    public static class SageSeer
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageSeer)]
        public static Composite SageSeerPull()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.IsInGroup, new Action(ret => RunStatus.Success)),
                    new Decorator(ret => !Global.IsInGroup, SageSeerCombat())
                );
        }
        
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageSeer)]
        public static Composite SageSeerCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.RangeDist),
                //Generel
                Spell.Cast("Force of Will", castWhen => BuddyTor.Me.IsStunned),
                Spell.Cast("Force Potency", castWhen => Helpers.Tank.HealthPercent <= Global.HighHealth),//Buff for 2 spells

                //Spell.Cast("Restoration", on => Helpers.DebuffTarget, castWhen => Helpers.DebuffTarget.Debuffs.Count() >= 2),//Cleanse (2 debuffs)
                Spell.Cast("Force Armor", on => Helpers.Tank, castWhen => !Helpers.Tank.HasBuff("Force Armor") && !Helpers.Tank.HasDebuff("Force-imbalance")),//Shield
                Spell.Cast("Rejuvenate", on => Helpers.Tank, castWhen => (AbilityManager.HasAbility("Force Shelter") && !Helpers.Tank.HasBuff("Force Shelter"))),//HoT & Armorbuff
                Spell.Cast("Noble Sacrifice", castWhen => (BuddyTor.Me.HasBuff("Resplendence") || !BuddyTor.Me.HasDebuff("Noble Sacrifice")) /*BuddyTor.Me.Debuffs.FirstOrDefault(d => d.Name == "Noble Sacrifice").Stacks < 2)*/ && (BuddyTor.Me.HealthPercent >= 90 || (BuddyTor.Me.EnergyMax - BuddyTor.Me.Energy) >= 50)),//Restores Force
                Spell.CastOnGround("Salvation", castWhen => Helpers.Group.Where(p => p.HealthPercent <= Global.HighHealth && p.Distance <= 0.4f).Count() >= 2 && BuddyTor.Me.HasBuff("Conveyance"), on => Helpers.Group.FirstOrDefault(p => p.HealthPercent <= Global.HighHealth).Position),//AoE Heal
                Spell.Cast("Force Armor", on => Helpers.LowestHealthPlayer, castWhen => !Helpers.LowestHealthPlayer.HasBuff("Force Armor") && !Helpers.LowestHealthPlayer.HasDebuff("Force-imbalance") && Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth),//Shield
                Spell.Cast("Benevolence", on => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.HealthPercent <= Global.OhShit),//mid heal - low casttime
                Spell.Cast("Deliverance", on => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.HealthPercent <= Global.LowHealth),
                Spell.Cast("Rejuvenate", on => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.HealthPercent <= Global.HighHealth && !Helpers.LowestHealthPlayer.HasBuff("Rejuvenate")),//Low Heal & HoT
                Spell.Cast("Healing Trance", on => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth),//Low Heal 
                new Decorator(when => !Global.IsInGroup,
                    new PrioritySelector
                    (
                        Spell.WaitForCast(),
                        new Decorator(ret => Helpers.LowestHealthPlayer.HealthPercent <= 80, new Action(delegate { return RunStatus.Failure; })),
                        Spell.Cast("Mind Crush"),
                        Spell.Cast("Project"),
                        Spell.Cast("Telekinetic Throw"),
                        Spell.Cast("Disturbance")
                    )),
                Movement.MoveTo(ret => Helpers.LowestHealthPlayer.Position, Global.RangeDist)
            );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageSeer)]
        public static Composite SageSeerOutOfCombat()
        {
            return new Decorator(ret => Helpers.LowestHealthPlayer.HealthPercent <= 95,
                new PrioritySelector(
                    Spell.Cast("Force Speed", ret => BuddyTor.Me.IsMoving),
                    Spell.Cast("Rejuvenate", on => Helpers.LowestHealthPlayer, j => Helpers.LowestHealthPlayer.HealthPercent <= Global.HighHealth && !Helpers.LowestHealthPlayer.HasBuff("Rejuvenate")),//Low Heal & HoT
                    Spell.Cast("Healing Trance", on => Helpers.LowestHealthPlayer, k => Helpers.LowestHealthPlayer.HealthPercent <= Global.MidHealth)//Low Heal 
                ));
        }

        private static Composite PreCombat()
        {
            foreach (var p in Helpers.Group)
            {
                new PrioritySelector(
                    Spell.Cast("Force Armor", on => p, ret => !p.HasBuff("Force Armor") && !p.HasDebuff("Force-imbalance"))
                );
            }
            return SageSeerPull();
        }
    }
}