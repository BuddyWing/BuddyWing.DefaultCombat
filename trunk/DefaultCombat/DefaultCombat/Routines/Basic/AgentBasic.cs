using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.CommonBot.Logic;
using Buddy.Swtor;
using DefaultCombat.Dynamics;
using System.Diagnostics;

namespace DefaultCombat.Routines.Basic
{
    public class AgentBasic
    {
        private static readonly Stopwatch MovementTimer = new Stopwatch();
        private const int MovementThrottle = 2500;
        private static readonly Stopwatch CrouchTimer = new Stopwatch();
        private const int CrouchThrottle = 3000;

        private static bool CrouchNeeded
        {
            get
            {
                if (BuddyTor.Me.CurrentTarget == null)
                {
                    return false;
                }
                bool needed = false;
                if (!CrouchTimer.IsRunning || CrouchTimer.ElapsedMilliseconds > CrouchThrottle)
                {

                    if (!BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover") && 
                        BuddyTor.Me.CurrentTarget.Distance <= 2.8f && BuddyTor.Me.CurrentTarget.InLineOfSight)
                    {
                        BuddyTor.Me.CurrentTarget.Face();
                        needed = true;
                    }
                }
                CrouchTimer.Restart();
                Logger.Write("Is Crouch Needed? {0}", needed);
                return needed;
            }
        }

        [Class(CharacterClass.Agent)]
        [Behavior(BehaviorType.Combat)]
        public static Composite AgentCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                new Decorator(ctx => (!BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover")),
                Movement.StopInRange(2.8f)),
                //***General***
                Spell.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned),//CC Break
                Spell.Cast("Crouch", ret => BuddyTor.Me, ret => (!CrouchTimer.IsRunning || CrouchTimer.ElapsedMilliseconds > CrouchThrottle) && CrouchNeeded),


                //CC
                Spell.Cast("Flash Bang", onUnit =>
                {
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    return
                        Helpers.Targets.FirstOrDefault(
                            t =>
                            t != previousTarget && (t.Toughness == CombatToughness.Strong)) ??
                        Helpers.Targets.FirstOrDefault(t => t != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3),

                //Offensive
                Spell.Cast("Explosive Probe", castWhen => (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")) && BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Distance <= 2.8f),
                Spell.Cast("Fragmentation Grenade", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 80),//AoE
                Spell.Cast("Debilitate", castWhen => BuddyTor.Me.CurrentTarget.Distance <= 1f && BuddyTor.Me.ResourceStat >= 70),//Stun 
                Spell.Cast("Overload Shot", castWhen => BuddyTor.Me.ResourceStat >= 77 && BuddyTor.Me.CurrentTarget.Distance <= 1f),//Nice dmg shot
                Spell.Cast("Snipe", castWhen => (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")) && BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Distance <= 2.8f),
                Spell.Cast("Shiv", castWhen => BuddyTor.Me.CurrentTarget.Distance <= 0.4f && BuddyTor.Me.ResourceStat >= 75),//melee attack
                                Spell.Cast("Rifle Shot", castWhen => BuddyTor.Me.ResourceStat <= 60 ||
                    ((BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard ||
                    BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak) &&
                    BuddyTor.Me.CurrentTarget.HealthPercent <= 20)),//No Resource needed
                //Dot Arrow is missing

                                new Decorator(ctx => (!MovementTimer.IsRunning || MovementTimer.ElapsedMilliseconds > MovementThrottle),
                    new Sequence(
                        new Buddy.BehaviorTree.Action(ctx => MovementTimer.Restart()),
                        Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)))
                );
        }

        /*[Class(CharacterClass.Agent)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite AgentOutOfCombat()
        {
        }
         * */

        [Class(CharacterClass.Agent)]
        [Behavior(BehaviorType.Pull)]
        public static Composite AgentPull()
        {
             return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(2.8f),
                Spell.Cast("Rifle Shot"));

        }
    }
}
