using System;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using System.Linq;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Basic
{
    public class BountyHunter
    {
        [Class(CharacterClass.BountyHunter)]
        [Behavior(BehaviorType.Combat)]
        public static Composite BountyHunterCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(2.8f),

                //***Generel***
                Spell.Cast("Determination", castWhen => BuddyTor.Me.IsStunned),//Insignia
                Spell.Cast("Vent Heat", ret => BuddyTor.Me, ret => BuddyTor.Me.ResourceStat > 39),//-50 Heat

                //**Offensive**
                //Spell.CastOnGround("Death from Above",castWhen => BuddyTor.Me.ResourceStat <= 23, BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Rail Shot", castWhen => BuddyTor.Me.CurrentTarget.IsStunned),
                Spell.Cast("Explosive Dart", castWhen => Helpers.Targets.Count() >= 2 && BuddyTor.Me.ResourceStat <= 23),
                Spell.Cast("Unload", castWhen => BuddyTor.Me.ResourceStat <= 23),//+16 Heat/15s CD
                Spell.Cast("Rocket Punch", castWhen => BuddyTor.Me.ResourceStat <= 23),//Melee
                Spell.Cast("Missile Blast", castWhen => BuddyTor.Me.ResourceStat <= 23),//+25 Heat
                Spell.Cast("Flame Thrower", castWhen => BuddyTor.Me.ResourceStat <= 23 && Helpers.Targets.Count(t => t.Distance <= 1) >= 3),
                Spell.Cast("Rapid Shots"),

                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
                );
        }

        [Class(CharacterClass.BountyHunter)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite BountyHunterOutOfCombat()
        {
            return new PrioritySelector(
                Spell.BuffSelf("Combustible Gas Cylinder")//Weapon Buff
                );
        }


        [Class(CharacterClass.BountyHunter)]
        [Behavior(BehaviorType.Pull)]
        public static Composite BountyHunterPull()
        {
            return BountyHunterCombat();
        }
    }
}


