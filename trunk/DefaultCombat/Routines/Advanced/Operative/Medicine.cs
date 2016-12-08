// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Medicine : RotationBase
	{
		public override string Name
		{
			get { return "Operative Medicine"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination"),
					Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Coordination"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 20),
					Spell.Buff("Stim Boost", ret => Me.EnergyPercent <= 70 && !Me.HasBuff("Tactical Advantage")),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 50),
					Spell.Buff("Escape")
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
					Spell.Cast("Shiv", ret => Me.CurrentTarget.Distance <= Distance.Melee),
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
					//Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),		//do we still have this ability even though take cover was removed
					Spell.CastOnGround("Orbital Strike", ret => Targeting.ShouldAoe),
					//Spell.Cast("Explosive Probe", ret => Me.IsInCover()),	//do we still have this ability even though take cover was removed
					Spell.Cast("Hidden Strike", ret => Me.HasBuff("Stealth")),
					Spell.Cast("Backstab"),
					Spell.DoT("Corrosive Dart", "", 16000),
					Spell.Cast("Overload Shot", ret => Me.EnergyPercent >= 70),
					Spell.Cast("Rifle Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
					Spell.Heal("Surgical Probe", 30),
					Spell.Heal("Recuperative Nanotech", on => Tank, 80, ret => Targeting.ShouldAoeHeal),
					Spell.Heal("Kolto Probe", on => Tank, 100, ret => Tank != null && Tank.BuffCount("Kolto Probe") < 2 || Tank.BuffTimeLeft("Kolto Probe") < 6),
					Spell.Heal("Kolto Infusion", 80,	ret => Me.BuffCount("Tactical Advantage") >= 2 && Me.EnergyPercent >= 60 && !HealTarget.HasBuff("Kolto Infusion")),
					Spell.Heal("Surgical Probe", 80, ret => Me.BuffCount("Tactical Advantage") >= 2),
					Spell.Heal("Kolto Injection", 80),
					Spell.Cleanse("Toxin Scan"),
					Spell.Heal("Kolto Probe", 90,	ret => HealTarget.BuffCount("Kolto Probe") < 2 || HealTarget.BuffTimeLeft("Kolto Probe") < 6),
					Spell.Heal("Diagnostic Scan", 90)
					);
			}
		}
	}
}