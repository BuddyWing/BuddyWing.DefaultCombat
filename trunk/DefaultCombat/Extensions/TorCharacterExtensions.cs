using System.Collections.Generic;
using System.Linq;
using Buddy.Swtor.Objects;

namespace DefaultCombat.Extensions
{
    public static class TorCharacterExtensions
    {
        private static readonly IReadOnlyList<string> _dispellableDebuffs = new List<string>
        {
            "Hunting Trap",
            "Burning (Physical)"
        };

        public static bool ShouldDispel(this TorCharacter target)
        {
            return target != null && _dispellableDebuffs.Any(target.HasDebuff);
        }
    }
}
