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
    public static class SentinelWatchman
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Knight, AdvancedClass.Sentinel, SkillTreeId.SentinelWatchman)]
        public static Composite SentinelWatchmanPull()
        {
            return SentinelWatchmanCombat();
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Knight, AdvancedClass.Sentinel, SkillTreeId.SentinelWatchman)]
        public static Composite SentinelWatchmanCombat()
        {
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                Spell.Cast("Force Leap", ret => BuddyTor.Me.CurrentTarget.Distance >= 0.5f && BuddyTor.Me.CurrentTarget.Distance <= 3f && !BuddyTor.Me.CurrentTarget.IsCoverAffected), //+3 Rage/15s CD/30m Range - Pull
                Spell.Cast("Overload Saber", castWhen => !BuddyTor.Me.HasBuff("Overload Saber")),
                Spell.Cast("Force Kick", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
                Spell.Cast("Valorous Call", castWhen => !BuddyTor.Me.HasBuff("Centering")),

                //#Only 1 of these 3 buffs can be active at a time#
                Spell.Cast("Inspiration", castWhen => BuddyTor.Me.HasBuff("Centering")),//5m CD
                Spell.Cast("Transcendence", castWhen => BuddyTor.Me.HasBuff("Centering") && BuddyTor.Me.HealthPercent <= 50 || (BuddyTor.Me.CurrentTarget.Distance >= 2f && !AbilityManager.CanCast("Force Leap", BuddyTor.Me.CurrentTarget))),
                Spell.Cast("Zen", castWhen => BuddyTor.Me.HasBuff("Centering")),
                //#
                Spell.Cast("Pacify", castwhen => BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Player || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Strong),

                //**Defensive**
                Spell.BuffSelf("Rebuke", castWhen => BuddyTor.Me.HealthPercent <= Global.HighHealth),//1m CD
                Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.HealthPercent <= Global.MidHealth),//3m CD
                Spell.BuffSelf("Force Camouflage", castWhen => BuddyTor.Me.HealthPercent <= Global.LowHealth),//45s CD
                Spell.BuffSelf("Guarded by the Force", castWhen => BuddyTor.Me.HealthPercent <= Global.OhShit),//1m15s CD

                //**Offensive**
                Spell.Cast("Dispatch", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 30),//-3 Rage
                Spell.Cast("Merciless Slash"),
                Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
                Spell.Cast("Zealous Strike", castWhen => BuddyTor.Me.ResourceStat <= 6),//+6 Rage/12s CD
                Spell.Cast("Force Sweep", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),
                Spell.Cast("Sweeping Slash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),

                //**CC**
                Spell.Cast("Force Kick", castWhen => BuddyTor.Me.CurrentTarget.IsCasting && BuddyTor.Me.CurrentTarget.CastTimeEnd - TimeSpan.FromSeconds(1) >= DateTime.Now),
                Spell.Cast("Force Stasis", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && !AbilityManager.CanCast("Force Kick", BuddyTor.Me.CurrentTarget) || BuddyTor.Me.HealthPercent <= 60)) && !BuddyTor.Me.HasBuff("Saber Ward")),
                Spell.Cast("Awe", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= Global.MeleeDist && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),

                //*Watchman*
                Spell.Cast("Cauterize", castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Bleeding") && BuddyTor.Me.CurrentTarget.HealthPercent >= 50),
                Spell.Cast("Master Strike"),//30s CD
                Spell.Cast("Slash", castWhen => (BuddyTor.Me.ResourceStat >= 7 && AbilityManager.HasAbility("Annihilate")) || (BuddyTor.Me.ResourceStat >= 3 && !AbilityManager.HasAbility("Annihilate"))),
                Spell.Cast("Strike"),//+2 Rage

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Knight, AdvancedClass.Sentinel, SkillTreeId.SentinelWatchman)]
        public static Composite SentinelWatchmanOutOfCombat()
        {
            return new PrioritySelector(
                    Spell.Cast("Juyo Form", ret => AbilityManager.HasAbility("Juyo Form") && !BuddyTor.Me.HasBuff("Juyo Form")),
                    Spell.Cast("Shii-Cho Form", ret => !AbilityManager.HasAbility("Juyo Form") && !BuddyTor.Me.HasBuff("Shii-Cho Form"))
                    );
        }
    }
}
