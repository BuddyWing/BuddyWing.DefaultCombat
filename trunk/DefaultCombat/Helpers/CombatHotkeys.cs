using Buddy.Common;
using DefaultCombat.Core;
using System.Windows.Forms;

namespace DefaultCombat.Helpers
{
    public static class CombatHotkeys
    {
        public static bool EnableAOE;
        public static bool PauseRotation;

        public static void Initialize()
        {
            EnableAOE = true;
            PauseRotation = false;

            Hotkeys.RegisterHotkey("Toggle AOE (F7)", () => { ChangeAOE(); }, Keys.F7);
            Logger.Write("[Hot Key][F7] Toggle AOE");

            Hotkeys.RegisterHotkey("Pause Rotation (F8)", () => { ChangePause(); }, Keys.F8);
            Logger.Write("[Hot Key][F8] Load UI");

            Hotkeys.RegisterHotkey("Set Tank (F12)", () => { Targeting.SetTank(); }, Keys.F12);
            Logger.Write("[Hot Key][F12] Set Tank");
        }

        private static void ChangeAOE()
        {
            if (EnableAOE)
            {
                Logger.Write("AOE Disabled");
                EnableAOE = false;
            }
            else
            {
                Logger.Write("AOE Enabled");
                EnableAOE = true;
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
