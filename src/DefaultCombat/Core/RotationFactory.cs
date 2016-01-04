// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System;
using System.Reflection;
using Buddy.Swtor;
using DefaultCombat.Routines;

namespace DefaultCombat.Core
{
	public class RotationFactory
	{
		public RotationBase Build(string name)
		{
			//Set the basic class as the rotation if char has no advanced class
			if (BuddyTor.Me.AdvancedClass == AdvancedClass.None)
			{
				name = BuddyTor.Me.CharacterClass.ToString();
			}

			if (name == "Rage" && BuddyTor.Me.AdvancedClass == AdvancedClass.Marauder)
			{
				name = "Fury";
			}

			if (name == "Focus" && BuddyTor.Me.AdvancedClass == AdvancedClass.Sentinel)
			{
				name = "Concentration";
			}

			if (name == "DirtyFighting" && BuddyTor.Me.AdvancedClass == AdvancedClass.Scoundrel)
			{
				name = "Ruffian";
			}

			if (name == "Balance" && BuddyTor.Me.AdvancedClass == AdvancedClass.Shadow)
			{
				name = "Serenity";
			}

			if (name == "CombatMedic" && BuddyTor.Me.AdvancedClass == AdvancedClass.Mercenary)
			{
				name = "Bodyguard";
			}

			if (name == "Firebug" && BuddyTor.Me.AdvancedClass == AdvancedClass.Powertech)
			{
				name = "Pyrotech";
			}

			var ns = "DefaultCombat.Routines";
			var assembly = Assembly.GetExecutingAssembly();
			var type = assembly.GetType(ns + "." + name);
			var instance = Activator.CreateInstance(type);
			return (RotationBase) instance;
		}
	}
}