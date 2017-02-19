// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class ShieldTech : RotationBase
	{
		public override string Name
		{
			get { return "PowerTech Shieldtech"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Hunter's Boon"),
					Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
				  // Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
					Spell.Buff("Determination", ret => Me.IsStunned),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
					Spell.Buff("Shoulder Cannon", ret => !Me.HasBuff("Shoulder Cannon"))
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Jet Charge", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					
					//Rotation
					new Decorator(ret => Me.ResourcePercent() > 40,
						new PrioritySelector(
							Spell.Cast("Heat Blast", ret => Me.BuffCount("Heat Screen") == 3),
							Spell.Cast("Firestorm", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level >= 57),
							Spell.Cast("Flame Burst", ret => Me.HasBuff("Flame Surge")),
							Spell.Cast("Rapid Shots"))),
					    Spell.Cast("Quell",	ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee && !DefaultCombat.MovementDisabled),
							Spell.CastOnGround("Oil Slick", ret => Me.CurrentTarget.BossOrGreater() && Me.CurrentTarget.Distance <= 0.8f),
							Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater()),
							Spell.Cast("Heat Blast", ret => Me.BuffCount("Heat Screen") == 3),
							Spell.Cast("Rocket Punch"),
							Spell.Cast("Rail Shot"),
							Spell.Cast("Firestorm", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level >= 57),
							Spell.Cast("Flame Burst")
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
							Spell.CastOnGround("Deadly Onslaught")
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new PrioritySelector(
							Spell.Cast("Searing Wave", ret => Me.HasBuff("Flame Engine") && Me.CurrentTarget.Distance <= 1f && Me.Level < 57),
							Spell.Cast("Firestorm", ret => Me.Level >= 57),
							Spell.Cast("Flame Sweep"),
							Spell.Cast("Shatter Slug")
							)));
			}
		}
	}
}
