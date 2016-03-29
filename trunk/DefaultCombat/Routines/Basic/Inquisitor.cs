// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Inquisitor : RotationBase
	{
		public override string Name
		{
			get { return "Basic Inquisitor"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Mark of Power")
					);
			}
		}

		public override Composite Cooldowns
		{
			get { return new PrioritySelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Force Lightning"),
					Spell.Cast("Shock", ret => Me.Force > 75),
					Spell.Cast("Thrash", ret => Me.Force > 70),
					Spell.Cast("Saber Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Overload", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE)
						));
			}
		}
	}
}