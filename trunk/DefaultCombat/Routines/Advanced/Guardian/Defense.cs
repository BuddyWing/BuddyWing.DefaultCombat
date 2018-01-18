﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows.Input;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat.Routines
{
    internal class Defense : RotationBase
    {
        public override string Name
        {
            get { return "Guardian Defense"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Might"),
                    Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
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
                    Spell.Buff("Enure", ret => Me.HealthPercent <= 80),
                    Spell.Buff("Focused Defense", ret => Me.HealthPercent < 70),
                    Spell.Buff("Warding Call", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Combat Focus", ret => Me.ActionPoints <= 6),
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
                    Spell.Cast("Dispatch", ret => Me.HasBuff("Ardent Advocate")),
                    Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
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

                    //Rotation
                    Spell.Cast("Hilt Bash", ret => !Me.CurrentTarget.IsStunned && Me.ActionPoints >= 7 && AbilityManager.CanCast("Guardian Slash", Me.CurrentTarget)),
                    Spell.Cast("Riposte"),
                    Spell.Cast("Warding Strike", ret => Me.ActionPoints <= 6 || !Me.HasBuff("Warding Strike")),
                    Spell.Cast("Guardian Slash"),
                    Spell.Cast("Blade Barrage"),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Blade Storm"),
                    Spell.Cast("Force Sweep", ret => !Me.CurrentTarget.HasDebuff("Unsteady (Force)") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.Cast("Freezing Force", ret => AbilityManager.HasAbility("Persistent Chill") && Me.CurrentTarget.Distance <= 0.5f && !Me.CurrentTarget.HasMyDebuff("Freezing Force")),
                    Spell.Cast("Saber Throw", ret => Me.ActionPoints <= 3),
                    Spell.Cast("Slash", ret => !Me.CurrentTarget.HasDebuff("Trauma")),
                    Spell.Cast("Slash", ret => Me.ActionPoints >= 9),
                    Spell.Cast("Strike"),
                    Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat),

                    //HK-55 Mode Rotation
                    Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
                    Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
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
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.Cast("Guardian Slash", ret => Me.HasBuff("Warding Strike")),
                        Spell.Cast("Warding Strike", ret => !Me.HasBuff("Warding Strike")),
                        Spell.Cast("Force Sweep"),
                        Spell.DoT("Freezing Force", "Freezing Force", 0, ret => Me.CurrentTarget.Distance <= 0.8f),
                        Spell.Cast("Cyclone Slash")
                        ));
            }
        }
    }
}