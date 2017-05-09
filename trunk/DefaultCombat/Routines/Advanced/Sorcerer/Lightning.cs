// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Lightning : RotationBase
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
					Spell.Buff("Recklessness", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Polarity Shift", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent <= 80),
					Spell.HoT("Static Barrier", on => Me, 99, ret => !Me.HasDebuff("Deionized") && !Me.HasBuff("Static Barrier")),
					Spell.Buff("Consuming Darkness", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary")),
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
					CombatMovement.CloseDistance(Distance.Ranged),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Rotation
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
					Spell.Cast("Thundering Blast", ret => Me.CurrentTarget.HasDebuff("Affliction")),
					Spell.Cast("Lightning Flash"),
					Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Force Flash")),
					Spell.Cast("Force Lightning", ret => Me.HasBuff("Lightning Barrage")),
					Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
					Spell.Cast("Lightning Bolt", ret => Me.Level >= 57),
					Spell.Cast("Lightning Strike", ret => Me.Level < 57),
					Spell.Cast("Shock")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
						Spell.CastOnGround("Force Storm")
						));
			}
		}
	}
}