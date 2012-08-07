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
    //Neo93
    public static class MarauderRage
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderRage)]
        public static Composite MarauderRagePull()
        {
            return MarauderRageCombat();
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderRage)]
        public static Composite MarauderRageCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                Spell.Cast("Force Charge", ret => BuddyTor.Me.CurrentTarget.Distance >= 0.5f && BuddyTor.Me.CurrentTarget.Distance <= 3f && !BuddyTor.Me.CurrentTarget.IsCoverAffected), //+3 Rage/15s CD/30m Range - Pull
                Spell.Cast("Unleash", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
                Spell.Cast("Frenzy", castWhen => !BuddyTor.Me.HasBuff("Fury")),

                //#Only 1 of these 3 buffs can be active at a time#
                Spell.Cast("Bloodthirst", castWhen => BuddyTor.Me.HasBuff("Fury")),//5m CD
                Spell.Cast("Predation", castWhen => BuddyTor.Me.HasBuff("Fury") && BuddyTor.Me.HealthPercent <= 50 || (BuddyTor.Me.CurrentTarget.Distance >= 2f && !AbilityManager.CanCast("Force Charge", BuddyTor.Me.CurrentTarget))),
                Spell.Cast("Berserk", castWhen => BuddyTor.Me.HasBuff("Fury")),
                //#
                Spell.Cast("Obfuscate", castwhen => BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Player || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Strong),

                //**Defensive**
                Spell.BuffSelf("Cloak of Pain", castWhen => BuddyTor.Me.HealthPercent <= Global.HighHealth),//1m CD
                Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.HealthPercent <= Global.MidHealth),//3m CD
                Spell.BuffSelf("Force Camouflage", castWhen => BuddyTor.Me.HealthPercent <= Global.LowHealth),//45s CD
                Spell.BuffSelf("Undying Rage", castWhen => BuddyTor.Me.HealthPercent <= Global.OhShit),//1m15s CD

                //**Offensive**
                Spell.Cast("Vicious Throw", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),//-3 Rage
                Spell.Cast("Annihilate"),
                Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
                Spell.Cast("Battering Assault", castWhen => BuddyTor.Me.ResourceStat <= 6),//+6 Rage/12s CD

                //**CC**
                Spell.Cast("Disruption", castWhen => BuddyTor.Me.CurrentTarget.IsCasting && BuddyTor.Me.CurrentTarget.CastTimeEnd - TimeSpan.FromSeconds(1) >= DateTime.Now),
                Spell.Cast("Force Choke", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && !AbilityManager.CanCast("Disruption", BuddyTor.Me.CurrentTarget) || BuddyTor.Me.HealthPercent <= 60)) && !BuddyTor.Me.HasBuff("Saber Ward") || BuddyTor.Me.Buffs.FirstOrDefault(b => b.Name == "Shockwave").Stacks < 4),
                Spell.Cast("Intimidating Roar", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),

                //*Rage*
                Spell.Cast("Force Crush", castWhen => BuddyTor.Me.Buffs.FirstOrDefault(b => b.Name == "Shockwave").Stacks < 4),
                Spell.Cast("Smash", castWhen => BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist && (AbilityManager.HasAbility("Dominate") && BuddyTor.Me.HasBuff("Dominate")) || (BuddyTor.Me.HasBuff("Shockwave") && BuddyTor.Me.Buffs.FirstOrDefault(b => b.Name == "Shockwave").Stacks == 4) || !AbilityManager.HasAbility("Dominate")),  
                Spell.Cast("Obliterate", castWhen => BuddyTor.Me.CurrentTarget.Distance < 1f),
                Spell.Cast("Force Scream"),//-2Rage/9s CD
                Spell.Cast("Vicious Slash"),
                Spell.Cast("Assault"),

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
            );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderRage)]
        public static Composite MarauderRageOutOfCombat()
        {
            return new PrioritySelector(
                    Spell.Cast("Shii-Cho Form", ret => !BuddyTor.Me.HasBuff("Shii-Cho Form"))
            );
        }
    }
}
