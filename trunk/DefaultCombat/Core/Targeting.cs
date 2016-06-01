// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;


using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using Buddy.Swtor.Objects.Containers;
using Buddy.Navigation;
using Buddywing;
using Buddy.CommonBot.Settings;
using Buddy.Common.Math;
using DefaultCombat.Helpers;


using Action = Buddy.BehaviorTree.Action;


namespace DefaultCombat.Core
{

    public static class Targeting
    {
        //Collections
        public static List<TorCharacter> HealCandidates;
        public static List<TorCharacter> Tanks;
        public static List<Vector3> HealCandidatePoints;
        public static List<TorCharacter> Enemies = new List<TorCharacter>();
        public static List<Vector3> EnemyPoints = new List<Vector3>();

        //Static Points and People
        public static string TankName = "";
        public static string TankNameStart;
        public static string TankNameCheck;
        public static TorCharacter Tank;
        public static TorCharacter HealTarget;
        public static TorCharacter AoeHealTarget;
        public static TorCharacter AoeDpsTarget;
        public static TorCharacter DispelTarget;
        private static TorPlayer Me
        {
            get { return BuddyTor.Me; }
        }
        private const int AoedpsCountNeeded = 3;

        public static Vector3 AoeDpsPoint = Vector3.Zero;

        //Counts
        public static int AoeHealCount;
        private const int AoeHealCountNeeded = 2;
        public static int AoeDpsCount;
        public static int AoePeanutButterCount;
        private static int _aoepbCountNeeded = 3;
        public static bool ShouldAoeHeal;
        public static bool ShouldAoe;
        public static bool ShouldPbaoe;

        //Settings for making target queries
        private const int MaxHealth = Health.Max;
        private const float HealingDistance = Distance.Ranged;
        private const float AoeHealDist = Distance.HealAoe;
        private const int AoeHealHp = Health.High;
        public static Vector3 AoeHealPoint = Vector3.Zero;

        //Determine if we should use the tank's target.
        private static bool UseTankTarget
        {
            get
            {
                return Me.CurrentTarget == null && Tank != null && Tank.Guid != Me.Guid && Tank.InCombat && Tank.CurrentTarget != null;
            }
        }


        //Caching shit
        public static int cacheCount = 75;
        public static int maxCacheCount = 2;
        public static List<TorCharacter> Objects;
        public static List<TorCharacter> objects;
        public static Composite ScanTargets
        {
            get
            {
                return new Action(delegate
                {
                    //increment shit!
                    cacheCount++;

                    //Reset counts
                    AoeHealCount = 0;
                    AoeDpsCount = 0;
                    AoePeanutButterCount = 0;
                    //Reset Targets
               //     Tank = null;
                    HealTarget = null;
                    AoeHealTarget = null;
                    AoeHealPoint = Vector3.Zero;
                    DispelTarget = null;


                    //Reset Lists and shit
                    HealCandidates = new List<TorCharacter>();
                    HealCandidatePoints = new List<Vector3>();
                    EnemyPoints = new List<Vector3>();
                    Tanks = new List<TorCharacter>();
                    ShouldAoeHeal = false;
                    var objects = GetTorCharacters();


                    //update the cache when we feel like it
                    if (cacheCount >= maxCacheCount)
                        updateObjects();

                    if (DefaultCombat.IsHealer)
                    {

                        foreach (TorCharacter p in Objects)
                        {
                            //Doing this shit early

                            //    Logger.Write(p.Name);
                            if (Tank != null && Tank == p)
                                Tank = p;

                            if (Tank == null && p.Name == TankName && !p.IsDead)
                                Tank = p;

                            // Got a Focus Tank?
                            if (Tank == null && Me.FocusTargetIsActive && p.Guid == Me.FocusTargetID && !p.IsDead)
                                Tank = p;

                            if (p.IsPartyRoleTank())
                                Tanks.Add(p);

                            // Damn couldnt find a tank ima be the boss!

                            if (Tank == null && Me.Companion != null)
                                Tank = Me.Companion;

                            if (Tank == null && Me.Companion == null)
                                Tank = Me;

                            //Check for HealTarget
                            if (p.HealthPercent <= MaxHealth && !p.IsDead)
                            {
                                if (HealTarget == null || p.HealthPercent < HealTarget.HealthPercent)
                                    HealTarget = p;

                                //Add to candidtates list
                                HealCandidates.Add(p);
                                HealCandidatePoints.Add(p.Position);

                                //increment our AOEHealCount
                                if (p.HealthPercent <= AoeHealHp)
                                    AoeHealCount++;
                            }

                            if (p.NeedsCleanse())
                            {

                                if (DispelTarget != null && p.HealthPercent < DispelTarget.HealthPercent)
                                    DispelTarget = p;

                                if (DispelTarget == null)
                                    DispelTarget = p;
                            }

                        }


                        //We have checked everyone out, lets set AOE stuff
                        if (AoeHealCount >= AoeHealCountNeeded)
                        {
                            ShouldAoeHeal = true;
                            //AOEHealTarget
                            AoeHealTarget = AoeHealLocation(AoeHealDist);

                            //AOEHealPoint
                            if (AoeHealTarget != null)
                                AoeHealPoint = AoeHealLocation(AoeHealTarget);
                        }
                    }

                    foreach (var c in objects)
                    {


                        //Dps
                        if (c.IsValidTarget())
                        {
                            //Enemies.Add(c);
                            EnemyPoints.Add(c.Position);
                        }

                        if (Me.CurrentTarget != null)
                            ShouldAoe = CheckDpsAoe(AoedpsCountNeeded, Distance.MeleeAoE, Me.CurrentTarget.Position);

                        ShouldPbaoe = CheckDpsAoe(AoedpsCountNeeded, Distance.MeleeAoE, Me.Position);
                    }





                    return RunStatus.Failure;
                });
            }
        }

