﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Engineering : RotationBase
    {
        public override string Name
        {
            get { return "Sniper Engineering"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Laze Target"),
                    Spell.Buff("Target Acquired"),
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
                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),


                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 30,
                        new PrioritySelector(
                            Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                            Spell.Cast("Rifle Shot")
                            )),

                    //Burn-ish
                    new Decorator(ret => Me.CurrentTarget.HealthPercent < 30,
                        new PrioritySelector(
                            Spell.Cast("Explosive Probe"),
                            Spell.Cast("Series of Shots"),
                            Spell.DoTGround("Plasma Probe", 8500),
                            Spell.DoT("Interrogation Probe", "Interrogation Probe"),
                            Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                            Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
                            Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Marked [Physical]")),
                            Spell.Cast("Takedown"),
                            Spell.CastOnGround("Orbital Strike")
                            )),


                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.CastOnGround("Orbital Strike", ret => Me.HasBuff("Target Acquired")),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Series of Shots"),
                    Spell.DoTGround("Plasma Probe", 8500),
                    Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                    Spell.DoT("Interrogation Probe", "Interrogation Probe"),
                    Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
                    Spell.CastOnGround("Orbital Strike"),
                    Spell.DoT("Corrosive Dart", "Corrosive Dart"),
                    Spell.Cast("Fragmentation Grenade"),
                    Spell.Cast("Rifle Shot"),
                    Spell.Cast("Snipe"),

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
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.CastOnGround("Orbital Strike", ret => Me.IsInCover() && Me.EnergyPercent > 30),
                        Spell.DoTGround("Plasma Probe", 9000),
                        Spell.Cast("Suppressive Fire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
                        ));
            }
        }
    }
}