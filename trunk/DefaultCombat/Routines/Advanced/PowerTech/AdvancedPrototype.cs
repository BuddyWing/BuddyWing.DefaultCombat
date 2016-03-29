// Copyright (C) 2011-2016 Bossland GmbH
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
					Spell.Buff("High Energy Gas Cylinder"),
					Spell.Buff("Hunter's Boon")
					);
			}
		}


		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Determination"),
					Spell.Buff("Shoulder Cannon"),
					Spell.Buff("Thermal Sensor Override", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Explosive Fuel", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 60),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30)
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

					//Low Energy
					new Decorator(ret => Me.ResourcePercent() < 50,
						new PrioritySelector(
							Spell.Cast("Rail Shot", ret => Me.HasBuff("Prototype Particle Accelerator")),
							Spell.Cast("Rapid Shots")
							)),

					//Rotation
					Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Energy Burst", ret => Me.BuffCount("Energy Lode") == 4),
					Spell.Cast("Rail Shot",
						ret => Me.CurrentTarget.HasDebuff("Bleeding (Retractable Blade)") && Me.HasBuff("Prototype Particle Accelerator")),
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
							Spell.CastOnGround("Death from Above"),
							Spell.Cast("Explosive Dart", ret => Me.CurrentTarget.HasDebuff("Bleeding (Retractable Blade)"))
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new PrioritySelector(
							Spell.Cast("Flame Thrower"),
							Spell.Cast("Flame Sweep"))
						));
			}
		}
	}
}