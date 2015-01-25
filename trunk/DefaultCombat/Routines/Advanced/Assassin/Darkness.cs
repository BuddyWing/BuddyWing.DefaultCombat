using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Darkness : RotationBase
    {
        public override string Name { get { return "Assassin Darkness"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Dark Charge"),
                    Spell.Buff("Mark of Power"),
                    Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Dark Ward", ret => !Me.HasBuff("Dark Ward")),
                    Spell.Buff("Unbreakable Will"),
                    Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Unity", ret => Me.CurrentTarget.BossOrGreater()),
                    Spell.Buff("Recklessness", ret => Me.CurrentTarget.BossOrGreater())
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),

                    //Rotation
                    Spell.Cast("Force Lightning", ret => Me.BuffCount("Harnessed Darkness") == 3),
                    Spell.Cast("Wither"),
                    Spell.Cast("Shock"),
                    Spell.Cast("Maul", ret => Me.HasBuff("Conspirator's Cloak")),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Discharge"),
                    Spell.Cast("Thrash"),
                    Spell.Cast("Saber Strike"),
                    Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
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
                            Spell.Cast("Wither"),
                            Spell.Cast("Discharge"))),
                    new Decorator(ret => Targeting.ShouldPBAOE,
                        Spell.Cast("Lacerate", ret => Me.ForcePercent >= 60))
                );
            }
        }
    }
}