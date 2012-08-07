using System.Linq;
using Buddy.Swtor;

namespace DefaultCombat.Routines
{
    public static class Global
    {
        public static bool IsInCover { get { return (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")); } }
        public static bool IsInGroup { get { return Helpers.Group.Count() > 2; } }
        public const int EnergyMin = 65;
        public const int CellMin = 8;
        public const float RangeDist = 2.8f;
        public const float MeleeDist = 0.35f;
        public const int FullHealth = 100;
        public const int HighHealth = 80;
        public const int MidHealth = 50;
        public const int LowHealth = 30;
        public const int OhShit = 15;
        public const int HealthShield = 90;
        public const int HealthCritical = 40;
    }
}
