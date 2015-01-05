using Buddy.BehaviorTree;
using Buddy.Common.Math;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Core
{
    public static class Targeting
    {   
        //Static Points and People
        public static string TankName = "";
        public static TorCharacter Tank;
        public static TorCharacter HealTarget;
        public static TorCharacter AOEHealTarget;
        public static Vector3 AOEHealPoint = Vector3.Zero;
        public static TorCharacter DispelTarget;
        private static TorPlayer Me { get { return BuddyTor.Me; } }
        public static TorCharacter AoeDpsTarget;
        public static Vector3 AOEDpsPoint = Vector3.Zero;

        //Counts
        public static int AOEHealCount         = 0;
        public static int AOEDpsCount          = 0;
        public static int AOEPeanutButterCount = 0;

        //Settings for making target queries
        private const int MaxHealth           = Health.Max;
        private const float HealingDistance   = Distance.Ranged;
        private const float AOEHealDist       = Distance.MeleeAoE;
        private static int AOEHealHP          = Health.High;

        private static int AOEHealCountNeeded = 2;
        private static int AOEDPSCountNeeded  = 3;
        private static int AOEPBCountNeeded   = 3;

        public static bool ShouldAOEHeal = false;
        public static bool ShouldAOE = false;
        public static bool ShouldPBAOE = false;
     
        //Lists
        public static List<TorCharacter> HealCandidates = new List<TorCharacter>();
        public static List<Vector3> HealCandidatePoints = new List<Vector3>();
        public static List<TorCharacter> Enemies = new List<TorCharacter>();
        public static List<Vector3> EnemyPoints = new List<Vector3>();
        
        public static Composite ScanTargets
        {
            get
            {
                    return new Action(delegate {
                        using (BuddyTor.Memory.AcquireFrame())
                        {
                            //Reset counts
                            AOEHealCount = 0;
                            AOEDpsCount = 0;
                            AOEPeanutButterCount = 0;

                            //Reset Targets
                            Tank = null;
                            HealTarget = null;
                            AOEHealTarget = null;
                            AOEHealPoint = Vector3.Zero;
                            DispelTarget = null;
                            var objects = GetTorCharacters();

                            HealCandidates = new List<TorCharacter>();
                            HealCandidatePoints = new List<Vector3>();
                            EnemyPoints = new List<Vector3>();

                            foreach (TorCharacter c in objects)
                            {
                                //Healing
                                if (DefaultCombat.IsHealer)
                                {
                                    //Find a Tank
                                    if (Me.FocusTargetIsActive && c.Guid == Me.FocusTargetID && !c.IsDead)
                                        Tank = c;

                                    if (Tank == null && c.Name.Contains(TankName) && !c.IsDead)
                                        Tank = c;

                                    if (Tank == null && Me.Companion != null && !Me.Companion.IsDead)
                                        Tank = Me.Companion;

                                    if (Tank == null)
                                        Tank = Me;

                                    if (c.HealthPercent <= MaxHealth && !c.IsDead)
                                    {
                                        if (HealTarget == null || c.HealthPercent < HealTarget.HealthPercent)
                                            HealTarget = c;

                                        //Add to candidtates list
                                        HealCandidates.Add(c);
                                        HealCandidatePoints.Add(c.Position);

                                        //increment our AOEHealCount
                                        if (c.HealthPercent <= AOEHealHP)
                                            AOEHealCount++;
                                    }

                                    if (c.NeedsCleanse())
                                    {

                                        if (DispelTarget != null && c.HealthPercent < DispelTarget.HealthPercent)
                                            DispelTarget = c;

                                        if (DispelTarget == null)
                                            DispelTarget = c;
                                    }
                                }

                                //Dps
                                if (c.IsValidTarget())
                                {
                                    //Enemies.Add(c);
                                    EnemyPoints.Add(c.Position);
                                }
                            }

                            if (DefaultCombat.IsHealer)
                            {
                                //We have checked everyone out, lets set AOE stuff
                                if (AOEHealCount >= AOEHealCountNeeded)
                                {
                                    ShouldAOEHeal = true;
                                    //AOEHealTarget
                                    AOEHealTarget = AOEHealLocation(AOEHealDist);

                                    //AOEHealPoint
                                    if (AOEHealTarget != null)
                                        AOEHealPoint = AOEHealLocation(AOEHealTarget);
                                }
                            }

                            if (Me.CurrentTarget != null)
                                ShouldAOE = CheckDPSAOE(AOEDPSCountNeeded, Distance.MeleeAoE, Me.CurrentTarget.Position);

                            ShouldPBAOE = CheckDPSAOE(AOEDPSCountNeeded, Distance.MeleeAoE, Me.Position);

                            return RunStatus.Failure;
                        }
                    
                    });
            }
        }

        public static void SetTank()
        {
            if (Me.CurrentTarget != null && Me.CurrentTarget.IsFriendly)
            {
                Tank = Me.CurrentTarget;
                Logger.Write("Tank set to : " + Tank.Name);
            }
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
                List<TorCharacter> objects = new List<TorCharacter>();
                IEnumerable<TorNpc> npcs = ObjectManager.GetObjects<TorNpc>();

                foreach (var c in npcs)
                    objects.Add(c as TorCharacter);


                if (DefaultCombat.IsHealer)
                {
                    IEnumerable<TorPlayer> players = ObjectManager.GetObjects<TorPlayer>();

                    foreach (var c in players)
                        objects.Add(c as TorCharacter);
                }

                return objects;
            }
        }

        private static Vector3 AOEHealLocation(TorCharacter p)
        {
            return p != null ? p.Position : Vector3.Zero;
        }

        private static TorCharacter AOEHealLocation(float dist)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                TorCharacter pt = Me;

                if (Tank != null)
                    pt = Tank;

                var currentPtCount = PointsAroundPoint(pt.Position, HealCandidatePoints, dist);
                var tempCount = 0;
                foreach (TorCharacter p in HealCandidates)
                {
                    tempCount = PointsAroundPoint(p.Position, HealCandidatePoints, dist);
                    if (p.Guid != Me.Guid && tempCount > currentPtCount)
                    {
                        pt = p;
                        currentPtCount = tempCount;
                    }
                }

                return tempCount >= AOEHealCountNeeded ? pt : null;
            }
        }

        private static int PointsAroundPoint(Vector3 pt, List<Vector3> l, float dist)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                var maxDistance = dist * dist;
                return l.Count(p => p.DistanceSqr(pt) <= maxDistance);
            }
        }

        public static bool CheckDPSAOE(int minMobs, float distance, Vector3 center)
        {
            return PointsAroundPoint(center, EnemyPoints, distance) >= minMobs;
        }
    }
}
