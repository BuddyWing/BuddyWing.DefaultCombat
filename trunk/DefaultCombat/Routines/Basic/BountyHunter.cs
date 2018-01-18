// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class BountyHunter : RotationBase
    {
        public override string Name
        {
            get { return "Basic Bounty Hunter"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Hunter's Boon")
                    );
            }
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
                    CombatMovement.CloseDistance(Distance.Ranged),
                    Spell.Cast("Rail Shot"),
                    Spell.Cast("Vent Heat", ret => Me.ResourcePercent() >= 50),
                    Spell.Cast("Flame Burst", ret => Me.ResourcePercent() <= 50),
                    Spell.Cast("Rapid Shots")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector());
            }
        }
    }
}