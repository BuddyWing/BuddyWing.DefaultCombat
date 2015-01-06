using System.Windows;
using System.Windows.Forms;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Core;
using DefaultCombat.Routines;
using Buddy.Swtor.Objects;
using DefaultCombat.Helpers;
using Buddy.Common;
using Buddy.Navigation;
using Buddy.CommonBot.Settings;
using System.Collections.Generic;

using Targeting = DefaultCombat.Core.Targeting;
using System.Diagnostics;


namespace DefaultCombat
{
    public class DefaultCombat : CombatRoutine
    {
        private Composite _ooc;
        private Composite _pull;
        private Composite _combat;

        public static bool IsHealer = false;
        public static bool MovementDisabled { get { return BotMain.CurrentBot.Name == "Combat Bot"; } }

        public override string Name { get { return "DefaultCombat"; } }

        public override Window ConfigWindow { get { return null; } }

        public override CharacterClass Class { get { return BuddyTor.Me.Class; } }

        public override Composite OutOfCombat { get { return _ooc; } }

        public override Composite Pull { get { return _pull; } }

        public override Composite Combat { get { return _combat; } }

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
            Logger.Write("Level: " + BuddyTor.Me.Level);
            Logger.Write("Class: " + Class);
            Logger.Write("Advanced Class: " + BuddyTor.Me.AdvancedClass);
            Logger.Write("Discipline: " + BuddyTor.Me.Discipline);

            RotationFactory f = new RotationFactory();
            RotationBase b = f.Build(BuddyTor.Me.Discipline.ToString());

            if (b == null)
                b = f.Build(BuddyTor.Me.CharacterClass.ToString());

            Logger.Write("Rotation Selected : " + b.Name);

            if (BuddyTor.Me.IsHealer())
            {
                IsHealer = true;
                Logger.Write("Healing Enabled");
            }

            _ooc = new Decorator(ret => !BuddyTor.Me.IsDead && !BuddyTor.Me.IsMounted,
                new PrioritySelector(
                    Spell.Buff(BuddyTor.Me.SelfBuffName()),
                    b.Buffs,
                    Rest.HandleRest
                    ));

            _combat = new LockSelector(
                Spell.WaitForCast(),
                Targeting.ScanTargets,
                b.Cooldowns,
                b.AreaOfEffect,
                b.SingleTarget);

            _pull = new Decorator( ret => !DefaultCombat.MovementDisabled || DefaultCombat.IsHealer,
                _combat
                );
        }
    }
}