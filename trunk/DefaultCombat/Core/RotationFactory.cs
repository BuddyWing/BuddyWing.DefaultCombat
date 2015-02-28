using Buddy.Swtor;
using DefaultCombat.Routines;
using System;
using System.Reflection;

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

            string ns = "DefaultCombat.Routines";
            var assmebly = Assembly.GetExecutingAssembly();
            var type = assmebly.GetType(ns + "." + name);
            var instance = Activator.CreateInstance(type);
            return (RotationBase)instance;
        } 
    }
}
