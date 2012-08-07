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
    public static class GuardianVigilance
    {
        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianVigilance)]
        [Behavior(BehaviorType.Pull)]
        public static Composite GuardianVigilancePull()
        {
            return GuardianVigilanceCombat();
        }

        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianVigilance)]
        [Behavior(BehaviorType.Combat)]
        public static Composite GuardianVigilanceCombat()
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
              Spell.Cast("Hilt Strike", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && (!AbilityManager.CanCast("Hilt Strike", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Force Kick", BuddyTor.Me.CurrentTarget)) || BuddyTor.Me.HealthPercent <= 30)) && !BuddyTor.Me.HasBuff("Saber Ward")),

              //**Defensive**
              Spell.BuffSelf("Enure", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 30),
              Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50),//3m CD   
              //Spell.Cast("Guardian Leap", castOn => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.GroupId == BuddyTor.Me.GroupId && Helpers.LowestHealthPlayer.HealthPercent <= 50),

              //**Offensive**
              new Decorator(ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Armor Reduced") || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Armor Reduced").Stacks < 5 || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(b => b.Name == "Armor Reduced").TimeLeft.Seconds < 2 || BuddyTor.Me.ResourceStat <= 9,
                    new PrioritySelector(
                        Spell.Cast("Sundering Strike"))),
              Spell.Cast("Strike",castWhen => BuddyTor.Me.ResourceStat <= 3),
              Spell.Cast("Dispatch", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 20 && BuddyTor.Me.ResourceStat >= 3),//-3 Rage
              Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
              Spell.Cast("Opportune Strike", castWhen => BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Player),
              Spell.Cast("Plasma Brand", castWhen => BuddyTor.Me.ResourceStat >= 5 && BuddyTor.Me.CurrentTarget.HasDebuff("Sundering Strike")),
              Spell.Cast("Overhead Slash", castWhen => BuddyTor.Me.ResourceStat >= 4),
              Spell.Cast("Blade Storm", castWhen => 
              {                  
                  if (BuddyTor.Me.ResourceStat >= 4 || (AbilityManager.HasAbility("Momentum") && BuddyTor.Me.HasBuff("Momentum")))
                  {
                      if (AbilityManager.HasAbility("Force Rush"))
                      {
                          if (BuddyTor.Me.HasBuff("Force Rush"))
                          {
                              return true;
                          }
                          else
                              return false;
                      }
                      else
                          return true;
                  }
                  else
                      return false;
              }),
              Spell.Cast("Hilt Strike", castWhen => BuddyTor.Me.ResourceStat >= 3 && !BuddyTor.Me.CurrentTarget.IsStunned),
              Spell.Cast("Force Sweep", castWhen => (ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 3 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 3) || AbilityManager.HasAbility("Effluence")),//-3 Rage/15s CD
              Spell.Cast("Cyclone Slash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 4 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 4),//-3 Rage/15s CD
              Spell.Cast("Master Strike"),
              Spell.Cast("Slash", castWhen => BuddyTor.Me.ResourceStat >= 4),

              //Movement
               Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 0.4f)
              );
        }

        [Class(CharacterClass.Knight, AdvancedClass.Guardian, SkillTreeId.GuardianVigilance)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite GuardianVigilanceOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Shien Form", ret => AbilityManager.HasAbility("Shien Form") && !BuddyTor.Me.HasBuff("Shien Form")),
                Spell.Cast("Shii-Cho Form", ret => !AbilityManager.HasAbility("Shien Form") && !BuddyTor.Me.HasBuff("Shii-Cho Form"))
             );
        }
    }
}
