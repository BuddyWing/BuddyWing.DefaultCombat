// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Diagnostics;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.Swtor;
using log4net;

namespace DefaultCombat.Helpers
{
	public class InventoryManagerItem
	{
		public delegate T Selection<out T>(object context);
		private ILog _log = Log.Get();

		private readonly Stopwatch Timer;
		public int CoolDown;
		public string ItemName;

		public InventoryManagerItem(string name, int cd)
		{
			ItemName = name;
			CoolDown = cd;
			Timer = new Stopwatch();
			_log.Info(name + "  Created!");
		}

		public Composite UseItem(Selection<bool> reqs = null)
		{
			return new Decorator(ret => (reqs == null || reqs(ret)),
				new Action(delegate
				{
					if (Timer.IsRunning && Timer.Elapsed.TotalSeconds < CoolDown)
						return RunStatus.Failure;

					if (!Timer.IsRunning)
						Timer.Start();

					foreach (var o in BuddyTor.Me.InventoryEquipment)
					{
						if (o.Name.Contains(ItemName))
						{
							o.Use();
							o.Interact();
							Timer.Stop();
							Timer.Reset();
							Timer.Start();
							return RunStatus.Success;
						}
					}
					return RunStatus.Failure;
				}));
		}
	}
}