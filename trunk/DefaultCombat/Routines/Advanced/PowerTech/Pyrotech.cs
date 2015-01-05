using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Pyrotech : RotationBase
    {
        public override string Name { get { return "PowerTech Pyrotech"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Combustible Gas Cylinder"),
                    Spell.Buff("Hunter's Boon")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Determination"),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 65),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    new Decorator(ret => Me.ResourcePercent() > 40,
                        new LockSelector(
                            Spell.Cast("Flame Burst", ret => Me.HasBuff("Flame Barrage")),
                            Spell.Cast("Rapid Shots")
                            )),

                    Spell.Cast("Flame Thrower", ret => Me.BuffCount("Superheated Flame Thrower") == 3),
                    Spell.DoT("Incendiary Missile", "Burning (Incendiary Missle)"),
                    Spell.DoT("Scorch", "Burning (Scorch)"),
                    Spell.Cast("Rail Shot", ret => Me.HasBuff("Charged Gauntlets")),
                    Spell.Cast("Immolate"),
                    Spell.Cast("Thermal Detonator"),
                    Spell.Cast("Flaming Fist"),
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
                            Spell.DoT("Scorch", "Burning (Scorch)"),
                            Spell.CastOnGround("Death from Above", ret => Me.CurrentTarget.Distance > Distance.MeleeAoE),
                            Spell.Cast("Explosive Dart")
                            )),
                    new Decorator(ret => Targeting.ShouldPBAOE,
                        new LockSelector(
                            Spell.DoT("Scorch", "Burning (Scorch)"),
                            Spell.Cast("Flame Thrower"),
                            Spell.Cast("Flame Sweep")
                )));
            }
        }
    }
}