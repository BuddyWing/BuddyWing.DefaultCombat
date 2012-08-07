using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    // Pios - 10/05
    public static class SageTelekinetics
    {
        /// <summary>
        /// Sage lightning pull.
        /// </summary>
        /// <returns></returns>
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageTelekinetics)]
        public static Composite SageTelekineticPull()
        {
            return new PrioritySelector(
                Spell.Cast("Mind Crush"),
                Spell.Cast("Weaken Mind"),
                Spell.Cast("Force Stun"),
                Spell.Cast("Project"),
                SageTelekineticCombat()
                );
        }

        /// <summary>
        /// Sorcerers lightning combat.
        /// </summary>
        /// <returns></returns>
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Consular, AdvancedClass.Sage, SkillTreeId.SageTelekinetics)]
        public static Composite SageTelekineticCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Force of Will", castWhen => BuddyTor.Me.IsStunned),
                Movement.MoveToLineOfSight(), // Check we are inline of sight. 
                CommonBuffsAndHealing(),
                Spell.Cast("Weaken Mind", canRun => !BuddyTor.Me.CurrentTarget.HasDebuff("Weaken Mind (Force)")),
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
                Spell.Cast("Force Potency"),
                Spell.Cast("Turbulence"),
                Spell.Cast("Mind Crush",
                           ret => BuddyTor.Me.HasBuff("Presence of Mind") &&
                                  !BuddyTor.Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                Spell.Cast("Telekinetic Throw",
                           ret => BuddyTor.Me.HasBuff("Psychic Projection")),
                Spell.Cast("Telekinetic Wave",
                           ret => BuddyTor.Me.HasBuff("Tidal Force")),
                Spell.WaitForCast(),
                Spell.Cast("Telekinetic Throw"),
                Spell.WaitForCast(),
                Spell.Cast("Disturbance"),
                new Decorator( // Only cast shock if we are moving.
                    ret => BuddyTor.Me.IsMoving,
                    Spell.Cast("Project")),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 29f / 10f)
                );
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