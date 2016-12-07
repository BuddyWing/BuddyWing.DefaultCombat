﻿// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Arsenal : RotationBase
	{
		public override string Name
		{
			get { return "Mercenary Arsenal"; }
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
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 70),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
					Spell.Buff("Responsive Safeguards", ret => Me.HealthPercent <= 50)
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

					//Rotation
					Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() > 40),
					Spell.Cast("Blazing Bolts", ret => Me.HasBuff("Barrage") && Me.Level >= 57),
					Spell.Cast("Unload", ret => Me.Level < 57),
					Spell.Cast("Heatseeker Missiles", ret => Me.CurrentTarget.HasDebuff("Heat Signature")),
					Spell.Cast("Rail Shot", ret => Me.BuffCount("Tracer Lock") == 5),
					Spell.Cast("Electro Net"),
					Spell.Cast("Priming Shot"),
					Spell.Cast("Tracer Missile"),
					Spell.Cast("Rapid Shots")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Buff("Thermal Sensor Override"),
						Spell.Buff("Power Surge"),
						Spell.CastOnGround("Death from Above"),
						Spell.Cast("Fusion Missile", ret => Me.ResourcePercent() <= 10 && Me.HasBuff("Power Surge")),
						Spell.CastOnGround("Sweeping Blasters", ret => Me.ResourcePercent() <= 10))
					);
			}
		}
	}
}
