// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Focus : RotationBase
    {
        public override string Name
        {
            get { return "Guardian Focus"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Resolute", ret => Me.IsStunned),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Focused Defense", ret => Me.HealthPercent < 70),
                    Spell.Buff("Enure", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Combat Focus", ret => Me.ActionPoints <= 6 || Me.BuffCount("Singularity") < 3),
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
                    Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Interrupts
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Awe", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stasis", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Rotation
                    Spell.Cast("Force Exhaustion", ret => !Me.HasBuff("Protective Focus")),
                    Spell.Cast("Blade Storm", ret => !Me.HasBuff("Protective Focus")),
                    Spell.Cast("Sundering Strike", ret => !Me.HasBuff("Protective Focus")),
                    Spell.Cast("Blade Barrage"),
                    Spell.Cast("Focused Burst", ret => Me.HasBuff("Felling Blow") && Me.BuffCount("Singularity") < 3),
                    Spell.Cast("Concentrated Slice", ret => Me.HasBuff("Heightened Power")),
                    Spell.Cast("Zealous Leap"),
                    Spell.Cast("Riposte"),
                    Spell.Cast("Dispatch"),
                    Spell.Cast("Strike", ret => Me.Level <= 22 && Me.ActionPoints <= 5)
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
                        Spell.Cast("Zealous Leap"),
                        Spell.Cast("Force Sweep"),
                        Spell.Cast("Freezing Force"),
                        Spell.Cast("Cyclone Slash"),
                        Spell.Cast("Sundering Strike")
                        ));
            }
        }
    }
}
