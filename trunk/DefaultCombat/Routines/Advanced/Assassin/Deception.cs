using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Deception : RotationBase
    {
        public override string Name { get { return "Assassin Deception"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Surging Charge"),
                    Spell.Buff("Mark of Power"),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
                    Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Recklessness", ret => Me.BuffCount("Static Charge") < 1 && Me.InCombat),
                    Spell.Buff("Blackout", ret => Me.ForcePercent <= 40)
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

                    //Interrupts
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Low Slash", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),

                    //Rotation
                    Spell.Cast("Discharge", ret => Me.BuffCount("Static Charge") == 3),
                    Spell.Cast("Ball Lightning", ret => Me.BuffCount("Induction") == 2 && Me.Level >= 57),
                    Spell.Cast("Shock", ret => Me.BuffCount("Induction") == 2 && Me.Level < 57),
                    Spell.Cast("Maul", ret => (Me.HasBuff("Stealth") || Me.HasBuff("Duplicity")) && Me.IsBehind(Me.CurrentTarget)),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Voltaic Slash", ret => Me.Level >= 26),
                    Spell.Cast("Thrash", ret => Me.Level < 26),
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent <= 25),
                    Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPBAOE,
                        Spell.Cast("Lacerate", ret => Me.ForcePercent >= 60)
                );
            }
        }
    }
}