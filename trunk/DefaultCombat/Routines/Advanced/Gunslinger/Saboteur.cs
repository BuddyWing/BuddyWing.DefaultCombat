// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Saboteur : RotationBase
	{
		public override string Name
		{
			get { return "Gunslinger Saboteur"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Escape"),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 50),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Smuggler's Luck"),
					Spell.Buff("Illegal Mods")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 60),

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),

					//Rotation
					Spell.Cast("Crouch", ret => !Me.IsInCover()),
					Spell.Cast("Flourish Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
					Spell.Cast("Speed Shot", ret => Me.IsInCover()),
					Spell.Cast("Incendiary Grenade"),
					Spell.DoT("Shock Charge", "Shock Charg"),
					Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
					Spell.Cast("Sabotage Charge", ret => Me.IsInCover()),
					Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Aimed Shot", ret => Me.IsInCover()),
					Spell.Cast("Charged Burst", ret => Me.IsInCover()),
					Spell.Cast("Quick Shot", ret => !Me.IsInCover())
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("XS Freighter Flyby"),
						Spell.Cast("Incendiary Grenade"),
						Spell.Cast("Thermal Grenade")
						));
			}
		}
	}
}