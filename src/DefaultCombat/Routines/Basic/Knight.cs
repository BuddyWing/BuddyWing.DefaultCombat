// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Knight : RotationBase
	{
		public override string Name
		{
			get { return "Basic Knight"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Shii-Cho Form")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Saber Ward", ret => Me.HealthPercent <= 70)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Force Leap",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance > 1f && Me.CurrentTarget.Distance <= 3f),
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Master Strike"),
					Spell.Cast("Blade Storm"),
					Spell.Cast("Riposte"),
					Spell.Cast("Slash", ret => Me.ActionPoints >= 7),
					Spell.Cast("Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Force Sweep", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE))
					);
			}
		}
	}
}