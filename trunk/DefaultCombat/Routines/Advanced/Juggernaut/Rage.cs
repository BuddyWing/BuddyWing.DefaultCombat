// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Rage : RotationBase
    {
        public override string Name
        {
            get { return "Juggernaut Rage"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unnatural Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unleash", ret => Me.IsStunned),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Enraged Defense", ret => Me.HealthPercent < 70),
                    Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Enrage", ret => Me.ActionPoints <= 6),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
                    Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Charge", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Crush", ret => !Me.HasBuff("Enveloping Rage")),
                    Spell.Cast("Force Scream", ret => !Me.HasBuff("Enveloping Rage")),
                    Spell.Cast("Sundering Assault", ret => !Me.HasBuff("Enveloping Rage")),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Raging Burst", ret => Me.HasBuff("Dominate") && Me.BuffCount("Shockwave") < 3),
                    Spell.Cast("Furious Strike", ret => Me.HasBuff("Cascading Power")),
                    Spell.Cast("Obliterate"),
                    Spell.Cast("Retaliation"),
                    Spell.Cast("Vicious Throw"),
                    Spell.Cast("Assault", ret => Me.Level <= 22 && Me.ActionPoints <= 5)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.Cast("Saber Throw"),
                        Spell.Cast("Obliterate"),
                        Spell.Cast("Smash"),
                        Spell.Cast("Chilling Scream"),
                        Spell.Cast("Cyclone Slash"),
                        Spell.Cast("Sweeping Assault")
                        ));
            }
        }
    }
}
