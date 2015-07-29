// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Ruffian : RotationBase
	{
		public override string Name
		{
			get { return "Scoundrel Ruffian"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Pugnacity", ret => !Me.HasBuff("Upper Hand") && Me.InCombat),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 50)
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

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new LockSelector(
							Spell.Cast("Flurry of Bolts")
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
					Spell.Cast("Brutal Shots",
						ret =>
							Me.CurrentTarget.HasDebuff("Bleeding (Vital Shot)") && Me.CurrentTarget.HasDebuff("Bleeding (Tech)") &&
							Me.HasBuff("Upper Hand")),
					Spell.Cast("Sanguinary Shot",
						ret => Me.CurrentTarget.HasDebuff("Bleeding (Vital Shot)") && Me.CurrentTarget.HasDebuff("Bleeding (Tech)")),
					Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
					Spell.DoT("Shrap Bomb", "Bleeding (Tech)"),
					Spell.Cast("Blaster Whip", ret => Me.BuffCount("Upper Hand") < 2 || Me.BuffTimeLeft("Upper Hand") < 6),
					Spell.Cast("Point Blank Shot", ret => Me.Level >= 57),
					Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget) && Me.Level < 57),
					Spell.Cast("Quick Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.DoT("Shrap Bomb", "Bleeding (Tech)"),
						Spell.Cast("Thermal Grenade"),
						Spell.Cast("Blaster Volley", ret => Me.HasBuff("Upper Hand"))
						));
			}
		}
	}
}