        public static void SetTank()
        {
            TankNameStart = Me.CurrentTarget.ToString();
            TankNameCheck = TankNameStart.Substring(0, TankNameStart.IndexOf(','));

            if (Me.CurrentTarget != null && Me.CurrentTarget.IsFriendly && !TankName.Equals(TankNameCheck))
            {
                TankName = TankNameStart.Substring(0, TankNameStart.IndexOf(','));
                Logger.Write("Tank set to : " + TankName);
                Tank = null;
            }
            else
            {
                TankName = "";
                Tank = null;
                Logger.Write("Cleared Tank");
            }
        }

        private static void updateObjects()
        {
            if (DefaultCombat.IsHealer)
            {
                Objects = Me.PartyMembers(false).ToList().FindAll(p =>
                !p.IsDead
                && p.DistanceSqr < HealingDistance * HealingDistance
                && p.InLineOfSight);

                if (!Objects.Contains(Me))
                    Objects.Add(Me);

                if (Me.Companion != null && !Objects.Contains(Me.Companion))
                    Objects.Add(Me.Companion);
            }

            //Reset dat count
            cacheCount = 0;
        }

        public static List<TorCharacter> GetTorCharacters()
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                //List<TorCharacter> objects = ObjectManager.GetObjects<TorCharacter>().ToList();

                /*
                if (Me.Companion != null)
                    objects.Add(Me.Companion);
                */
                var npcs = ObjectManager.GetObjects<TorNpc>();
                var objects = npcs.Cast<TorCharacter>().ToList();

                return objects;
            }
        }

        private static Vector3 AoeHealLocation(TorCharacter p)
        {
            return p != null ? p.Position : Vector3.Zero;
        }

        private static TorCharacter AoeHealLocation(float dist)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                TorCharacter pt = Me;

                if (Tank != null)
                    pt = Tank;

                var currentPtCount = PointsAroundPoint(pt.Position, HealCandidatePoints, dist);
                var tempCount = 0;
                foreach (var p in HealCandidates)
                {
                    tempCount = PointsAroundPoint(p.Position, HealCandidatePoints, dist);
                    if (p.Guid != Me.Guid && tempCount > currentPtCount)
                    {
                        pt = p;
                        currentPtCount = tempCount;
                    }
                }

                return tempCount >= AoeHealCountNeeded ? pt : null;
            }
        }

        public static bool CheckDpsAoe(int minMobs, float distance, Vector3 center)
        {
            return PointsAroundPoint(center, EnemyPoints, distance) >= minMobs;
        }

        private static int PointsAroundPoint(Vector3 pt, List<Vector3> l, float dist)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                var maxDistance = dist * dist;
                return l.Count(p => p.DistanceSqr(pt) <= maxDistance);
            }
        }
    }
}
