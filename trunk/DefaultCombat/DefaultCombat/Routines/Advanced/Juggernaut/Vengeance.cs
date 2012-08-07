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
    public static class JuggernautVengeance
    {
        [Class(CharacterClass.Warrior, AdvancedClass.Juggernaut, SkillTreeId.JuggernautVengeance)]
        [Behavior(BehaviorType.Pull)]
        public static Composite JuggernautVengeancePull()
        {
            return JuggernautVengeanceCombat();
        }

        [Class(CharacterClass.Warrior, AdvancedClass.Juggernaut, SkillTreeId.JuggernautVengeance)]
        [Behavior(BehaviorType.Combat)]
        public static Composite JuggernautVengeanceCombat()
        {
            return new PrioritySelector(
              Movement.StopInRange(Global.MeleeDist),
              Spell.WaitForCast(),

              //***Generel***
              Spell.Cast("Saber Throw", castWhen => BuddyTor.Me.CurrentTarget.Distance >= 0.5f && BuddyTor.Me.CurrentTarget.Distance <= 3f),
              Spell.Cast("Force Charge", castWhen => BuddyTor.Me.CurrentTarget.Distance >= 1f && BuddyTor.Me.CurrentTarget.Distance <= 3f), //+3 Rage/15s CD/30m Range - Pull
              Spell.Cast("Force Push", castWhen => BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Player),
              Spell.Cast("Unleash", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
              Spell.Cast("Enrage", castWhen => BuddyTor.Me.ResourceStat <= 6),//+6 Rage

              //**CC**
              Spell.Cast("Disruption", castWhen => BuddyTor.Me.CurrentTarget.IsCasting && BuddyTor.Me.CurrentTarget.CastTimeEnd - TimeSpan.FromSeconds(1) >= DateTime.Now),
              Spell.Cast("Force Choke", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && !AbilityManager.CanCast("Disruption", BuddyTor.Me.CurrentTarget) || BuddyTor.Me.HealthPercent <= 60)) && !BuddyTor.Me.HasBuff("Saber Ward")),
              //Spell.Cast("Chilling Scream", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .3 && !o.IsDead) >= 3 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 3),
              Spell.Cast("Intimidating Roar", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .3 && !o.IsDead) >= 3 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 3 && BuddyTor.Me.HealthPercent <= 60),
              Spell.Cast("Backhand", castWhen => ((BuddyTor.Me.CurrentTarget.IsCasting && (!AbilityManager.CanCast("Force Choke", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Disruption", BuddyTor.Me.CurrentTarget)) || BuddyTor.Me.HealthPercent <= 30)) && !BuddyTor.Me.HasBuff("Saber Ward")),

              //**Defensive**
              Spell.BuffSelf("Endure Pain", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 30),
              Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50),//3m CD   
              //Spell.Cast("Intercede", castOn => Helpers.LowestHealthPlayer, castWhen => Helpers.LowestHealthPlayer.GroupId == BuddyTor.Me.GroupId && Helpers.LowestHealthPlayer.HealthPercent <= 50),

              //**Offensive**
              Spell.Cast("Vicious Throw", castWhen => BuddyTor.Me.CurrentTarget.HealthPercent <= 20 && BuddyTor.Me.ResourceStat >= 3),//-3 Rage
              Spell.Cast("Pommel Strike", castWhen => BuddyTor.Me.CurrentTarget.IsStunned && (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak)),//strong skill but only usable on stunned - normal or weak - enemys
              Spell.Cast("Savage Kick", castWhen => BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Player),
              new Decorator(ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Armor Reduced") || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Armor Reduced").Stacks < 5 || BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(b => b.Name == "Armor Reduced").TimeLeft.Seconds < 2 || BuddyTor.Me.ResourceStat <= 9,
                    new PrioritySelector(
                        Spell.Cast("Sundering Assault"))),
              Spell.Cast("Shatter", castWhen => BuddyTor.Me.CurrentTarget.HasDebuff("Sundering Assault")),
              Spell.Cast("Impale"),
              Spell.Cast("Force Scream", castWhen => 
              {
                  if (BuddyTor.Me.ResourceStat >= 4 || (AbilityManager.HasAbility("Battle Cry") && BuddyTor.Me.HasBuff("Battle Cry")))
                  {
                      if (AbilityManager.HasAbility("Savagery"))
                      {
                          if (BuddyTor.Me.HasBuff("Savagery"))
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
              Spell.Cast("Backhand", castWhen => BuddyTor.Me.ResourceStat >= 3 && !BuddyTor.Me.CurrentTarget.IsStunned),
              Spell.Cast("Smash", castWhen => (ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 2 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 2) || AbilityManager.HasAbility("Ruin")),//-3 Rage/15s CD
              Spell.Cast("Sweeping Slash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 0.3 && !o.IsDead) >= 4 && BuddyTor.Me.ResourceStat >= 3 && Helpers.Targets.Count() >= 4),//-3 Rage/15s CD
              Spell.Cast("Ravage"),
              Spell.Cast("Vicious Slash", castWhen => BuddyTor.Me.ResourceStat >= 4),
              Spell.Cast("Assault"),

              Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
              );
        }

        [Class(CharacterClass.Warrior, AdvancedClass.Juggernaut, SkillTreeId.JuggernautVengeance)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite JuggernautVengeanceOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Shien Form", ret => AbilityManager.HasAbility("Shien Form") && !BuddyTor.Me.HasBuff("Shien Form")),
                Spell.Cast("Shii-Cho Form", ret => !AbilityManager.HasAbility("Shien Form") && !BuddyTor.Me.HasBuff("Shii-Cho Form"))
             );
        }
    }
}
