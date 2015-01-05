using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultCombat.Helpers
{
    public class Global
    {
        public const int EnergyMinimum = 65;
        public const int CellMinimum = 8;
    }

    public class Distance
    {
        public const float Melee = 0.4f;
        public const float MeleeAoE = 0.8f;
        public const float Ranged = 2.8f;
    }

    public class Health
    {
        public const int Max = 95;
        public const int Shield = 90;
        public const int High = 85;
        public const int Mid = 50;
        public const int Low = 35;
        public const int Critical = 15;
        public const int OffHealThreshold = 40;
    }
}