﻿// Copyright (C) 2011-2018 Bossland GmbH
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
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
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
                    Spell.Cast("Recklessness"),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Phantom Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),
                    Spell.Cast("Force Speed", ret => CombatHotkeys.EnableCharge && Me.IsMoving && Me.CurrentTarget.Distance > 1f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Interrupts
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    Spell.Cast("Eradicate", ret => Me.ForcePercent < 25 && Me.HasBuff("Raze")),
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),

                    //Rotation
                    Spell.Cast("Eradicate", ret => Me.HasBuff("Raze")),
                    Spell.CastOnGround("Death Field", ret => !Me.CurrentTarget.HasDebuff("Deathmark") || Me.CurrentTarget.BuffCount("Deathmark") <= 2),
                    Spell.Cast("Creeping Terror", ret => !Me.CurrentTarget.HasDebuff("Creeping Terror") || Me.CurrentTarget.DebuffTimeLeft("Creeping Terror") <= 2),
                    Spell.Cast("Discharge", ret => !Me.CurrentTarget.HasDebuff("Discharge") || Me.CurrentTarget.DebuffTimeLeft("Discharge") <= 2),
                    Spell.Cast("Assassinate", ret => Me.HasBuff("Reaper's Rush") || Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Bloodletting")),
                    Spell.Cast("Leeching Strike", ret => Me.ForcePercent > 40),
                    Spell.Cast("Thrash")

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
						Spell.Cast("Severing Slash"),
                        Spell.Cast("Creeping Terror", ret => !Me.CurrentTarget.HasDebuff("Creeping Terror")),
                        Spell.CastOnGround("Death Field"),
                        Spell.Cast("Lacerate", ret => Me.ForcePercent > 70)
                        ));
            }
        }
    }
}
