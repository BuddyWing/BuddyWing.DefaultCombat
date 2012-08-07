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
    public static class MercenaryArsenal
    {

        public static bool Burn = false;
        public static bool Move = true;
        public static bool AllCombat = true;
        private static DateTime LastChecked { get; set; }
        [Class(CharacterClass.BountyHunter, AdvancedClass.Mercenary, SkillTreeId.MercenaryArsenal)]
        [Behavior(BehaviorType.Pull)]
        public static Composite MercenaryArsenalPull()
        {
            return MercenaryArsenalCombat();
            //new PrioritySelector(
                // new Decorator(ret => BuddyTor.Me.IsMounted, new PrioritySelector()), --- still pulling if mounted
                // We need to sort our movement and line-of-sight stuff out before anything else.
              //  Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f),
               // Movement.StopInRange(2.8f),
               // Spell.BuffSelf("Reserve Powercell", ret => BuddyTor.Me.Level > 41 && AbilityManager.HasAbility("Reserve Powercell")),
               // Spell.Cast("Plasma Grenade", ret => BuddyTor.Me.Level > 17 && AbilityManager.HasAbility("Plasma Grenade")),
               // Spell.Cast("Explosive Dart", ret => AbilityManager.HasAbility("Explosive Dart")),
               // Spell.Cast("Rapid Shots", ret => true)
               // );
        }
        public static bool ToggleState(bool sw,string log)
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
        [Class(CharacterClass.BountyHunter, AdvancedClass.Mercenary, SkillTreeId.MercenaryArsenal)]
        public static Composite MercenaryArsenalCombat()
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
                    Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f),
                    Movement.StopInRange(2.8f))
                ),
            #region BurnPhase
                new Decorator(ret => BuddyTor.Me.ResourceStat > 80,
                        new Action(ret => Burn = false)
                ),
                new Decorator(ret => Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"),
                    new PrioritySelector(
                        Spell.WaitForCast(),
                //new Action(ret => BuddyTor.Me.InventoryEquipped.FirstOrDefault(item => item.IsUsable).Use()),
                        Spell.Cast("Unload", ret => BuddyTor.Me.HasBuff("Barrage")),
                        Spell.Cast("Rail Shot", ret => BuddyTor.Me.HasBuff("Tracer Lock") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Tracer Lock").Stacks > 4),
                        Spell.Cast("Heatseeker Missiles", ret => true),
                        Spell.Cast("Rocket Punch", ret => BuddyTor.Me.CurrentTarget.Distance <= .8f),
                        Spell.Cast("Tracer Missile", ret => true),
                        Spell.Cast("Power Shot", ret => AbilityManager.CanCast("Power Shot", BuddyTor.Me.CurrentTarget)),
                        Spell.Cast("Missile Blast", ret => true)
                    )
                ),
            #endregion
            #region Lazyraiderish stuff - works with combat movement off
            new Decorator(ret => BuddyTor.Me.IsMoving,
                    new PrioritySelector(
                        Spell.BuffSelf("Energy Shield", ret => BuddyTor.Me.HealthPercent < 40),
                        Spell.Cast("Kolto Overload", ret => BuddyTor.Me.HealthPercent <= 60),
                        Spell.Cast("Rapid Shots", ret => BuddyTor.Me.ResourceStat > 30 && !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                        Spell.Cast("Jet Boost", ret => BuddyTor.Me.ResourceStat >= 10 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                        Spell.Cast("Heatseeker Missiles", ret => BuddyTor.Me.ResourceStat >= 10 || (Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                        Spell.Cast("Rail Shot", ret => true),
                        Spell.Cast("Explosive Dart", ret => true),
                        Spell.Cast("Rapid Shots", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                    )
                ),
            #endregion
            new Decorator(ret => !Burn || !BuddyTor.Me.HasBuff("Critical Boost") || !BuddyTor.Me.HasBuff("Power Boost"),
                new PrioritySelector(Spell.Cast("Vent Heat", ret => BuddyTor.Me.ResourceStat >= 50),
                Spell.Cast("Tracer Missile", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name).TimeLeft.Seconds < 6),
                Spell.WaitForCast(),
                Spell.Cast("Determination", ret => BuddyTor.Me.IsStunned),
                Spell.BuffSelf("Energy Shield", ret => BuddyTor.Me.HealthPercent < 40),
                Spell.Cast("Kolto Overload", ret => BuddyTor.Me.HealthPercent <= 60),
                Spell.Cast("Unload", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.HasBuff("Barrage")),
                Spell.Cast("Rapid Shots", ret => BuddyTor.Me.ResourceStat > 30 && !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                Spell.Cast("Jet Boost", ret => BuddyTor.Me.ResourceStat >= 10 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                Spell.Cast("Rocket Punch", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= .8f),
            #region PreGravRound-Low LVL Support
 new Decorator(ret => !AbilityManager.HasAbility("Tracer Missile"),
                    new PrioritySelector(
                        Spell.Cast("Explosive Dart", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 10),
                        Spell.Cast("Rail Shot", ret => true),
                        Spell.Cast("Missile Blast", ret => BuddyTor.Me.ResourceStat >= 10),
                        Spell.Cast("Power Shot", ret => AbilityManager.CanCast("Power Shot", BuddyTor.Me.CurrentTarget)),
                        Spell.Cast("Rapid Shots", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                    )
                ),
            #endregion
                Spell.Cast("Heatseeker Missiles", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name).Stacks > 3 && BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Rail Shot", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Heat Signature" && B.Caster.Name == BuddyTor.Me.Name).Stacks > 3 && BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.HasBuff("Tracer Lock") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Tracer Lock").Stacks > 2),
                Spell.Cast("Unload", ret => BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Tracer Missile", ret => BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Rapid Shots", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                new Decorator(ret => Move,
                    Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
                    )
                )
                ) 
            #endregion
                )

            ));
        }

        [Class(CharacterClass.BountyHunter, AdvancedClass.Mercenary, SkillTreeId.MercenaryArsenal)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite ComandoGunneryOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Recharge and Reload", ret => BuddyTor.Me.ResourceStat >= 50 || BuddyTor.Me.HealthPercent < 100),
                Spell.BuffSelf("High Velocity Gas Cylinder", ret => AbilityManager.HasAbility("High Velocity Gas Cylinder")),
                Spell.BuffSelf("Combustible Gas Cylinder", ret => !AbilityManager.HasAbility("High Velocity Gas Cylinder")),
                new Action(ret => StopRest()),
                Spell.WaitForCast()
                );
        }
    }
}
