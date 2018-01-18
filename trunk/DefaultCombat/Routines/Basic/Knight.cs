// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Knight : RotationBase
    {
        public override string Name
        {
            get { return "Basic Knight"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Saber Ward", ret => Me.HealthPercent <= 70)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Leap", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance > 1f && Me.CurrentTarget.Distance <= 3f),


                CombatMovement.CloseDistance(Distance.Melee),
                    Spell.Cast("Master Strike"),
                    Spell.Cast("Blade Storm"),
                    Spell.Cast("Riposte"),
                    Spell.Cast("Slash", ret => Me.ActionPoints >= 7),
                    Spell.Cast("Strike")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Force Sweep", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE))
                    );
            }
        }
    }
}