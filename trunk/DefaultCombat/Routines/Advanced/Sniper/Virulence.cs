using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Virulence : RotationBase
    {
        public override string Name { get { return "Sniper Virulence"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Escape"),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Laze Target"),
                    Spell.Cast("Target Acquired")
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

                    //Low Energy
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent < 60),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
                    Spell.DoT("Corrosive Grenade", "", 18000),
                    Spell.DoT("Corrosive Dart", "", 12000),
                    Spell.Cast("Weakening Blast"),
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Cull"),
                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                    Spell.Cast("Series of Shots", ret => Me.IsInCover()),
                    Spell.Cast("Snipe", ret => Me.IsInCover()),
                    Spell.Cast("Overload Shot")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAOE,
                    new LockSelector(
                        Spell.CastOnGround("Orbital Strike"),
                        Spell.Cast("Fragmentation Grenade"),
                        Spell.DoT("Corrosive Grenade", "", 18000),
                        Spell.CastOnGround("Suppressive Fire")
                    ));
            }
        }
    }
}