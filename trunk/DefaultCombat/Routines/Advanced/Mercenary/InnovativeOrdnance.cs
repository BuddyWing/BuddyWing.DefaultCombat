// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class InnovativeOrdnance : RotationBase
	{
		public override string Name
		{
			get { return "Mercenary Innovative Ordnance"; }
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
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 50),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
					Spell.Cast("Responsive Safeguards", ret => Me.HealthPercent <= 20)
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
					new Decorator(ret => Me.ResourcePercent() > 40,
						new PrioritySelector(
							Spell.Cast("Rapid Shots")
							)),

					//Rotation
					Spell.Cast("Disabling Shot",
						ret =>
							Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee && !DefaultCombat.MovementDisabled),
					Spell.DoT("Incendiary Missile", "", 12000),
					Spell.Cast("Thermal Detonator"),
					Spell.Cast("Electro Net"),
					Spell.Cast("Unload"),
					Spell.Cast("Power Shot", ret => Me.HasBuff("Speed to Burn")),
					Spell.Cast("Mag Shot", ret => Me.HasBuff("Innovative Particle Accelerator")),
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
						Spell.CastOnGround("Death from Above"),
						Spell.Cast("Fusion Missle", ret => Me.HasBuff("Thermal Sensor Override")),
						Spell.Cast("Explosive Dart"))
					);
			}
		}
	}
}
