// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Combat : RotationBase
	{
		public override string Name
		{
			get { return "Sentinel Combat"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Ataru Form"),
					Spell.Buff("Force Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Resolute"),
					Spell.Buff("Rebuke", ret => Me.HealthPercent <= 90),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 70),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Valorous Call", ret => Me.BuffCount("Centering") < 5),
					Spell.Buff("Zen", ret => Me.CurrentTarget.Distance <= 0.4f)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Saber Throw",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Leap",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Dual Saber Throw",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Dispatch", ret => Me.HasBuff("Hand of Justice") || Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Precision", ret => Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Blade Dance", ret => Me.HasBuff("Precision")),
					Spell.Cast("Clashing Blast", ret => Me.HasBuff("Opportune Attack") && Me.Level >= 57),
					Spell.Cast("Blade Storm", ret => Me.HasBuff("Opportune Attack") && Me.Level < 57),
					Spell.Cast("Blade Rush"),
					Spell.Cast("Slash", ret => Me.ActionPoints >= 7 && Me.Level < 26),
					Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 7),
					Spell.Cast("Strike", ret => Me.ActionPoints <= 10)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new LockSelector(
						Spell.Cast("Force Sweep"),
						Spell.Cast("Cyclone Slash")
						));
			}
		}
	}
}