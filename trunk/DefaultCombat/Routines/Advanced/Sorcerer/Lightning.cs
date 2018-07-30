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
                    Spell.Buff("Recklessness"),
                    Spell.Buff("Polarity Shift"),
                    Spell.Buff("Consuming Darkness", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary")),
                    Spell.HoT("Static Barrier", on => Me, 60, ret => !Me.HasDebuff("Deionized") && !Me.HasBuff("Static Barrier")),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
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
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Solo Mode
                    Spell.Cast("Resurgence", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
                    Spell.Cast("Dark Heal", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
                    Spell.Cast("Unnatural Preservation", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Thundering Blast", ret => Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Force Flash")),
                    Spell.Cast("Lightning Flash"),
                    Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                    Spell.Cast("Lightning Bolt"),
                    Spell.Cast("Lightning Strike"),
                    Spell.Cast("Shock"),
                    Spell.Cast("Force Lightning")
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
