using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Watchman : RotationBase
    {
        public override string Name { get { return "Sentinel Watchman"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Juyo Form"),
                    Spell.Buff("Force Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Rebuke", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 10),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30)
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

                    //Rotation
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Buff("Zen", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") > 29),
                    Spell.Buff("Valorous Call", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") < 15),
                    Spell.Cast("Cauterize", ret => !Me.CurrentTarget.HasDebuff("Burning (Cauterize)")),
                    Spell.Cast("Merciless Slash"),
                    Spell.Cast("Force Melt"),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Buff("Overload Saber"),
                    Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 5),
                    Spell.Cast("Master Strike"),
                    Spell.Cast("Slash"),
                    Spell.Cast("Strike")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPBAOE,
                        new LockSelector(
                           Spell.Cast("Force Sweep"),
                           Spell.Cast("Twin Saber Throw"),
                           Spell.Cast("Cyclone Slash")
                ));
            }
        }
    }
}