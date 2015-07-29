using Buddy.BehaviorTree;
using Buddy.Swtor;

namespace DefaultCombat.Core
{
	public class LockSelector : PrioritySelector
	{
		public LockSelector(params Composite[] children)
			: base(children)
		{
		}

		public override RunStatus Tick(object context)
		{
			using (BuddyTor.Memory.AcquireFrame())
			{
				return base.Tick(context);
			}
		}
	}
}