// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Scrapper : RotationBase
	{
		public override string Name
		{
			get { return "Scoundrel Scrapper"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !Rest.NeedRest())
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Pugnacity", ret => Me.HasBuff("Upper Hand")),
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
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
					Spell.Cast("Back Blast", ret => (Me.IsBehind(Me.CurrentTarget))),
					Spell.Cast("Sucker Punch", ret => Me.HasBuff("Upper Hand") && Me.CurrentTarget.HasDebuff("Vital Shot")),
					Spell.DoT("Vital Shot", "Vital Shot"),
					Spell.Cast("Blood Boiler"),
					Spell.Cast("Bludgeon", ret => Me.Level >= 41),
					Spell.Cast("Blaster Whip", ret => Me.Level < 41),
					Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 75)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Thermal Grenade"),
						Spell.Cast("Blaster Volley", ret => Me.HasBuff("Upper Hand") && Me.CurrentTarget.Distance <= 10f))
					);
			}
		}
	}
}