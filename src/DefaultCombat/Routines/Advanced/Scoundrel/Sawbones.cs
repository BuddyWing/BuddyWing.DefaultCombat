// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Sawbones : RotationBase
	{
		public override string Name
		{
			get { return "Scoundrel Sawbones"; }
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
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 20),
					Spell.Buff("Pugnacity", ret => Me.EnergyPercent <= 70 && Me.BuffCount("Upper Hand") < 3),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 50),
					Spell.Buff("Escape", ret => Me.IsCrowdControlled())
					);
			}
		}

		//DPS
		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
					Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget)),
					Spell.Cast("Blaster Whip"),
					Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Bleeding (Vital Shot)")),
					Spell.Cast("Charged Burst", ret => Me.EnergyPercent >= 70),
					Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 70),
					Spell.Cast("Flurry of Bolts")
					);
			}
		}

		//Healing
		public override Composite AreaOfEffect
		{
			get
			{
				return new LockSelector(
					Spell.Heal("Kolto Cloud", on => Tank, 80, ret => Tank != null && Targeting.ShouldAoeHeal),
					Spell.Heal("Slow-release Medpac", on => Tank, 100, ret => Tank != null && Tank.BuffCount("Slow-release Medpac") < 2),
					Spell.Heal("Kolto Pack", 80,
						ret =>
							Me.BuffCount("Upper Hand") >= 2 && Me.EnergyPercent >= 60 && HealTarget != null &&
							!HealTarget.HasMyBuff("Kolto Pack")),
					Spell.Heal("Emergency Medpac", 90,
						ret => Me.BuffCount("Upper Hand") >= 2 && HealTarget != null && HealTarget.BuffCount("Slow-release Medpac") == 2),
					Spell.Heal("Underworld Medicine", 80),
					Spell.Cleanse("Triage"),
					Spell.Heal("Slow-release Medpac", 90, ret => HealTarget != null && HealTarget.BuffCount("Slow-release Medpac") < 2),
					Spell.Heal("Diagnostic Scan", 95)
					);
			}
		}
	}
}