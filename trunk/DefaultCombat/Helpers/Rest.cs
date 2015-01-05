using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using System.Threading;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Helpers
{
    public static class Rest
    {
        private static TorPlayer Me { get { return BuddyTor.Me; } }

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

        public static bool KeepResting()
        {
            int resource = NormalizedResource();
            return !DefaultCombat.MovementDisabled && !Me.InCombat && ((resource < 100 || Me.HealthPercent < 100)
                || (Me.Companion != null && !Me.Companion.IsDead && Me.Companion.HealthPercent < 100));
        }

        public static Composite HandleRest
        {
            get
            {
                using (BuddyTor.Memory.AcquireFrame())
                {
                    return new Action(delegate
                    {

                        if (NeedRest())
                        {
                            while (KeepResting())
                            {
                                if (!Me.IsCasting)
                                    AbilityManager.Cast(Me.RejuvenateAbilityName(), Me);

                                Thread.Sleep(100);
                            }

                            Movement.Move(Buddy.Swtor.MovementDirection.Forward, System.TimeSpan.FromMilliseconds(1));
                            return RunStatus.Success;
                        }

                        return RunStatus.Failure;
                    });
                }
            }
        }

    }
}