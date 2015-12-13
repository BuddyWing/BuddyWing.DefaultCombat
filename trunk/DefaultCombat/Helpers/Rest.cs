// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Core;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Helpers
{
	public static class Rest
	{
		private static TorPlayer Me { get { return BuddyTor.Me; } }
		private static int _swtorpid;                                                        // For Mounting and Spell Cancel/Stop
		private static IntPtr _swtorhWnd;
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);  // Used for mounting/keysend
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr HWnd);
		public static int NormalizedResource()
		{
			if (Me.AdvancedClass == AdvancedClass.None)
			{
				switch (Me.Class)
				{
					//Add cases needed for reverse basic classes
					case CharacterClass.BountyHunter:
						return 100 - (int)Me.ResourcePercent();
					case CharacterClass.Warrior:
						return 100;
					case CharacterClass.Knight:
						return 100;
					default:
						return (int)Me.ResourcePercent();
				}
			}
			else
			{
				switch (Me.AdvancedClass)
				{
					//Add cases needed for reverse Advance classes
					case AdvancedClass.Mercenary:
						return 100 - (int)Me.ResourcePercent();
					case AdvancedClass.Powertech:
						return 100 - (int)Me.ResourcePercent();
					case AdvancedClass.Juggernaut:
						return 100;
					case AdvancedClass.Marauder:
						return 100;
					case AdvancedClass.Guardian:
						return 100;
					case AdvancedClass.Sentinel:
						return 100;
					default:
						return (int)Me.ResourcePercent();
				}
			}
		}

		public static bool NeedRest()
		{
			int resource = NormalizedResource();
			return !DefaultCombat.MovementDisabled && !Me.InCombat && ((resource < 50 || Me.HealthPercent < 90)
				|| (Me.Companion != null && !Me.Companion.IsDead && Me.Companion.HealthPercent < 90));
		}

		public static void SetProcessAttrs()
		{
			int TorMem = 0;
			foreach (Process proc in Process.GetProcesses())
				if (proc.ProcessName.Contains("swtor") && proc.MainWindowTitle.Contains("Star Wars"))

					if (proc.PrivateMemorySize64 > TorMem)
					{

						_swtorpid = proc.Id;
						TorMem = (int)proc.NonpagedSystemMemorySize64;
						_swtorhWnd = proc.MainWindowHandle;
					}
		}
		public static bool KeepResting()
		{
			int resource = NormalizedResource();
			return !DefaultCombat.MovementDisabled && !Me.InCombat && ((resource < 100 || Me.HealthPercent < 100)
				|| (Me.Companion != null && !Me.Companion.IsDead && Me.Companion.HealthPercent < 100));
		}

		private static Composite ReviveCompanion
		{
			get
			{
				return new PrioritySelector(
				   new Decorator(ret => Me.Companion != null && Me.Companion.IsDead && Me.CompanionUnlocked > 0,
						new PrioritySelector(
							Spell.WaitForCast(),
							CommonBehaviors.MoveAndStop(location => Me.Companion.Position, 0.2f, true),
							Spell.Cast("Revive Companion", on => Me.Companion, when => Me.Companion.Distance <= 0.2f)
							))
				   );
			}
		}

		private static Composite Rejuvenate
		{
			get
			{
				using (BuddyTor.Memory.AcquireFrame())
				{
					return new Action(delegate
					{

						if (NeedRest())
						{
							SetProcessAttrs();
							if (BuddyTor.Me.IsMoving) Input.MoveStopAll();
							while (KeepResting())
							{
								if (!Me.IsCasting)
									AbilityManager.Cast(Me.RejuvenateAbilityName(), Me);

								Thread.Sleep(100);
							}
                            if (Me.IsCasting)
                            {
                                SendMessage(_swtorhWnd, (int)0x100, (IntPtr)(char)0x1b, (IntPtr)0);
                                Thread.Sleep(100);
                            }
                            return RunStatus.Success;
						}

						return RunStatus.Failure;
					});
				}
			}
		}

		public static Composite HandleRest
		{
			get
			{
				return new PrioritySelector(
					ReviveCompanion,
					Rejuvenate
					);
			}
		}

	}
}