// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Gunnery : RotationBase
    {
        public override string Name
        {
            get { return "Commando Gunnery"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Fortification")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Tenacity", ret => Me.IsStunned),
                    Spell.Buff("Supercharged Celerity", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Echoing Deterrence", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Recharge Cells", ret => Me.ResourceStat <= 60),
                    Spell.Cast("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10),
                    Spell.Cast("Reserve Powercell", ret => Me.ResourceStat <= 60),
                    Spell.Cast("Tech Override"),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Hammer Shot", ret => Me.ResourcePercent() <= 60),

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
					Spell.Cast("Vortex Bolt"),
					Spell.Cast("Electro Net"),
					Spell.Cast("High Impact Bolt", ret => Me.BuffCount("Charged Barrel") >= 4),
					Spell.Cast("Demolition Round", ret => Me.CurrentTarget.HasDebuff("Gravity Vortex")),
					Spell.Cast("Boltstorm", ret => !Me.HasBuff("Reserve Powercell")),
					Spell.Cast("Grav Round"),
					Spell.Cast("Hammer Shot")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.CastOnGround("Mortar Volley"),
                        Spell.CastOnGround("Hail of Bolts")
                        ));
            }
        }
    }
}
