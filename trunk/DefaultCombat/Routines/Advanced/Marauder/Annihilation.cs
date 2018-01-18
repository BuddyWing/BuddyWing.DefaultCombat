// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Annihilation : RotationBase
    {
        public override string Name
        {
            get { return "Marauder Annihilation"; }
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
                    Spell.Buff("Deadly Saber", ret => !Me.HasBuff("Deadly Saber")),
                    Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 20),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Frenzy", ret => Me.BuffCount("Fury") < 5),
                    Spell.Buff("Berserk", ret => Me.BuffCount("Fury") > 29),
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
                    Spell.Cast("Dual Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
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
                    Spell.Cast("Annihilate"),
                    Spell.DoT("Force Rend", "Force Rend"),
                    Spell.DoT("Rupture", "Bleeding (Rupture)"),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Dual Saber Throw", ret => Me.HasBuff("Pulverize")),
                    Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 9 && Me.CurrentTarget.HasDebuff("Bleeding (Rupture)")),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),
                    Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 6),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),
                    Spell.Cast("Force Charge", ret => Me.ActionPoints <= 8),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),
                    Spell.Cast("Assault", ret => Me.ActionPoints < 9 && Me.CurrentTarget.HasDebuff("Bleeding (Rupture)")),
                    Spell.Cast("Annihilate"),
                    Spell.Cast("Force Rend"),

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
                        Spell.Cast("Dual Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                        Spell.Cast("Smash", ret => Me.CurrentTarget.HasDebuff("Bleeding (Rupture)") && Me.CurrentTarget.HasDebuff("Force Rend")),
                        Spell.DoT("Force Rend", "Force Rend"),
                        Spell.DoT("Rupture", "Bleeding (Rupture)"),
                        Spell.Cast("Sweeping Slash")
                        ));
            }
        }
    }
}
