using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat
{
    public class DefaultCombat : CombatRoutine
    {
        public int number { get { return 0; } }
        private Composite _ooc;
        private Composite _pull;
        private Composite _combat;

        public static CombatRoutine Instance { get; set; }

        #region Overrides of CombatRoutine

        public override string Name { get { return "DefaultCombat"; } }

        public override Window ConfigWindow { get { return null; } }

        public override CharacterClass Class { get { return BuddyTor.Me.Class; } }
        public AdvancedClass AdvClass { get { return BuddyTor.Me.AdvancedClass; } }
        public SkillTreeId charSpec { get { return SpecHandler.GetSpec(); } }

        public override Composite OutOfCombat { get { return _ooc; } }

        public override Composite Pull { get { return _pull; } }

        public override Composite Combat { get { return _combat; } }

        public override void Dispose()
        {
        }

        private readonly List<string> _selfHeals = new List<string>
        {
            "Introspection","Meditation","Recuperate","Recharge and Reload","Channel Hatred","Seethe",
        };

        public override void Initialize()
        {
            Logger.Write("Level: " + BuddyTor.Me.Level);
            Logger.Write("Class: " + Class);
            Logger.Write("Advanced Class: " + AdvClass + " (" + SpecHandler.GetSpec().ToString().Replace(AdvClass.ToString(), "") + ")");
            Instance = this;
            _ooc = new PrioritySelector(
                        new Decorator(ret => _selfHeals.Any(b => BuddyTor.Me.HasBuff(b) && BuddyTor.Me.HealthPercent < 95),
                            new Action(ret => RunStatus.Success)),
                        new Decorator(ret => !BuddyTor.Me.IsDead,
                            OOCWhenAlive()),
                        new Decorator(ret => BuddyTor.Me.HealthPercent < 70,
                            CreateUseClassRegainHealth()),
                        CreateBuffSelf()
                );

            int count;

            var extraOoc = CompositeBuilder.GetComposite(Class, AdvClass, charSpec, BehaviorType.OutOfCombat, out count);
            // Add the extra logic for OOC stuff from the combat declaration.
            if (extraOoc != null && count > 0)
                (_ooc as PrioritySelector).AddChild(extraOoc);

            _combat = CompositeBuilder.GetComposite(Class, AdvClass, charSpec, BehaviorType.Combat, out count);
            if (count == 0 || _combat == null)
            {
                Logger.Write("Combat support for " + Class + " [" + charSpec + "] is not currently implemented.");
                BotMain.Stop();
            }

            _pull = CompositeBuilder.GetComposite(Class, AdvClass, charSpec, BehaviorType.Pull, out count);
            if (count == 0 || _pull == null)
            {
                Logger.Write("Pull support for " + Class + " is not currently implemented.");
                if (_combat != null)
                {
                    _pull = _combat;
                    Logger.Write("Using Pull for combat.");
                }
                else
                {
                    BotMain.Stop();
                    Logger.Write("Buddy Wing stopped!");
                }
            }


        }

        #endregion



        private Composite CreateBuffSelf()
        {
            return new Sequence(
                new PrioritySelector(
                CompanionHandler(),
                Spell.BuffSelf("Sprint"),

                // Do not change this to a PrioSelector and a chain of Spell.BuffSelf calls. This is a switch for performance reasons.
                new Switch<CharacterClass>(ret => Class,

                    // Republic
                    new SwitchArgument<CharacterClass>(CharacterClass.Knight, Spell.BuffSelf("Force Might")),
                    new SwitchArgument<CharacterClass>(CharacterClass.Consular, Spell.BuffSelf("Force Valor")),
                    new SwitchArgument<CharacterClass>(CharacterClass.Smuggler, Spell.BuffSelf("Lucky Shots")),
                    new SwitchArgument<CharacterClass>(CharacterClass.Trooper, Spell.BuffSelf("Fortification")),

                    // Empire
                    new SwitchArgument<CharacterClass>(CharacterClass.Warrior, Spell.BuffSelf("Unnatural Might")),
                    new SwitchArgument<CharacterClass>(CharacterClass.Inquisitor, Spell.BuffSelf("Mark of Power")),
                    new SwitchArgument<CharacterClass>(CharacterClass.Agent, Spell.BuffSelf("Coordination")),
                    new SwitchArgument<CharacterClass>(CharacterClass.BountyHunter, Spell.BuffSelf("Hunter's Boon"))
                )));
        }
        private Composite OOCWhenAlive()
        {
            // Do This when out of combat. 
            return new PrioritySelector(
                new Decorator(ret => (BuddyTor.Me.Class == CharacterClass.Smuggler || BuddyTor.Me.Class == CharacterClass.Agent) && (BuddyTor.Me.HasBuff("Cover") || BuddyTor.Me.HasBuff("Crouch")),
                    new Action(ret => Buddy.Swtor.Movement.Move(MovementDirection.Forward, TimeSpan.FromMilliseconds(10)))));
        }

        private Composite CompanionHandler()
        {
            return new PrioritySelector(
               new Decorator(ret => Helpers.Companion != null && Helpers.Companion.IsDead && BuddyTor.Me.CompanionUnlocked > 0,
                    new PrioritySelector(
                        Spell.WaitForCast(),
                        Spell.Cast("Revive Companion", on => Helpers.Companion, when => Helpers.Companion.Distance <= 0.2f),
                        new Sleep(2500), // I don't give a damn. It's going to want to do other stuff while resting, so let's force him to wait a bit.
                        Movement.MoveTo(ret => Helpers.Companion.Position, 0.2f)
                        ))
                //new Decorator(ret => BuddyTor.Me.CompanionSummonable, //Summon)
               );
        }

        private Composite CreateUseClassRegainHealth()
        {
            // Do not change this to a PrioSelector and a chain of Spell.BuffSelf calls. This is a switch for performance reasons.
            return new PrioritySelector(
                new Decorator(
                    ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat < 95 || BuddyTor.Me.Companion.HealthPercent <= 95,
                //new Decorator(ret => BuddyTor.Me.Class == CharacterClass.Trooper || BuddyTor.Me.Class == CharacterClass.,
                    Spell.WaitForCast()),

                new Switch<CharacterClass>(
                    ret => Class,
                // Republic
                    new SwitchArgument<CharacterClass>(CharacterClass.Knight, Cast("Introspection", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95)),
                    new SwitchArgument<CharacterClass>(CharacterClass.Consular, Cast("Meditation", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat <= 50)),
                    new SwitchArgument<CharacterClass>(CharacterClass.Smuggler, Cast("Recuperate", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat <= 50)),
                    new SwitchArgument<CharacterClass>(CharacterClass.Trooper, Cast("Recharge and Reload", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat < 5)),

                    // Empire
                    new SwitchArgument<CharacterClass>(CharacterClass.Warrior, Cast("Channel Hatred", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95)),
                    new SwitchArgument<CharacterClass>(CharacterClass.Inquisitor, Cast("Seethe", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat <= 50)),
                    new SwitchArgument<CharacterClass>(CharacterClass.Agent, Cast("Recuperate", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat <= 50)),
                    new SwitchArgument<CharacterClass>(CharacterClass.BountyHunter, Cast("Recharge and Reload", ret => BuddyTor.Me, ret => BuddyTor.Me.HealthPercent < 95 || BuddyTor.Me.ResourceStat >= 50))
                    ));
        }
    }
}