// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license


using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class AdvancedPrototype : RotationBase
	{
		public override string Name
		{
			get { return "PowerTech Advanced Prototype"; }
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
					Spell.Buff("Shoulder Cannon"),
					Spell.Buff("Thermal Sensor Override", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Explosive Fuel", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 60),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
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
					new Decorator(ret => Me.ResourcePercent() < 50,
						new PrioritySelector(
							Spell.Cast("Rail Shot", ret => Me.HasBuff("Prototype Particle Accelerator")),
							Spell.Cast("Rapid Shots")
							)),

					//Rotation
					Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Energy Burst", ret => Me.BuffCount("Energy Lode") == 4),
					Spell.Cast("Rail Shot",	ret => Me.CurrentTarget.HasDebuff("Bleeding (Retractable Blade)") && Me.HasBuff("Prototype Particle Accelerator")),
					Spell.DoT("Retractable Blade", "Bleeding (Retractable Blade)"),
					Spell.Cast("Thermal Detonator"),
					Spell.Cast("Rocket Punch"),
					Spell.Cast("Magnetic Blast", ret => Me.Level >= 26),
					Spell.Cast("Flame Burst", ret => Me.Level < 26)
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
							Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
							Spell.Cast("Searing Wave"),
							Spell.Cast("Flame Sweep"),
							Spell.Cast("Shatter Slug")
							)));
			}
		}
	}
}
