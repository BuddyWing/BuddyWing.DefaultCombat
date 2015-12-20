// Copyright (C) 2011-2015 Bossland GmbH
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
				return new LockSelector(
					Spell.Buff("Determination"),
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
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() > 40,
						new LockSelector(
							Spell.Cast("Rail Shot", ret => Me.HasBuff("Prototype Particle Accelerator")),
							Spell.Cast("Rapid Shots")
							)),
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
				return new LockSelector(
					new Decorator(ret => Targeting.ShouldAoe,
						new LockSelector(
							Spell.CastOnGround("Death from Above"),
							Spell.Cast("Explosive Dart", ret => Me.CurrentTarget.HasDebuff("Bleeding (Retractable Blade)"))
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new LockSelector(
							Spell.Cast("Flame Thrower"),
							Spell.Cast("Flame Sweep"))
						));
			}
		}
	}
}