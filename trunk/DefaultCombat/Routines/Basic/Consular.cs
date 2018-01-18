// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Consular : RotationBase
    {
        public override string Name
        {
            get { return "Basic Consular"; }
        }

        public override Composite Buffs
        {
            get { return new PrioritySelector(); }
        }

        public override Composite Cooldowns
        {
            get { return new PrioritySelector(); }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    CombatMovement.CloseDistance(Distance.Melee),
                    Spell.Cast("Project", ret => Me.Force > 75),
                    Spell.Cast("Saber Strike")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Force Wave", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE))
                    );
            }
        }
    }
}