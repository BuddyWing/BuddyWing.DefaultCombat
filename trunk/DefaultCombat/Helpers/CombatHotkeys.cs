﻿// Copyright (C) 2011-2017 Bossland GmbH
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
        public static bool EnableInterrupts;
        public static bool EnableCharge;
        public static bool EnableSolo;
        public static bool EnableHK55;

        public static void Initialize()
        {
            EnableAoe = true;
            PauseRotation = false;
            EnableInterrupts = true;
            EnableCharge = true;
            EnableSolo = false;
            EnableHK55 = false;

            //F9 and F10 are reservered for internal commands

            Hotkeys.RegisterHotkey("Toggle HK55 (F4)", ChangeHK55, Keys.F4);
            Logger.Write("[Hot Key][F4] Toggle HK55");

            Hotkeys.RegisterHotkey("Toggle Interrupts (F5)", ChangeInterrupts, Keys.F5);
            Logger.Write("[Hot Key][F5] Toggle Interrupts");

            Hotkeys.RegisterHotkey("Toggle Charge (F6)", ChangeCharge, Keys.F6);
            Logger.Write("[Hot Key][F6] Toggle Charge");

            Hotkeys.RegisterHotkey("Toggle AOE (F7)", ChangeAoe, Keys.F7);
            Logger.Write("[Hot Key][F7] Toggle AOE");

            Hotkeys.RegisterHotkey("Pause Rotation (F8)", ChangePause, Keys.F8);
            Logger.Write("[Hot Key][F8] Pause Rotation");

            Hotkeys.RegisterHotkey("Toggle Solo (F11)", ChangeSolo, Keys.F11);
            Logger.Write("[Hot Key][F11] Toggle Solo Mode");

            Hotkeys.RegisterHotkey("Set Tank (F12)", Targeting.SetTank, Keys.F12);
            Logger.Write("[Hot Key][F12] Set Tank");
        }

        private static void ChangeAoe()
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

        private static void ChangeCharge()
        {
            if (EnableCharge)
            {
                Logger.Write("Charge Disabled");
                EnableCharge = false;
            }
            else
            {
                Logger.Write("Charge Enabled");
                EnableCharge = true;
            }
        }

        private static void ChangeInterrupts()
        {
            if (EnableInterrupts)
            {
                Logger.Write("Interrupts Disabled");
                EnableInterrupts = false;
            }
            else
            {
                Logger.Write("Interrupts Enabled");
                EnableInterrupts = true;
            }
        }

        private static void ChangeSolo()
        {
            if (EnableSolo)
            {
                Logger.Write("Solo Mode Disabled");
                EnableSolo = false;
            }
            else
            {
                Logger.Write("Solo Mode Enabled");
                EnableSolo = true;
            }
        }

        private static void ChangeHK55()
        {
            if (EnableHK55)
            {
                Logger.Write("HK-55 Mode Disabled");
                EnableHK55 = false;
            }
            else
            {
                Logger.Write("HK-55 Mode Enabled");
                EnableHK55 = true;
            }
        }
    }
}