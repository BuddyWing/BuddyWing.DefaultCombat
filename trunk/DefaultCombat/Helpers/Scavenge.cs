// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Core;
using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Helpers
{
    //This shit comes from Joe's
    internal class Scavenge
    {
        private static Stopwatch _throttleTimer = Stopwatch.StartNew();
        private static TimeSpan _pauseDuration = TimeSpan.FromSeconds(3);
        private static TorNpc _scavengeTargetCached;

        private static TorPlayer Me
        {
            get { return BuddyTor.Me; }
        }

        private static bool HasScav
        {
            get { return Me.ProfessionInfos.Any(p => p.Name.Contains("Scavenging")); }
        }

        private static bool HasBio
        {
            get { return Me.ProfessionInfos.Any(p => p.Name.Contains("Bioanalysis")); }
        }

        public static TorCharacter ScavUnit
        {
            get
            {
                // This takes up about 50% of the total time we spend in Tick(), so we'll cache the target we select for scavenging.
                // (Especially the npc.IsDead check takes a long time)
                if (_throttleTimer.Elapsed < _pauseDuration && _scavengeTargetCached != null)
                    return _scavengeTargetCached;

                _scavengeTargetCached = ObjectManager.GetObjects<TorNpc>().FirstOrDefault(CanHarvestCorpse);

                _throttleTimer.Restart();
                return _scavengeTargetCached;
            }
        }

        public static Composite ScavengeCorpse
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => ScavUnit != null,
                        new PrioritySelector(
                            Spell.WaitForCast(),
                            CommonBehaviors.MoveAndStop(location => ScavUnit.Position, 0.2f, true),
                            new Action(ret => ScavUnit.Interact())
                            ))
                    );
            }
        }


        public static bool CanHarvestCorpse(TorNpc unit)
        {
            var SkillReq = Math.Max(unit.Level - 3, 0) * 8;
            if (unit != null)
                if (!unit.IsDeleted)
                    if (unit.IsDead && unit.Distance <= 4.0f
                        &&
                        ((HasBio && unit.CreatureType == CreatureType.Creature && PISkill("Bioanalysis") >= SkillReq) ||
                         (HasScav && unit.CreatureType == CreatureType.Droid && PISkill("Scavenging") >= SkillReq))
                        && StrongOrGreater(unit) && !unit.IsFriendly)
                        if (!Blacklist.Contains(unit.Guid))
                            return true;

            return false;
        }

        public static ulong PISkill(string Skill)
        {
            foreach (var pi in BuddyTor.Me.ProfessionInfos) if (pi.Name.Contains(Skill)) return (ulong)pi.CurrentLevel;
            return 0;
        }

        public static bool StrongOrGreater(TorCharacter unit)
        {
            if (unit != null)
                if (unit.Toughness == CombatToughness.Strong || unit.Toughness == CombatToughness.Boss1 ||
                    unit.Toughness == CombatToughness.Boss2 || unit.Toughness == CombatToughness.Player)
                    return true;
            return false;
        }
    }
}
