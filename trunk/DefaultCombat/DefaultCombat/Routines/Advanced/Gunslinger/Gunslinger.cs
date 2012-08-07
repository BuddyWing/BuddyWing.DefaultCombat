using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class Gunslinger
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.None)]
        public static Composite GunslingerCombat()
        {
            return new PrioritySelector(
                Movement.StopInRange(2.8f),

				// Cover so we can use our stuff
                // Spell.Cast("Take Cover", ret => !BuddyTor.Me.HasBuff("Cover") && !BuddyTor.Me.HasBuff("Crouch")),
                Spell.Cast("Crouch", ret => !BuddyTor.Me.HasBuff("Cover")),

				// AoE Grenade, High Damage
                Spell.Cast("Thermal Grenade", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
				// Medium DPS Grenade
                Spell.Cast("Sabotage Charge", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
				// add usage
                Spell.Cast("Aimed Shot", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
				// Medium DPS Shot
                Spell.Cast("Charged Burst", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
				// Medium DPS shot
                Spell.Cast("Quick Slot", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
				// Low DPS shot
                Spell.Cast("Flurry of Bolts", ret => BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
				
              Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f)
            );
        }
    }
}

