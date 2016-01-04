// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Navigation;
using Buddy.Swtor;

namespace DefaultCombat.Core
{
	public class CombatMovement
	{
		public static Composite CloseDistance(float range)
		{
			return new Decorator(ret => !DefaultCombat.MovementDisabled && BuddyTor.Me.CurrentTarget != null,
				new PrioritySelector(
					new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance < range,
						new Action(delegate
						{
							Navigator.MovementProvider.StopMovement();
							return RunStatus.Failure;
						})),
					new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance >= range,
						CommonBehaviors.MoveAndStop(location => BuddyTor.Me.CurrentTarget.Position, range, true)),
					new Action(delegate { return RunStatus.Failure; })));
		}
	}
}