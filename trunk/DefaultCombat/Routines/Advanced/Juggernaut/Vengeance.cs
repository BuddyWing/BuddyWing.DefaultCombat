// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Vengeance : RotationBase
    {
        public override string Name
        {
            get { return "Juggernaut Vengeance"; }
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
                    Spell.Buff("Enraged Defense", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Enrage", ret => Me.ActionPoints <= 6),
                    Spell.Cast("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
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
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Impale"),
                    Spell.Cast("Shatter"),
                    Spell.Cast("Force Scream", ret => Me.BuffCount("Savagery") == 2),
                    Spell.Cast("Vengeful Slam"),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Hew", ret => Me.HasBuff("Destroyer") || Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Sundering Assault", ret => Me.ActionPoints <= 7),
                    Spell.Cast("Chilling Scream", ret => Me.ActionPoints >= 11)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Sweeping Slash"),
                        Spell.Cast("Vengeful Slam"),
                        Spell.Cast("Shatter"),
                        Spell.Cast("Impale"),
                        Spell.Cast("Chilling Scream", ret => Me.ActionPoints <= 2)
                        ));
            }
        }
    }
}
