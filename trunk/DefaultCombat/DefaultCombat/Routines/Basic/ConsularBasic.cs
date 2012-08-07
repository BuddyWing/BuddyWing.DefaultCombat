using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.CommonBot.Logic;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines.Basic
{
    public class Consular
    {
        [Class(CharacterClass.Consular)]
        [Behavior(BehaviorType.Combat)]
        public static Composite ConsularCombat()
        {
            //Neo93 29.04.2012
            return new PrioritySelector(
                Spell.WaitForCast(),
                Movement.StopInRange(0.4f),
                //Generel
                Spell.Cast("Force of Will", castWhen => BuddyTor.Me.IsStunned),//Insignia/2m CD
                Spell.Cast("Force Potency", castWhen => BuddyTor.Me.InCombat && AbilityManager.CanCast("Project", BuddyTor.Me.CurrentTarget)),

                //CC
                /*Spell.Cast("Force Lift", onUnit => 
                {
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    return
                        Helpers.Targets.FirstOrDefault(
                            t =>
                            t != previousTarget && (t.Toughness != CombatToughness.Player)) ??
                        Helpers.Targets.FirstOrDefault(t => t != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3),*/
                
                //Offensive
                Spell.Cast("Project", castWhen => BuddyTor.Me.ResourceStat >= 45),//-45 Force/Range/Instant
                Spell.Cast("Telekinetic Throw", castWhen => (BuddyTor.Me.ResourceStat >= 30 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f)),//-30 Force/Range/3s Channel
                Spell.WaitForCast(),
                Spell.Cast("Force Stun", castWhen => BuddyTor.Me.ResourceStat >= 20),
                Spell.Cast("Force Wave", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .3 && !o.IsDead) >= 3),
                Spell.Cast("Double Strike", castWhen => BuddyTor.Me.ResourceStat >= 45 && BuddyTor.Me.CurrentTarget.Distance <= 0.4f && !AbilityManager.CanCast("Project", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Telekinetic Throw", BuddyTor.Me.CurrentTarget)),//-25 Force/Melee
                Spell.Cast("Saber Strike", castWhen => BuddyTor.Me.CurrentTarget.Distance <= 0.4f),//0 Force/Melee
                
                
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 0.4f)
                );
        }

        [Class(CharacterClass.Consular)]
        [Behavior(BehaviorType.Pull)]
        public static Composite ConsularPull()
        {
            return ConsularCombat();
        }
    }
}