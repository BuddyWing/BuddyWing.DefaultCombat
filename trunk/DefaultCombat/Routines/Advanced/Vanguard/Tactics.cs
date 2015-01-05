using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Tactics : RotationBase
    {
        public override string Name { get { return "Vanguard Tactics"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("High Energy Cell"),
                    Spell.Buff("Fortification")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Tenacity"),
                    Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() <= 50),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 80),
                    Spell.Buff("Battle Focus")
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Cast("Storm", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    new Decorator(ret => Me.ResourcePercent() < 60,
                        new LockSelector(
                            Spell.Cast("Stockstrike", ret => Me.HasBuff("Battering Ram")),
                            Spell.Cast("Hammer Shot")
                            )),

                    Spell.Cast("Pulse Cannon", ret => Me.BuffCount("Pulse Generator") == 3 && Me.CurrentTarget.Distance <= 1f),
                    Spell.DoT("Gut", "", 12000),
                    Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Combat Tactics")),
                    Spell.Cast("Stockstrike", ret => Me.HasBuff("Battering Ram")),
                    Spell.Cast("Fire Pulse"),
                    Spell.Cast("Ion Pulse")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAOE,
                        new LockSelector(
                            Spell.CastOnGround("Mortar Volley", ret => Me.CurrentTarget.Distance > Distance.MeleeAoE),
                            Spell.Cast("Sticky Grenade"),
                            Spell.Cast("Pulse Cannon", ret => Me.CurrentTarget.Distance <= 1f && Me.CurrentTarget.IsFacing)
                ));
            }
        }
    }
}