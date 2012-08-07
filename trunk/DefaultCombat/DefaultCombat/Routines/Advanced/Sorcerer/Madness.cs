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
    public static class SorcererMadness
    {
        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererMadness)]
        public static Composite SorcererMadnessPull()
        {
            return new PrioritySelector(
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 29f / 10f),
                Spell.CastOnGround("Death Field",
                                   castWhen => BuddyTor.Me.ResourceStat >= 100,
                                   location => BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Affliction"),
                SorcererMadnessCombat()
                );
        }

        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererMadness)]
        public static Composite SorcererMadnessCombat()
        {
            return new PrioritySelector(
                
                Spell.Cast("Force of Will", castWhen => BuddyTor.Me.IsStunned),
                CommonBuffsAndHealing(),
                CommonInterupts(),
                Spell.Cast("Consumption",
                        onWhoToCast => BuddyTor.Me,
                        ret => BuddyTor.Me.Debuffs.FirstOrDefault(d => d.Name == "Consumption").Stacks <= 2 &&
                               BuddyTor.Me.ForcePercent <= 50f &&
                               BuddyTor.Me.HealthPercent >= 50f),
                Spell.WaitForCast(),
                Spell.CastOnGround("Death Field",
                                   castWhen => BuddyTor.Me.CurrentTarget.Distance <= 0.5f &&
                                               BuddyTor.Me.ResourceStat >= 100,
                                   location => BuddyTor.Me.CurrentTarget.Position),
                Spell.Cast("Affliction",
                    onUnit =>
                    {
                        return BuddyTor.Me.CurrentTarget;
                    },
                    castWhen => !BuddyTor.Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                Spell.Cast("Recklessness"), // Lets get recklessness up.
                Spell.Cast("Force Lightning", castWhen => !BuddyTor.Me.HasBuff("Wrath")),
                Spell.WaitForCast(),
                
                Spell.Cast("Crushing Darkness",
                           castWhen => BuddyTor.Me.HasBuff("Wrath") &&
                                  !BuddyTor.Me.CurrentTarget.HasDebuff("Crushing Darkness")),
                Spell.Cast("Lightning Strike",castWhen => BuddyTor.Me.HasBuff("Wrath")),
                
                new Decorator( // Only cast shock if we are moving.
                    ret => BuddyTor.Me.IsMoving,
                    Spell.Cast("Shock")),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position,Global.RangeDist));
        }

        [Behavior(BehaviorType.OutOfCombat)]
        [Class(CharacterClass.Inquisitor, AdvancedClass.Sorcerer, SkillTreeId.SorcererMadness)]
        public static Composite SorcererMadnessOOC()
        {
            return new PrioritySelector(
                Spell.Cast("Seethe", ret => BuddyTor.Me.HealthPercent <= 90 || BuddyTor.Me.ResourceStat <= 40));
        }
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
                    Spell.Cast("Dark Infusion",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 20),
                    Spell.Cast("Dark Infusion",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 20),
                    Spell.Cast("Dark Heal",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 25),
                    Spell.Cast("Dark Heal",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 25),
                    
                    Spell.Cast("Innervate",
                               onWhoToCast => Helpers.Companion,
                               ret => Helpers.Companion != null &&
                                      !Helpers.Companion.IsDead &&
                                      Helpers.Companion.HealthPercent < 80),
                    Spell.Cast("Innervate",
                               onWhoToCast => BuddyTor.Me,
                               ret => BuddyTor.Me.HealthPercent < 80)
                    );
        }

        private static Composite CommonInterupts()
        {
            return Spell.Cast("Jolt", castTarget => Helpers.Targets.FirstOrDefault(t => t.IsCasting), shouldCast => Helpers.Targets.Count(t => t.IsCasting) != 0);
        }
    }
}
