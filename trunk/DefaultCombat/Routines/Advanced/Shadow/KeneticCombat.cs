// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class KeneticCombat : RotationBase
    {
        public override string Name
        {
            get { return "Shadow Kenetic Combat"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Valor")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force of Will", ret => Me.IsStunned),
                    Spell.Cast("Kinetic Ward", ret => Me.BuffCount("Kinetic Ward") <= 1 || Me.BuffTimeLeft("Kinetic Ward") < 3),
                    Spell.Buff("Battle Readiness", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Force Speed", ret => Me.HealthPercent <= 40),
                    Spell.Cast("Force Potency"),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Shadow Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Interrupts
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Cascading Debris", ret => Me.BuffCount("Harnessed Shadows") == 3),
                    Spell.Cast("Slow Time"),
                    Spell.Cast("Project", ret => Me.HasBuff("Particle Acceleration")),
                    Spell.Cast("Shadow Strike", ret => Me.HasBuff("Shadow Wrap")),
                    Spell.Cast("Spinning Strike", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Force Breach"),
                    Spell.Cast("Double Strike")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Slow Time"),
						Spell.Cast("Cleaving Cut"),
                        Spell.Cast("Force Breach"),
                        Spell.Cast("Whirling Blow", ret => Me.ForcePercent >= 60 && Me.CurrentTarget.Distance <= 0.5f)
                        ));
            }
        }
    }
}
