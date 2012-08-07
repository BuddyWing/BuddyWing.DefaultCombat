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
    public static class MarauderCarnage
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderCarnage)]
        public static Composite MarauderCarnagePull()
        {
            return MarauderCarnageCombat();
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderCarnage)]
        public static Composite MarauderCarnageCombat()
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
                Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
                Spell.Cast("Battering Assault", castWhen => BuddyTor.Me.ResourceStat <= 6),//+6 Rage/12s CD
                Spell.Cast("Smash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),
                Spell.Cast("Sweeping Slash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),

                //**CC**
                Spell.Cast("Disruption", castWhen => BuddyTor.Me.CurrentTarget.IsCasting && BuddyTor.Me.CurrentTarget.CastTimeEnd - TimeSpan.FromSeconds(1) >= DateTime.Now),
                Spell.Cast("Force Choke", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && !AbilityManager.CanCast("Disruption", BuddyTor.Me.CurrentTarget) || BuddyTor.Me.HealthPercent <= 60)) && !BuddyTor.Me.HasBuff("Saber Ward")),
                Spell.Cast("Intimidating Roar", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),

                //*Carnage*
                Spell.Cast("Gore", castWhen => BuddyTor.Me.ResourceStat >= 8 || (BuddyTor.Me.ResourceStat >= 3 && AbilityManager.CanCast("Ravage", BuddyTor.Me.CurrentTarget))),//-3Rage/15s CD
                Spell.Cast("Ravage"),//27s CD
                Spell.Cast("Force Scream", castWhen =>
                {
                    Logger.Write("Health" + BuddyTor.Me.CurrentTarget.HealthPercent);
                    if (AbilityManager.HasAbility("Sever"))//Reduced rage costs
                    {
                        if (AbilityManager.HasAbility("Towering Rage"))//50/100% Crit
                        {
                            if (BuddyTor.Me.HasBuff("Blood Frenzy") && BuddyTor.Me.ResourceStat >= 2)
                            {
                                return true;                
                            }
                            else 
                                return false;
                        }
                        else
                        {
                            if (BuddyTor.Me.ResourceStat >= 2)
                            {
                                return true;
                            }
                            else
                                return false;  
                        }
                    }
                    else
                    {
                        if (BuddyTor.Me.ResourceStat >= 4)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                }),//-2Rage/9s CD
                Spell.Cast("Massacre"),//-3Rage/15s CD
                Spell.Cast("Vicious Slash", castWhen => !AbilityManager.HasAbility("Massacre")),
                Spell.Cast("Rupture", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Bleeding") && BuddyTor.Me.CurrentTarget.HealthPercent >= 50),//-2Rage

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Warrior, AdvancedClass.Marauder, SkillTreeId.MarauderCarnage)]
        public static Composite MarauderCarnageOutOfCombat()
        {
            return new PrioritySelector(
                    Spell.Cast("Ataru Form", ret => AbilityManager.HasAbility("Ataru Form") && !BuddyTor.Me.HasBuff("Ataru Form")),
                    Spell.Cast("Shii-Cho Form", ret => !AbilityManager.HasAbility("Ataru Form") && !BuddyTor.Me.HasBuff("Shii-Cho Form"))
                    );
        }
    }
}
