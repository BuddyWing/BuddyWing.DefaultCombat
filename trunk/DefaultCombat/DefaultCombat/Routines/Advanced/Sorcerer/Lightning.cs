using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    //09.05.2012 Pios
    public static class SorcererLightning
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererLightning)]
        public static Composite SorcererLightningPull()
        {
            return new PrioritySelector(
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 29f / 10f),
                Spell.Cast("Crushing Darkness"),
                Spell.Cast("Affliction"),
                Spell.Cast("Electrocute"),
                Spell.Cast("Shock"),
                SorcererLightningCombat()
                );
        }

        /// <summary>
        /// Sorcerers lightning combat.
        /// </summary>
        /// <returns></returns>
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererLightning)]
        public static Composite SorcererLightningCombat()
        {
            return new PrioritySelector(
                Spell.Cast("Unbreakable Will", castWhen => BuddyTor.Me.IsStunned),
                Movement.MoveToLineOfSight(), // Check we are inline of sight. 
                //SorcererGroupHealing(), // Lets see if we are meant to heal in this group.
                CommonBuffsAndHealing(),
                Spell.Cast("Affliction", canRun => !BuddyTor.Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                Spell.Cast("Affliction", onUnit =>
                {
                    //Get our dots up.
                    var previousTarget = BuddyTor.Me.CurrentTarget;
                    if (Helpers.Targets.Count() == 1)
                        return Helpers.Targets.FirstOrDefault(targ => !targ.HasDebuff("Affliction (Force)"));
                    return
                        Helpers.Targets.FirstOrDefault(
                            targ =>
                            targ != previousTarget &&
                            (targ.Toughness != CombatToughness.Player) &&
                                // Dont go all pvp on us.
                            !targ.IsDead &&
                            !targ.HasDebuff("Affliction (Force)")) ??
                        Helpers.Targets.FirstOrDefault(targ => targ != previousTarget);
                }, castWhen => Helpers.Targets.Count() >= 3 && Helpers.Targets.Count(wh => wh.HasDebuff("Affliction (Force)")) <= 2),
                Spell.Cast("Recklessness"),
                Spell.Cast("Thundering Blast"),
                Spell.Cast("Crushing Darkness",
                           ret => BuddyTor.Me.HasBuff("Wrath") &&
                                  !BuddyTor.Me.CurrentTarget.HasDebuff("Crushing Darkness")),
                Spell.Cast("Force Lightning",
                           ret => BuddyTor.Me.HasBuff("Lightning Barrage")),
                Spell.Cast("Chain Lightning",
                           ret => BuddyTor.Me.HasBuff("Lightning Storm")),
                Spell.WaitForCast(),
                Spell.Cast("Force Lightning"),
                Spell.WaitForCast(),
                Spell.Cast("Lightning Strike"),
                new Decorator( // Only cast shock if we are moving.
                    ret => BuddyTor.Me.IsMoving,
                    Spell.Cast("Shock"))
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
                    Spell.Cast("Static Barrier",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      !Helpers.Companion.HasBuff("Static Barrier") &&
                                      !Helpers.Companion.HasDebuff("Deionized")),
                    Spell.Cast("Static Barrier",
                               onWhoToCast => BuddyTor.Me,
                               ret => !BuddyTor.Me.HasBuff("Static Barrier") &&
                                      !BuddyTor.Me.HasDebuff("Deionized")),
                    Spell.Cast("Resurgence",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 90),
                    Spell.Cast("Resurgence",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 90),
                    Spell.Cast("Innervate",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 80),
                    Spell.Cast("Innervate",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 80),
                    Spell.Cast("Dark Heal",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 30),
                    Spell.Cast("Dark Heal",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 30),
                    Spell.Cast("Dark Infusion",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 25),
                    Spell.Cast("Dark Infusion",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 25),
                    Spell.Cast("Consumption",
                               onWhoToCast => BuddyTor.Me,
                               ret => !BuddyTor.Me.HasDebuff("Consumption (2)") &&
                                   BuddyTor.Me.HealthPercent >= 90 &&
                                   BuddyTor.Me.ResourceStat <= 90));
        }


    }
}