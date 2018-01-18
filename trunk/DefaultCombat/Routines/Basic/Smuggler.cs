// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Smuggler : RotationBase
    {
        public override string Name
        {
            get { return "Basic Smuggler"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots")
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
                    CombatMovement.CloseDistance(Distance.Ranged),
                    Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
                    Spell.Cast("Sabotage Charge"),
                    Spell.Cast("Flurry of Bolts")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Thermal Grenade", ret => Me.CurrentTarget.Distance <= Distance.Ranged)
                        ));
            }
        }
    }
}