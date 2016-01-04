// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Agent : RotationBase
	{
		public override string Name
		{
			get { return "Basic Agent"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination")
					);
			}
		}

		public override Composite Cooldowns
		{
			get { return new LockSelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.DoT("Corrosive Dart", "", 15000),
					Spell.Cast("Shiv", ret => Me.CurrentTarget.Distance <= Distance.Melee),
					Spell.Cast("Explosive Probe"),
					Spell.Cast("Snipe"),
					Spell.Cast("Rifle Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Fragmentation Grenade", ret => Me.CurrentTarget.Distance <= Distance.Ranged)
						));
			}
		}
	}
}