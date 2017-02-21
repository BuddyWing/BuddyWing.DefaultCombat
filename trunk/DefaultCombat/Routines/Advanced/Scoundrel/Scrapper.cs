// Copyright (C) 2011-2017 Bossland GmbH
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
				return new PrioritySelector(
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
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
				return new PrioritySelector(
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

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new PrioritySelector(
							Spell.Cast("Flurry of Bolts")
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
					Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget)),
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
					new PrioritySelector(
						Spell.Cast("Thermal Grenade"),
						Spell.Cast("Bushwhack", ret => !Me.HasBuff("Upper Hand")),
					  Spell.Cast("Lacerating Blast")
					));
			}
		}
	}
}