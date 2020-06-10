// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Immortal : RotationBase
    {
        public override string Name
        {
            get { return "Juggernaut Immortal"; }
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
					Spell.Buff("Furious Power", ret => Me.CurrentTarget.BossOrGreater()),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 80),
                    Spell.Buff("Enraged Defense", ret => Me.HealthPercent < 70),
                    Spell.Buff("Invincible", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Enrage", ret => Me.ActionPoints <= 6),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Charge", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),
                    Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance > .4f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Interrupts
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Rotation
                    Spell.Cast("Retaliation"),
                    Spell.Cast("Crushing Blow"),
                    Spell.Cast("Force Scream"),
                    Spell.Cast("Aegis Assault", ret => Me.ActionPoints <= 7 || !Me.HasBuff("Aegis")),
                    Spell.Cast("Smash", ret => !Me.CurrentTarget.HasDebuff("Unsteady (Force)") && Targeting.ShouldPbaoe),
                    Spell.Cast("Backhand", ret => !Me.CurrentTarget.IsStunned),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("War Bringer")),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 9),
                    Spell.Cast("Assault")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.DoT("Chilling Scream", "Chilling Scream", 0, ret => Me.CurrentTarget.Distance <= 0.8f),
                        Spell.Cast("Smash"),
                        Spell.Cast("Crushing Blow", ret => Me.HasBuff("Aegis")),
                        Spell.Cast("Aegis Assault", ret => !Me.HasBuff("Aegis")),
                        Spell.Cast("Retaliation"),
                        Spell.Cast("Force Scream"),
                        Spell.Cast("Sweeping Slash")
                        ));
            }
        }
    }
}
