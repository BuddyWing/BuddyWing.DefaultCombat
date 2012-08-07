using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Routines.Basic
{
    public class Knight
    {
        [Class(CharacterClass.Knight)]
        [Behavior(BehaviorType.Combat)]
        public static Composite KnightCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(Global.MeleeDist),

                //***Generel***
                Spell.Cast("Force Leap", ret => BuddyTor.Me.CurrentTarget.Distance >= 1f && BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
                Spell.Cast("Resolute", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD

                //**Defensive**
                Spell.BuffSelf("Saber Ward", castWhen => BuddyTor.Me.InCombat && BuddyTor.Me.HealthPercent <= 50),//3m CD

                //**Offensive**
                Spell.Cast("Blade Storm"),//-4 Focus
                Spell.Cast("Force Sweep", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= 1f && !o.IsDead) >= 3 && Helpers.Targets.Count() >= 3),//-3 Focus/15s CD
                Spell.Cast("Master Strike"),//0 Focus/27s CD
                Spell.WaitForCast(),
                Spell.Cast("Riposte"),//-3 Focus
                Spell.Cast("Slash"),//-3 Focus
                Spell.Cast("Strike"),//+2 Focus

                //Movement
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.MeleeDist)
                );
        }

        [Class(CharacterClass.Knight)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite KnightOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Buff("Shii-Cho Form", ret => BuddyTor.Me)
                );
        }

        [Class(CharacterClass.Knight)]
        [Behavior(BehaviorType.Pull)]
        public static Composite KnightPull()
        {
            return KnightCombat();
        }
    }
}