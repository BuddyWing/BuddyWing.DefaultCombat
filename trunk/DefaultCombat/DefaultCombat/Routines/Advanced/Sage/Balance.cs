using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Swtor;
using DefaultCombat.Dynamics;
using DefaultCombat;

namespace DefaultCombat.Routines
{
    // Pios - 10/05
    public static class SageBalance
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageBalance)]
        public static Composite SageBalancePull()
        {
            return new PrioritySelector(
               Spell.Cast("Mind Crush"),
               Spell.Cast("Weaken Mind"),
               Spell.Cast("Force Stun"),
               Spell.Cast("Project"),
               SageBalanceCombat()
               );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageBalance)]
        public static Composite SageBalanceCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Unbreakable Will", castWhen => BuddyTor.Me.IsStunned),
                CommonBuffsAndHealing(),
                Spell.WaitForCast(),
                Spell.Cast("Mind Crush",
                           ret => BuddyTor.Me.HasBuff("Presence of Mind") &&
                                  !BuddyTor.Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                Spell.Cast("Weaken Mind", onUnit =>
                {
                    //Get our dots up.
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    if (Helpers.Targets.Count() == 1)
                        return Helpers.Targets.FirstOrDefault(targ => !targ.HasDebuff("Weaken Mind (Force)"));
                    return
                        Helpers.Targets.FirstOrDefault(
                            targ =>
                            targ != previousTarget &&
                            (targ.Toughness != CombatToughness.Player) &&
                                // Dont go all pvp on us.
                            !targ.IsDead &&
                            !targ.HasDebuff("Weaken Mind (Force)")) ??
                        Helpers.Targets.FirstOrDefault(targ => targ != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3 && Helpers.Targets.Count(wh => wh.HasDebuff("Weaken Mind (Force)")) <= 2),
                Spell.Cast("Sever Force",
                           ret => !BuddyTor.Me.CurrentTarget.HasDebuff("Sever Force (Force)")),
                Spell.CastOnGround("Force in Balance",
                                   ret => BuddyTor.Me.CurrentTarget.Distance <= 0.5f &&
                                          BuddyTor.Me.ResourceStat >= 100,
                                   location => BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Force Potency"), // Lets get recklessness up.
                new Decorator( // Only cast shock if we are moving.
                    ret => BuddyTor.Me.IsMoving,
                    Spell.Cast("Project")),
                Spell.Cast("Telekinetic Throw"), // 3 Second channel
                Spell.WaitForCast(),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 2.8f));
        }

        /// <summary>
        /// Commons buffs and healing.
        /// </summary>
        /// <returns></returns>
        public static Composite CommonBuffsAndHealing()
        {
            return
                new PrioritySelector(
                    Spell.Cast("Force Armor",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.HasBuff("Force Armor") &&
                                      !Helpers.Companion.HasDebuff("Force-imbalance")),
                    Spell.Cast("Force Armor",
                               onWhoToCast => BuddyTor.Me,
                               ret => !BuddyTor.Me.HasBuff("Force Armor") &&
                                      !BuddyTor.Me.HasDebuff("Force-imbalance")),
                    Spell.Cast("Rejuvenate",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      Helpers.Companion.HealthPercent < 90),
                    Spell.Cast("Rejuvenate",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 90),
                    Spell.Cast("Healing Trance",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      Helpers.Companion.HealthPercent < 80),
                    Spell.Cast("Healing Trance",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 80),
                    Spell.Cast("Benevolence",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      Helpers.Companion.HealthPercent < 30),
                    Spell.Cast("Benevolence",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 30),
                    Spell.Cast("Deliverance",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      Helpers.Companion.HealthPercent < 25),
                    Spell.Cast("Deliverance",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 25),
                    Spell.Cast("Noble Sacrifice",
                               onWhoToCast => BuddyTor.Me,
                               ret => !BuddyTor.Me.HasDebuff("Noble Sacrifice (2)") &&
                                   BuddyTor.Me.HealthPercent >= 90 &&
                                   BuddyTor.Me.ResourceStat <= 90));
        }
    }
}