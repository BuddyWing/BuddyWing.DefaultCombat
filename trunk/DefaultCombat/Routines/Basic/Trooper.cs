using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Trooper : RotationBase
    {
        public override string Name { get { return "Basic Trooper"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Fortification")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector();
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    CombatMovement.CloseDistance(Distance.Ranged),
                    Spell.Cast("Sticky Grenade"),
                    Spell.CastOnGround("Mortar Volley", ret => Me.CurrentTarget.Distance > .5f),
                    Spell.Cast("High Impact Bolt"),
                    Spell.Cast("Recharge Cells", ret => Me.ResourcePercent() <= 50),
                    Spell.Cast("Stockstrike", ret => Me.CurrentTarget.Distance <= .4f),
                    Spell.Cast("Pulse Cannon", ret => Me.CurrentTarget.Distance <= 1f),
                    Spell.Cast("Ion Pulse", ret => Me.ResourcePercent() >= 50),
                    Spell.Cast("Hammer Shot")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAOE,
                    new LockSelector());
            }
        }
    }
}
