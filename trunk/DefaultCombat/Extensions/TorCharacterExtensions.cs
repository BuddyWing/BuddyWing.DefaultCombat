using Buddy.Swtor.Objects;

namespace DefaultCombat.Extensions
{
    public static class TorCharacterExtensions
    {
        public static bool ShouldDispel(this TorCharacter target, string debuffName)
        {
            if (target == null)
                return false;

            return target.HasDebuff(debuffName);
        }
    }
}
