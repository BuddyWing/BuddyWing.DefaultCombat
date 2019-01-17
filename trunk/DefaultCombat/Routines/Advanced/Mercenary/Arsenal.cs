// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Arsenal : RotationBase
    {
        public override string Name
        {
            get { return "Mercenary Arsenal"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Hunter's Boon")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Supercharged Celerity", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 70),
                    Spell.Cast("Responsive Safeguards", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 60),
                    Spell.Buff("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 10),
                    Spell.Buff("Thermal Sensor Override", ret => Me.ResourceStat >= 60),
                    Spell.Buff("Power Surge"),
                    Spell.Cast("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() > 60),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Solo Mode
                    Spell.Cast("Kolto Shot", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
                    Spell.Cast("Emergency Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
                    Spell.Cast("Rapid Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Blazing Bolts", ret => !Me.HasBuff("Thermal Sensor Override")),
                    Spell.Cast("Heatseeker Missiles", ret => Me.CurrentTarget.HasDebuff("Heat Signature")),
                    Spell.Cast("Electro Net", ret => !Me.HasBuff("Thermal Sensor Override")),
                    Spell.Cast("Rail Shot", ret => Me.BuffCount("Tracer Lock") == 5),
                    Spell.Cast("Priming Shot"),
                    Spell.Cast("Blazing Bolts", ret => Me.ResourceStat <= 50),
                    Spell.Cast("Tracer Missile", ret => Me.HasBuff("Tracer Beacon")),
                    Spell.Cast("Tracer Missile")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.CastOnGround("Death from Above", ret => Me.HasBuff("Thermal Sensor Override")),
                        Spell.Cast("Fusion Missile", ret => Me.ResourceStat <= 10 && Me.HasBuff("Power Surge")),
                        Spell.Cast("Explosive Dart"),
                        Spell.CastOnGround("Sweeping Blasters", ret => Me.ResourcePercent() <= 10))
                    );
            }
        }
    }
}
