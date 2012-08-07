using System;
using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Routines
{//by protpally
    public static class PowertechFirebug
    {
        public static bool Burn = false;
        public static bool Move = true;
        public static bool AllCombat = true;
        private static DateTime LastChecked { get; set; }
        [Class(CharacterClass.BountyHunter, AdvancedClass.Powertech, SkillTreeId.PowertechFirebug)]
        [Behavior(BehaviorType.Pull)]
        public static Composite PowertechAssaultPull()
        {
            return PowertechAssaultCombat();
            //return new PrioritySelector(
            // new Decorator(ret => BuddyTor.Me.IsMounted, new PrioritySelector()), --- still pulling if mounted
            // We need to sort our movement and line-of-sight stuff out before anything else.
            //Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f),
            //Movement.StopInRange(2.8f),
            //Spell.BuffSelf("Reserve Powercell", ret => BuddyTor.Me.Level > 41 && AbilityManager.HasAbility("Reserve Powercell")),
            //Spell.Cast("Plasma Grenade", ret => BuddyTor.Me.Level > 17 && AbilityManager.HasAbility("Plasma Grenade")),
            //Spell.Cast("Explosive Dart", ret => AbilityManager.HasAbility("Explosive Dart")),
            //Spell.Cast("Rapid Shots", ret => true)
            // );

        }
        public static bool ToggleState(bool sw, string log)
        {
            if (sw == true)
            {
                Logging.Write("Stoping " + log);
                sw = false;
                Logging.Write("State " + sw.ToString());
            }
            else
            {
                Logging.Write("Starting " + log);
                sw = true;
                Logging.Write("State " + sw.ToString());
            };
            return sw;
        }
        public static void StopRest()
        {
            if (BuddyTor.Me.HealthPercent >= 99 && BuddyTor.Me.ResourceStat >= 11 && BuddyTor.Me.IsCasting)
            {
                if (DateTime.Now.Subtract(LastChecked).TotalSeconds > 5)
                {
                    LastChecked = DateTime.Now;
                    Logging.Write("Fully Rested");
                    Buddy.Swtor.Movement.Move(Buddy.Swtor.MovementDirection.Forward, TimeSpan.FromMilliseconds(1));
                }
            }
        }
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.BountyHunter, AdvancedClass.Powertech, SkillTreeId.PowertechFirebug)]
        public static Composite PowertechAssaultCombat()
        {
            return new PrioritySelector(
                new Decorator(ret => !AllCombat,
                    new Sequence(
                        new Action(ret => Logging.Write("Combat Disabled!"))
                    )
                ),
                new Decorator(ret => AllCombat,
                    new PrioritySelector(
                        #region AllCombat
                        // We need to sort our movement and line-of-sight stuff out before anything else.
                        new Decorator(ret => Move,
                            new PrioritySelector(
                            Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, .5f),
                            Movement.StopInRange(.5f))
                        ),
                        #region BurnPhase
                        new Decorator(ret => BuddyTor.Me.ResourceStat > 80,
                            new Action(ret => Burn = false)
                        ),
                        new Decorator(ret => Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"),
                            new PrioritySelector(
                                Spell.WaitForCast(),
                                new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance > .5f && !Move,
                                    new PrioritySelector(
                                        Spell.Cast("Grapple", ret => !BuddyTor.Me.IsMoving),
                                        Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, .5f)
                                    )
                                ),
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) == null),
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name).TimeLeft.Seconds < 4),
                                Spell.WaitForCast(),
                                Spell.Cast("Rocket Punch", ret => BuddyTor.Me.CurrentTarget.Distance <= .5f),
                                Spell.Cast("Rail Shot", ret => BuddyTor.Me.HasBuff("Prototype Particle Accelerator")),
                                Spell.Cast("Rail Shot", ret => true),
                                Spell.Cast("Unload", ret => true),   
                                Spell.Cast("Flame Burst", ret => true)
                            )
                        ),
                        #endregion BurnPhase
                        #region Lazyraiderish stuff - works with combat movement off
                        new Decorator(ret => BuddyTor.Me.IsMoving,
                            new PrioritySelector(
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) == null),
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.ResourceStat <= 24 && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name).TimeLeft.Seconds < 4),
                                Spell.BuffSelf("Energy Shield", ret => BuddyTor.Me.HealthPercent < 40),
                                Spell.Cast("Kolto Overload", ret => BuddyTor.Me.HealthPercent <= 60),
                                Spell.Cast("Rapid Shots", ret => BuddyTor.Me.ResourceStat > 30 && !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                                Spell.Cast("Jet Boost", ret => BuddyTor.Me.ResourceStat <= 24 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                                Spell.Cast("Rail Shot", ret => true),
                                Spell.Cast("Explosive Dart", ret => true),
                                Spell.Cast("Rapid Shots", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                            )
                        ),
                        #endregion Lazyraiderish stuff - works with combat movement off
                        new Decorator(ret => !Burn || !BuddyTor.Me.HasBuff("Critical Boost") || !BuddyTor.Me.HasBuff("Power Boost"),
                            new PrioritySelector(
                                Spell.Cast("Vent Heat", ret => BuddyTor.Me.ResourceStat >= 50),
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) == null),
                                Spell.Cast("Incendiary Missile", ret => BuddyTor.Me.ResourceStat <= 24 && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Burning (Tech)" && B.Duration == TimeSpan.FromSeconds(18) && B.Caster.Name == BuddyTor.Me.Name).TimeLeft.Seconds < 4),
                                Spell.WaitForCast(),
                                Spell.Cast("Determination", ret => BuddyTor.Me.IsStunned),
                                Spell.BuffSelf("Energy Shield", ret => BuddyTor.Me.HealthPercent < 40),
                                Spell.Cast("Kolto Overload", ret => BuddyTor.Me.HealthPercent <= 60),
                                Spell.Cast("Rail Shot", ret => BuddyTor.Me.HasBuff("Prototype Particle Accelerator")),
                                Spell.Cast("Rapid Shots", ret => BuddyTor.Me.ResourceStat > 30),
                                new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance > .5f,
                                    new PrioritySelector(
                                        Spell.Cast("Grapple", ret => !BuddyTor.Me.IsMoving),
                                        Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, .5f)
                                    )
                                ),
                                Spell.Cast("Jet Boost", ret => BuddyTor.Me.ResourceStat <= 24 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                                Spell.Cast("Rocket Punch", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= .8f),
                                #region PreIncendiaryRound-Low LVL Support
                                new Decorator(ret => !AbilityManager.HasAbility("Incendiary Missile"),
                                    new PrioritySelector(
                                        Spell.Cast("Explosive Dart", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 10),
                                        Spell.Cast("Rail Shot", ret => true),
                                        Spell.Cast("Missile Blast", ret => BuddyTor.Me.ResourceStat >= 10),
                                        Spell.Cast("Rapid Shots", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                                    )
                                ),
                                #endregion PreIncendiaryRound-Low LVL Support
                                Spell.Cast("Rail Shot", ret => true),
                                Spell.Cast("Rocket Punch", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= .5f),
                                Spell.Cast("Unload", ret => BuddyTor.Me.ResourceStat >= 10),
                                Spell.Cast("Flame Burst", ret => BuddyTor.Me.ResourceStat >= 10),
                                Spell.Cast("Vent Heat", ret => BuddyTor.Me.ResourceStat > 80),
                                Spell.Cast("Rapid Shots", ret => true),
                                new Decorator(ret => Move,
                                    Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, .5f)
                                )
                            )
                        )
                        #endregion AllCombat
                    )
                )
            );
        }

        [Class(CharacterClass.BountyHunter, AdvancedClass.Powertech, SkillTreeId.PowertechFirebug)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite PowertechAssaultOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Recharge and Reload", ret => BuddyTor.Me.ResourceStat >= 50 || BuddyTor.Me.HealthPercent < 100),
                Spell.BuffSelf("Combustible Gas Cylinder"),
                new Action(ret => StopRest()),
                Spell.WaitForCast()
                );
        }
    }
}
