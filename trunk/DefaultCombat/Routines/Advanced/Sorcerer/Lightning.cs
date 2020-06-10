// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Lightning : RotationBase
    {
        public override string Name
        {
            get { return "Sorcerer Lightning"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
					Spell.Cast("Volt Rush", ret => Me.CurrentTarget.BossOrGreater()),   // maybe this could be implemented better in the rotation
                    Spell.Buff("Unlimited Power", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Cast("Recklessness"),
                    Spell.Cast("Polarity Shift"),
                    Spell.Cast("Consuming Darkness", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary")),
                    Spell.HoT("Static Barrier", on => Me, 60, ret => !Me.HasDebuff("Deionized") && !Me.HasBuff("Static Barrier")),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
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
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Thundering Blast"),
					Spell.Cast("Lightning Flash"),
                    Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Force Flash")),
					Spell.Cast("Shock", ret => Me.CurrentTarget.HasDebuff("Crushed (Crushing Darkness)")),
                    Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                    Spell.Cast("Lightning Bolt")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                        Spell.CastOnGround("Force Storm")
                        ));
            }
        }
    }
}
