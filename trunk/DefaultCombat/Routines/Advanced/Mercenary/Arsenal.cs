// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

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
                    Spell.Buff("Responsive Safeguards", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Vent Heat", ret => Me.ResourcePercent() >= 60),
                    Spell.Cast("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 10),
                    Spell.Cast("Thermal Sensor Override", ret => Me.ResourceStat >= 60),
                    Spell.Cast("Power Surge"),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() >= 60),
					

                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Priming Shot"),
                    Spell.Cast("Tracer Missile", ret => Me.HasBuff("Tracer Beacon")),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Rail Shot", ret => Me.BuffCount("Tracer Lock") == 5),
                    Spell.Cast("Heatseeker Missiles", ret => Me.CurrentTarget.HasDebuff("Heat Signature")),
                    Spell.Cast("Blazing Bolts"),
                    Spell.Cast("Tracer Missile"),
                    Spell.Cast("Rapid Shots")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.CastOnGround("Death from Above"),
                        Spell.CastOnGround("Sweeping Blasters")
                    ));
            }
        }
    }
}
