using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Basic
{
    public class Warrior
    {
        [Class(CharacterClass.Warrior)]
        [Behavior(BehaviorType.Pull)]
        public static Composite WarriorPull()
        {
            return WarriorCombat();
        }

        [Class(CharacterClass.Warrior)]
        [Behavior(BehaviorType.Combat)]
        public static Composite WarriorCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                Spell.Cast("Force Charge", ret => BuddyTor.Me.CurrentTarget.Distance >= 1f && BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist), //+3 Rage/15s CD/30m Range - Pull
                Spell.Cast("Unleash", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD

                //**Defensive**
                Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50),//3m CD

                //**Offensive**
                Spell.Cast("Force Scream"),//-4Rage/2s CD
                Spell.Cast("Smash", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 1f && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),//-3 Rage/15s CD
                Spell.Cast("Ravage"),//0 Rage/27s CD
                Spell.Cast("Retaliation"),//-3 Rage
                Spell.Cast("Vicious Slash"),//-3 Rage
                Spell.Cast("Assault"),//+2 Rage

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Class(CharacterClass.Warrior)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite WarriorOutOfCombat()
        {
            return new PrioritySelector(
                Spell.BuffSelf("Shii-Cho Form")
                );
        }
    }
}