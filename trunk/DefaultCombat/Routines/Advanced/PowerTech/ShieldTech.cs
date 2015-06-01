using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class ShieldTech : RotationBase
    {
        public override string Name { get { return "PowerTech Shieldtech"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Ion Gas Cylinder"),
                    Spell.Buff("Hunter's Boon"),
                    Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Shoulder Cannon", ret => !Me.HasBuff("Shoulder Cannon"))
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Cast("Jet Charge", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    new Decorator(ret => Me.ResourcePercent() > 40,
                        new LockSelector(
                            Spell.Cast("Heat Blast", ret => Me.BuffCount("Heat Screen") == 3),
                            Spell.Cast("Firestorm", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level >= 57),
                            Spell.Cast("Flame Thrower", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level < 57),
                            Spell.Cast("Flame Burst", ret => Me.HasBuff("Flame Surge")),
                            Spell.Cast("Rapid Shots")
                            )),

                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee && !DefaultCombat.MovementDisabled),
                    Spell.CastOnGround("Oil Slick", ret => Me.CurrentTarget.BossOrGreater() && Me.CurrentTarget.Distance <= 0.8f),
                    Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater()),
                    Spell.Cast("Heat Blast", ret => Me.BuffCount("Heat Screen") == 3),
                    Spell.Cast("Rocket Punch"),
                    Spell.Cast("Rail Shot"),
                    Spell.Cast("Firestorm", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level >= 57),
                    Spell.Cast("Flame Thrower", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level < 57),
                    Spell.Cast("Flame Burst")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new LockSelector(
                    new Decorator(ret => Targeting.ShouldAOE,
                        new LockSelector(
                            Spell.CastOnGround("Death from Above"),
                            Spell.Cast("Explosive Dart")
                            )),
                    new Decorator(ret => Targeting.ShouldPBAOE,
                        new LockSelector(
                            Spell.Cast("Firestorm", ret => Me.Level >= 57),
                            Spell.Cast("Flame Thrower", ret => Me.Level < 57),
                            Spell.Cast("Flame Sweep")
                )));
            }
        }
    }
}