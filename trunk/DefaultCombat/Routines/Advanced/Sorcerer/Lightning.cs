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
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Solo Mode
					Spell.Cast("Resurgence", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
					Spell.Cast("Dark Heal", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
					Spell.Cast("Unnatural Preservation", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

					//Rotation
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Thundering Blast"),
					Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
					Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Force Flash")),
					Spell.Cast("Lightning Flash"),
					Spell.Cast("Shock", ret => Me.CurrentTarget.HasDebuff("Crushed (Crushing Darkness)")),
					Spell.Cast("Chain Lightning", ret => Me.HasBuff("Focal Lightning")),
					Spell.Cast("Lightning Bolt"),
					Spell.Cast("Force Lightning", ret => Me.HasBuff("Lightning Barrage")),
					Spell.Cast("Lightning Strike")
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
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
						Spell.CastOnGround("Force Storm")
						));
			}
		}
	}
}