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
            if (name == "Rage" && BuddyTor.Me.AdvancedClass == AdvancedClass.Marauder)
            {
                name = "Fury";
            }

            if (name == "Focus" && BuddyTor.Me.AdvancedClass == AdvancedClass.Sentinel)
            {
                name = "Concentration";
            }

            string ns = "DefaultCombat.Routines";
            var assmebly = Assembly.GetExecutingAssembly();
            var type = assmebly.GetType(ns + "." + name);
            var instance = Activator.CreateInstance(type);
            return (RotationBase)instance;
        } 
    }
}
