using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Buddy.Swtor;
using Buddy.Common;
using Buddy.CommonBot;

namespace DefaultCombat.Dynamics
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int thePriority)
        {
            PriorityLevel = thePriority;
        }

        public int PriorityLevel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class ClassAttribute : Attribute
    {
        // I know this is .NET 4.0. But for some reason, this is throwing attribute format exceptions when used with a default
        // argument. So this specific ctor overload is to prevent issues finding the correct class/advclass combos.
        public ClassAttribute(CharacterClass specificClass, AdvancedClass advancedClass, SkillTreeId charSpec/*,CharacterSpec*/)
        {
            SpecificClass = specificClass;
            AdvancedClass = advancedClass;
            CharSpec = charSpec;
        }
        public ClassAttribute(CharacterClass specificClass):this(specificClass,0,0)
        {
        }

        public CharacterClass SpecificClass { get; private set; }
        public AdvancedClass AdvancedClass { get; private set; }
        public SkillTreeId CharSpec { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class IgnoreBehaviorCountAttribute : Attribute
    {
        public IgnoreBehaviorCountAttribute(BehaviorType type)
        {
            Type = type;
        }

        public BehaviorType Type { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class BehaviorAttribute : Attribute
    {
        public BehaviorAttribute(BehaviorType type)
        {
            Type = type;
        }

        public BehaviorType Type { get; private set; }
    }

}
