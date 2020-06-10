// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Darkness : RotationBase
    {
        public override string Name
        {
            get { return "Assassin Darkness"; }
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
                    Spell.Cast("Dark Ward", ret => !Me.HasBuff("Dark Ward")),
                    Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
                    Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Force Speed", ret => Me.HealthPercent <= 40),
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
                    Spell.Buff("Phantom Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),

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

                    //Rotation
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Wither"),
                    Spell.Cast("Shock"),
                    Spell.Cast("Maul", ret => Me.HasBuff("Conspirator's Cloak")),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Discharge"),
                    Spell.Cast("Thrash"),
                    Spell.Cast("Saber Strike"),
                    Spell.Cast("Depredating Volts", ret => Me.BuffCount("Harnessed Darkness") > 2)

                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => Targeting.ShouldAoe,
                        new PrioritySelector(
                            Spell.Cast("Wither"),
							Spell.Cast("Severing Slash"),
                            Spell.Cast("Discharge"))),
                    new Decorator(ret => Targeting.ShouldPbaoe,
                        Spell.Cast("Lacerate", ret => Me.ForcePercent >= 60))
                    );
            }
        }
    }
}
