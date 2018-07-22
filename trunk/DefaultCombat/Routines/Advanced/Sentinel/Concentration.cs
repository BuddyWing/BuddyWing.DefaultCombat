// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Concentration : RotationBase
    {
        public override string Name
        {
            get { return "Sentinel Concentration"; }
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
                    Spell.Buff("Rebuke", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Force Camouflage", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 15),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 25),
                    Spell.Buff("Zen", ret => Me.BuffCount("Centering") > 29 && !Me.HasBuff("Koan")),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Twin Saber Throw", ret => CombatHotkeys.EnableCharge && !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

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
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Force Exhaustion", ret => !Me.HasBuff("Koan")),
                    Spell.Cast("Zealous Leap", ret => CombatHotkeys.EnableCharge && !Me.HasBuff("Felling Blow")),
                    Spell.Cast("Concentrated Slice"),
                    Spell.Cast("Focused Burst", ret => Me.HasBuff("Koan") || Me.HasBuff("Felling Blow")),
                    Spell.Cast("Blade Barrage", ret => Me.HasBuff("Heightened Power")),
                    Spell.Cast("Zealous Strike", ret => Me.HasBuff("Focused Slash")),
                    Spell.Cast("Blade Storm", ret => Me.HasBuff("Momentum")),
                    Spell.Cast("Slash"),
                    Spell.Cast("Strike", ret => Me.ActionPoints <= 4)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Force Sweep"),
                        Spell.Cast("Cyclone Slash"),
                        Spell.Cast("Twin Saber Throw", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance <= 3f)
                        ));
            }
        }
    }
}
