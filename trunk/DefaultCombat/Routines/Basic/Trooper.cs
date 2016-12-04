// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Trooper : RotationBase
	{
		public override string Name
		{
			get { return "Basic Trooper"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Fortification")
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
					CombatMovement.CloseDistance(Distance.Ranged),
					Spell.Cast("High Impact Bolt"),
					Spell.Cast("Recharge Cells", ret => Me.ResourcePercent() <= 50),
					Spell.Cast("Ion Pulse", ret => Me.ResourcePercent() >= 50),
					Spell.Cast("Hammer Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector());
			}
		}
	}
}
