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
    public static class CommandoGunnery
    {

        public static bool Burn = false;
        public static bool Move = true;
        public static bool AllCombat = true;
        private static DateTime LastChecked { get; set; }
        [Class(CharacterClass.Trooper, AdvancedClass.Commando, SkillTreeId.CommandoGunnery)]
        [Behavior(BehaviorType.Pull)]
        public static Composite CommandoGunneryPull()
        {
            return CommandoGunneryCombat();
            //new PrioritySelector(
                // new Decorator(ret => BuddyTor.Me.IsMounted, new PrioritySelector()), --- still pulling if mounted
                // We need to sort our movement and line-of-sight stuff out before anything else.
              //  Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f),
               // Movement.StopInRange(2.8f),
               // Spell.BuffSelf("Reserve Powercell", ret => BuddyTor.Me.Level > 41 && AbilityManager.HasAbility("Reserve Powercell")),
               // Spell.Cast("Plasma Grenade", ret => BuddyTor.Me.Level > 17 && AbilityManager.HasAbility("Plasma Grenade")),
               // Spell.Cast("Sticky Grenade", ret => AbilityManager.HasAbility("Sticky Grenade")),
               // Spell.Cast("Hammer Shot", ret => true)
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
        [Class(CharacterClass.Trooper, AdvancedClass.Commando, SkillTreeId.CommandoGunnery)]
        public static Composite CommandoGunneryCombat()
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
                new Decorator(ret => BuddyTor.Me.ResourceStat < 2,
                        new Action(ret => Burn = false)
                ),
                new Decorator(ret => Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"),
                    new PrioritySelector(
                        Spell.WaitForCast(),
                //new Action(ret => BuddyTor.Me.InventoryEquipped.FirstOrDefault(item => item.IsUsable).Use()),
                        Spell.Cast("Full Auto", ret => BuddyTor.Me.HasBuff("Curtain of Fire")),
                        Spell.Cast("High Impact Bolt", ret => BuddyTor.Me.HasBuff("Charged Barrel") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Charged Barrel").Stacks > 4),
                        Spell.Cast("Demolition Round", ret => true),
                        Spell.Cast("Stockstrike", ret => BuddyTor.Me.CurrentTarget.Distance <= .8f),
                        Spell.Cast("Grav Round", ret => true),
                        Spell.Cast("Charged Bolts", ret => AbilityManager.CanCast("Charged Bolts", BuddyTor.Me.CurrentTarget)),
                        Spell.Cast("Explosive Round", ret => true)
                    )
                ),
            #endregion
            #region Lazyraiderish stuff - works with combat movement off
            new Decorator(ret => BuddyTor.Me.IsMoving,
                    new PrioritySelector(
                        Spell.BuffSelf("Reactive Shield", ret => BuddyTor.Me.HealthPercent < 40),
                        Spell.Cast("Adrenaline Rush", ret => BuddyTor.Me.HealthPercent <= 60),
                        Spell.Cast("Hammer Shot", ret => BuddyTor.Me.ResourceStat < 9 && !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                        Spell.Cast("Concussion Charge", ret => BuddyTor.Me.ResourceStat >= 10 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                        Spell.Cast("Demolition Round", ret => BuddyTor.Me.ResourceStat >= 10 || (Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                        Spell.Cast("High Impact Bolt", ret => true),
                        Spell.Cast("Sticky Grenade", ret => true),
                        Spell.Cast("Hammer Shot", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                    )
                ),
            #endregion
            new Decorator(ret => !Burn || !BuddyTor.Me.HasBuff("Critical Boost") || !BuddyTor.Me.HasBuff("Power Boost"),
                new PrioritySelector(Spell.Cast("Recharge Cells", ret => BuddyTor.Me.ResourceStat <= 6),
                Spell.Cast("Grav Round", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name).TimeLeft.Seconds < 6),
                Spell.WaitForCast(),
                Spell.Cast("Tenacity", ret => BuddyTor.Me.IsStunned),
                Spell.BuffSelf("Reactive Shield", ret => BuddyTor.Me.HealthPercent < 40),
                Spell.Cast("Adrenaline Rush", ret => BuddyTor.Me.HealthPercent <= 60),
                Spell.Cast("Full Auto", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.HasBuff("Curtain of Fire")),
                Spell.Cast("Hammer Shot", ret => BuddyTor.Me.ResourceStat < 9 && !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                Spell.Cast("Concussion Charge", ret => BuddyTor.Me.ResourceStat >= 10 && Helpers.Targets.Count(t => t.Distance <= .4f) >= 2),
                Spell.Cast("Stockstrike", ret => BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.CurrentTarget.Distance <= .8f),
            #region PreGravRound-Low LVL Support
 new Decorator(ret => !AbilityManager.HasAbility("Grav Round"),
                    new PrioritySelector(
                        Spell.Cast("Sticky Grenade", castWhen => Helpers.Targets.Count() >= 3 && BuddyTor.Me.ResourceStat >= 10),
                        Spell.Cast("High Impact Bolt", ret => true),
                        Spell.Cast("Explosive Round", ret => BuddyTor.Me.ResourceStat >= 10),
                        Spell.Cast("Charged Bolts", ret => AbilityManager.CanCast("Charged Bolts", BuddyTor.Me.CurrentTarget)),
                        Spell.Cast("Hammer Shot", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost")))
                    )
                ),
            #endregion
                Spell.Cast("Demolition Round", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name).Stacks > 3 && BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("High Impact Bolt", ret => BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name) != null && BuddyTor.Me.CurrentTarget.Debuffs.FirstOrDefault(B => B.Name == "Gravity Vortex" && B.Caster.Name == BuddyTor.Me.Name).Stacks > 3 && BuddyTor.Me.ResourceStat >= 10 && BuddyTor.Me.HasBuff("Charged Barrel") && BuddyTor.Me.Buffs.FirstOrDefault(B => B.Name == "Charged Barrel").Stacks > 2),
                Spell.Cast("Full Auto", ret => BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Grav Round", ret => BuddyTor.Me.ResourceStat >= 10),
                Spell.Cast("Hammer Shot", ret => !(Burn || BuddyTor.Me.HasBuff("Critical Boost") || BuddyTor.Me.HasBuff("Power Boost"))),
                new Decorator(ret => Move,
                    Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
                    )
                )
                ) 
            #endregion
                )

            ));
        }

        [Class(CharacterClass.Trooper, AdvancedClass.Commando, SkillTreeId.CommandoGunnery)]
        [Behavior(BehaviorType.OutOfCombat)]
        public static Composite ComandoGunneryOutOfCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Recharge and Reload", ret => BuddyTor.Me.ResourceStat <= 6 || BuddyTor.Me.HealthPercent < 100),
                Spell.BuffSelf("Armor-piercing Cell", ret => AbilityManager.HasAbility("Armor-piercing Cell")),
                Spell.BuffSelf("Plasma Cell", ret => !AbilityManager.HasAbility("Armor-piercing Cell")),
                new Action(ret => StopRest()),
                Spell.WaitForCast()
                );
        }
    }
}
