using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using Buddy.Common.Math;
using Buddy.Swtor;
using Buddy.Swtor.Objects;

namespace DefaultCombat
{
    /// <summary>
    /// Class of helpers, because copy/pasting the same thing in every class got a bit tedious.
    /// </summary>
    /// 3/20/2012 11:21 AM Aevitas
    public static class Helpers
    {
        /*public static int GetBuffStacks(TorCharacter target, string buffName)
        {
            var buff = target.GetBuff(buffName);
            if (buff != null)
                return buff.Stacks;
            return 0;
        }
        public static int GetDebuffStacks(TorCharacter target, string buffName)
        {
            var buff = target.GetDebuff(buffName);
            if (buff != null)
                return buff.Stacks;
            return 0;
        }*/

        /// <summary>
        /// Gets all friendly companions, including those of players around us.
        /// </summary>
        /// 3/20/2012 11:21 AM Aevitas
        public static IEnumerable<TorCharacter> Companions
        {
            get
            {
                return
                    ObjectManager.GetObjects<TorCharacter>().Where(
                        unit => unit.Toughness == CombatToughness.Companion && unit.IsFriendly);
            }
        }

        /// <summary>
        /// Gets our current companion.
        /// </summary>
        /// 3/20/2012 11:22 AM Aevitas
        public static TorNpc Companion
        {
            get { return BuddyTor.Me.Companion; }
        }

        /// <summary>
        /// Gets the enemies currently attacking us.
        /// </summary>
        /// 3/20/2012 11:23 AM Aevitas
        public static IEnumerable<TorCharacter> Targets { get { return BuddyTor.Me.EnemiesAttackers; } }

        /// <summary>
        /// Gets the weakest target, that is the one with lowest actual HP.
        /// </summary>
        /// 3/20/2012 12:11 PM Aevitas
        public static TorCharacter WeakestTarget { get { return Targets.OrderBy(t => t.Health).FirstOrDefault(); } }

        /// <summary>
        /// Gets the players in our group.
        /// </summary>
        /// 3/22/2012 4:02 PM Aevitas
        public static IEnumerable<TorPlayer> Group
        {
            get { return ObjectManager.GetObjects<TorPlayer>().Where(p => p.GroupId == BuddyTor.Me.GroupId); }
        }

        /// <summary>
        /// Gets the lowest health player.
        /// </summary>
        /// 3/22/2012 4:06 PM Aevitas
        public static TorCharacter LowestHealthPlayer
        {
            get
            {
                return
                    ObjectManager.GetObjects<TorCharacter>().OrderBy(p => p.HealthPercent).FirstOrDefault(
                        p => p.IsFriendly && !p.IsDead && p.InLineOfSight);
            }
        }

        /// <summary>
        /// Gets the lowest health player in group.
        /// </summary>
        /// Neo93
        public static TorPlayer LowestHealthGroupPlayer
        {
            get
            {
                return 
                    Group.OrderBy(p => p.HealthPercent).FirstOrDefault(
                        p => p.IsFriendly && !p.IsDead && p.InLineOfSight);
            }
        }

        /// <summary>
        /// Gets the players in the group with >= 1 debuff.
        /// </summary>
        /// Neo93
        public static TorPlayer DebuffTarget
        {
            get { return Group.OrderByDescending(o => o.Debuffs).FirstOrDefault(c => c.Debuffs.Count() >= 1); }
        }

        /// <summary>
        /// Gets all players.
        /// </summary>
        /// 3/22/2012 4:06 PM Aevitas
        public static IEnumerable<TorPlayer> Players { get { return ObjectManager.GetObjects<TorPlayer>(); } }

        /// <summary>
        /// Gets all units.
        /// </summary>
        /// 3/22/2012 4:07 PM Aevitas
        public static IEnumerable<TorCharacter> Units { get { return ObjectManager.GetObjects<TorCharacter>(); } }

        /// <summary>
        /// Gets the central target of specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        /// 3/23/2012 8:09 PM Aevitas
        public static TorCharacter CentralTarget(List<TorCharacter> group)
        {
            // Handle single targets without going through too much difficult stuff.
            if (group.Count() == 1)
                return group.FirstOrDefault();

            TorCharacter ret = null;

            float distance = float.MaxValue;

            // We're going to take a look at the cumulative distance of all members in the group in relation
            // to the unit we're evaluating. The one most in the center will have the smallest average distance to the others.
            foreach (var unit in group)
            {
                var totalDistance = group.Sum(t => t.Distance);
                var averageDistance = totalDistance/group.Count;

                if (averageDistance < distance)
                {
                    ret = unit;
                    distance = averageDistance;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the central target of a group, based on who is closest to the "central point" of a group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        /// 3/24/2012 2:55 PM Aevitas
        public static TorCharacter CentralTargetEx(List<TorCharacter> group)
        {
            TorCharacter ret = null;

            if (group.Count == 1)
                return group.FirstOrDefault();

            Vector3 totalPosition = new Vector3(0, 0, 0);
            totalPosition = @group.Aggregate(totalPosition, (current, unit) => current + unit.Position);

            Vector3 centralPoint = totalPosition/group.Count;
            float distanceToCentral = float.MaxValue;

            foreach (var unit in group)
            {
                var distance = Vector3.Distance(unit.Position, centralPoint);
                if (distance < distanceToCentral)
                {
                    ret = unit;
                    distanceToCentral = distance;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the current Tank
        /// </summary>
        /// Neo93
        public static TorCharacter Tank
        {
            get
            {
                if (Group.FirstOrDefault(p => tankSpecs.Contains(SpecHandler.GetSpec(p))) != null)
                {
                    return Group.FirstOrDefault(p => tankSpecs.Contains(SpecHandler.GetSpec(p)));
                }
                else
                    return Companion;
            }
        }

        private static SkillTreeId[] tankSpecs = { SkillTreeId.AssassinDarkness, SkillTreeId.GuardianDefense,
                                                   SkillTreeId.JuggernautImmortal, SkillTreeId.PowertechShieldTech,
                                                   SkillTreeId.ShadowCombat, SkillTreeId.VanguardShieldSpecialist,
                                                 };

        public static TorCharacter ccTarget()
        {
            var previousTarget = BuddyTor.Me.CurrentTarget;
            return
            Targets.FirstOrDefault(
                t =>
                t != previousTarget && (t.Toughness == CombatToughness.Player || t.Toughness == CombatToughness.Strong || t.Toughness == CombatToughness.Standard || t.Toughness == CombatToughness.Weak) && !t.IsDead) ??
           Targets.FirstOrDefault(t => t != previousTarget);
        }
    }
}
