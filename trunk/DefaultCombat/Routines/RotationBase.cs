using Buddy.BehaviorTree;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Core;

//using Movement = DefaultCombat.Core.Movement;

namespace DefaultCombat.Routines
{
    public abstract class RotationBase
    {
        protected static TorPlayer Me { get { return BuddyTor.Me; } }
        
        public static TorCharacter Tank {get { return Targeting.Tank; }}
        public static TorCharacter HealTarget { get { return Targeting.HealTarget; } }

        public abstract string Name { get; }
        public abstract Composite Buffs { get; }
        public abstract Composite Cooldowns { get; }
        public abstract Composite SingleTarget { get; }
        public abstract Composite AreaOfEffect { get; }
    }
}
