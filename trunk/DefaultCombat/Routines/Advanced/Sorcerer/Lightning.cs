using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Lightning : RotationBase
    {
        public override string Name { get { return "Sorcerer Lightning"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Recklessness"),
                    Spell.Buff("Polarity Shift"),
                    Spell.Buff("Static Barrier", ret => !Me.HasBuff("Deionized")),
                    Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50),
                    Spell.Buff("Consuming Darkness", ret => Me.HealthPercent > 15 && Me.ForcePercent < 40)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Thundering Blast", ret => Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Lightning Flash"),	
                    Spell.DoT("Crushing Darkness", "", 6000),
                    Spell.Cast("Force Lightning", ret => Me.HasBuff("Lightning Barrage")),
                    Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                    Spell.Cast("Lightning Bolt"),
                    Spell.Cast("Lightning Strike"),
                    Spell.Cast("Shock"));
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAOE,
                    new LockSelector(
                        Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                        Spell.CastOnGround("Force Storm")
                    ));
            }
        }
    }
}