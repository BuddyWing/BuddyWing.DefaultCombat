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
    //Neo93 07.05.2012
    public static class GuardianDefense
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianDefense)]
        public static Composite GuardianDefensePull()
        {
            return GuardianDefenseCombat();
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianDefense)]
        public static Composite GuardianDefenseCombat()
        {
            return new PrioritySelector(
              Movement.StopInRange(0.4f),
              Spell.WaitForCast(),

              //***Generel***
              Spell.Cast("Saber Throw", castWhen => BuddyTor.Me.CurrentTarget.Distance >= 0.5f && BuddyTor.Me.CurrentTarget.Distance <= 3f),
              Spell.Cast("Force Leap", castWhen => BuddyTor.Me.CurrentTarget.Distance >= 1f && BuddyTor.Me.CurrentTarget.Distance <= 3f), //+3 Rage/15s CD/30m Range - Pull
              Spell.Cast("Force Push", castWhen => BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Player),
              Spell.Cast("Resolute", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
              Spell.Cast("Combat Focus", castWhen => BuddyTor.Me.ResourceStat <= 6),//+6 Rage

              //**CC**
              Spell.Cast("Force Kick", castWhen => BuddyTor.Me.CurrentTarget.IsCasting && BuddyTor.Me.CurrentTarget.CastTimeEnd - TimeSpan.FromSeconds(1) >= DateTime.Now),
              Spell.Cast("Force Stasis", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && !AbilityManager.CanCast("Force Kick", BuddyTor.Me.CurrentTarget) || BuddyTor.Me.HealthPercent <= 60)) && !BuddyTor.Me.HasBuff("Saber Ward")),
                //Spell.Cast("Freezing Force", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .3 && !o.IsDead) >= 3 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 3),
              Spell.Cast("Awe", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .3 && !o.IsDead) >= 3 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 3 && BuddyTor.Me.HealthPercent <= 60),
              Spell.Cast("Hilt Strike", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && (!AbilityManager.CanCast("Force Stasis", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Force Kick", BuddyTor.Me.CurrentTarget)) || BuddyTor.Me.HealthPercent <= 30)) && !BuddyTor.Me.HasBuff("Saber Ward")),   

              //**Defensive**
              Spell.BuffSelf("Warding Call", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 60),//3m CD
              Spell.BuffSelf("Enure", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 30),
              Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50),//3m CD   
              //Spell.Cast("Guardian Leap", castOn => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.GroupId == BuddyTor.Me.GroupId && Helpers.LowestHealthPlayer.HealthPercent <= 50),

              //**Offensive**
              Spell.Cast("Dispatch", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 20 && BuddyTor.Me.ResourceStat >= 3),//-3 Rage
              Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
              Spell.Cast("Opportune Strike", castWhen => BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Player),
              Spell.Cast("Guardian Slash", castWhen => BuddyTor.Me.ResourceStat >= 4),
              new Decorator(ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Armor Reduced") || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Armor Reduced").Stacks < 5 || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(b => b.Name == "Armor Reduced").TimeLeft.Seconds < 2 || BuddyTor.Me.ResourceStat <= 9,
                    new PrioritySelector(
                        Spell.Cast("Sundering Strike"))),
              Spell.Cast("Blade Storm"),
              Spell.Cast("Hilt Strike", castWhen => BuddyTor.Me.ResourceStat >= 3 && !BuddyTor.Me.CurrentTarget.IsStunned),
              Spell.Cast("Force Sweep", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 2 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 2),//-3 Rage/15s CD
              Spell.Cast("Cyclone Slash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 4 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 4),//-3 Rage/15s CD
              Spell.Cast("Master Strike"),
              Spell.Cast("Slash", castWhen => BuddyTor.Me.ResourceStat >= 6),
              Spell.Cast("Strike"),
              
              Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 0.4f)
              );
        }

        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianDefense)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite GuardianDefenseOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Soresu Form", ret => AbilityManager.HasAbility("Soresu Form") && !BuddyTor.Me.HasBuff("Soresu Form")),
                Spell.Cast("Shii-Cho Form", ret => !AbilityManager.HasAbility("Soresu Form") && !BuddyTor.Me.HasBuff("Shii-Cho Form"))
             );
        }
    }
}
