// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows.Forms;
using Buddy.Common;
using DefaultCombat.Core;

namespace DefaultCombat.Helpers
{
	public static class CombatHotkeys
	{
		public static bool EnableAoe;
		public static bool PauseRotation;

		public static void Initialize()
		{
			EnableAoe = true;
			PauseRotation = false;

			Hotkeys.RegisterHotkey("Toggle AOE (F7)", ChangeAOE, Keys.F7);
			Logger.Write("[Hot Key][F7] Toggle AOE");

			Hotkeys.RegisterHotkey("Pause Rotation (F8)", ChangePause, Keys.F8);
			Logger.Write("[Hot Key][F8] Load UI");

			Hotkeys.RegisterHotkey("Set Tank (F12)", Targeting.SetTank, Keys.F12);
			Logger.Write("[Hot Key][F12] Set Tank");
		}

		private static void ChangeAOE()
		{
			if (EnableAoe)
			{
				Logger.Write("AOE Disabled");
				EnableAoe = false;
			}
			else
			{
				Logger.Write("AOE Enabled");
				EnableAoe = true;
			}
		}

		private static void ChangePause()
		{
			if (PauseRotation)
			{
				Logger.Write("Rotation Resumed");
				PauseRotation = false;
			}
			else
			{
				Logger.Write("Rotation Paused");
				PauseRotation = true;
			}
		}
	}
}