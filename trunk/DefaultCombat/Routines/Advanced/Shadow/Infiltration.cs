// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Infiltration : RotationBase
    {
        public override string Name
        {
            get { return "Shadow Infiltration"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Valor"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force of Will", ret => Me.IsStunned),
                    Spell.Buff("Battle Readiness"),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
                    Spell.Cast("Force Potency", ret => Me.BuffCount("Breaching Shadow") < 1 && Me.InCombat),
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
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f),

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
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Low Slash", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Rotation
                    Spell.Cast("Force Breach", ret => Me.BuffCount("Breaching Shadow") == 3),
                    Spell.Cast("Psychokinetic Blast", ret => Me.BuffCount("Circling Shadows") == 2),
                    Spell.Cast("Vaulting Slash"),
                    Spell.Cast("Shadow Strike", ret => Me.HasBuff("Infiltration Tactics")),
                    Spell.Cast("Spinning Strike", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Stalker's Swiftness")),
                    Spell.Cast("Clairvoyant Strike"),
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent <= 25)

                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Severing Slash"),
                        Spell.Cast("Cleaving Cut", ret => Me.ForcePercent >= 60)
                    ));
            }
        }
    }
}
