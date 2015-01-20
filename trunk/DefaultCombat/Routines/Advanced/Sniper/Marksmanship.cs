using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Marksmanship : RotationBase
    {
        public override string Name { get { return "Sniper Marksmanship"; } }

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
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 40),
                    Spell.Buff("Sniper Volley", ret => Me.EnergyPercent <= 60),
                    Spell.Buff("Entrench", ret => Me.CurrentTarget.StrongOrGreater() && Me.IsInCover()),
                    Spell.Buff("Laze Target"),
                    Spell.Buff("Target Acquired")
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
                    new Decorator(ret => Me.EnergyPercent < 60,
                        new LockSelector(
                            Spell.Cast("Rifle Shot")
                            )),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                    Spell.Cast("Followthrough"),
                    Spell.Cast("Penetrating Blasts", ret => Me.IsInCover() && Me.Level >= 26),
                    Spell.Cast("Series of Shots", ret => Me.IsInCover() && Me.Level < 26),
                    Spell.DoT("Corrosive Dart", "Poisoned (Corrosive Dart)"),
                    Spell.Cast("Ambush", ret => Me.IsInCover() && Me.BuffCount("Zeroing Shots") == 2),
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Snipe", ret => Me.IsInCover()),
                    Spell.Cast("Overload Shot", ret => !Me.IsInCover())
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
                        Spell.CastOnGround("Suppressive Fire")
                    ));
            }
        }
    }
}