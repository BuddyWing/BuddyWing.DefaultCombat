// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Hatred : RotationBase
    {
        public override string Name
        {
            get { return "Assasin Hatred"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power"),
                    Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
                    Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Recklessness"),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Phantom Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

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

                    //Low Energy
                    Spell.Cast("Eradicate", ret => Me.ForcePercent < 25 && Me.HasBuff("Raze")),
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Eradicate", ret => Me.HasBuff("Raze")),
                    Spell.CastOnGround("Death Field", ret => !Me.CurrentTarget.HasDebuff("Deathmark") || Me.CurrentTarget.BuffCount("Deathmark") <= 2),
                    Spell.Cast("Creeping Terror", ret => !Me.CurrentTarget.HasDebuff("Creeping Terror") || Me.CurrentTarget.DebuffTimeLeft("Creeping Terror") <= 2),
                    Spell.Cast("Discharge", ret => !Me.CurrentTarget.HasDebuff("Discharge") || Me.CurrentTarget.DebuffTimeLeft("Discharge") <= 2),
                    Spell.Cast("Shock", ret => Me.Level < 26),
                    Spell.Cast("Assassinate", ret => Me.HasBuff("Reaper's Rush") || Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Bloodletting")),
                    Spell.Cast("Leeching Strike", ret => Me.ForcePercent > 40),
                    Spell.Cast("Thrash"),
                    Spell.Buff("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)

                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.DoT("Discharge", "Discharge"),
                        Spell.Cast("Creeping Terror", ret => !Me.CurrentTarget.HasDebuff("Creeping Terror")),
                        Spell.CastOnGround("Death Field"),
                        Spell.Cast("Lacerate", ret => Me.ForcePercent > 70)
                        ));
            }
        }
    }
}
