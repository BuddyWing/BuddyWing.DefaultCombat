// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Collections.Generic;
using System.Linq;
using Buddy.BehaviorTree;
using Buddy.Common.Math;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Helpers;

namespace DefaultCombat.Core
{
	public static class Targeting
	{
		//Settings for making target queries
		private const int MaxHealth = Health.Max;
		private const float HealingDistance = Distance.Ranged;
		private const float AoeHealDist = Distance.MeleeAoE;
		//Static Points and People
		public static string TankName = "";
		public static TorCharacter Tank;
		public static TorCharacter HealTarget;
		public static TorCharacter AoeHealTarget;
		public static Vector3 AoeHealPoint = Vector3.Zero;
		public static TorCharacter DispelTarget;
		public static TorCharacter AoeDpsTarget;
		public static Vector3 AoeDpsPoint = Vector3.Zero;
		//Counts
		public static int AoeHealCount;
		public static int AoeDpsCount;
		public static int AoePeanutButterCount;
		private const int AoeHealHp = Health.High;
		private const int AoeHealCountNeeded = 2;
		private const int AoedpsCountNeeded = 3;
		private static int _aoepbCountNeeded = 3;
		public static bool ShouldAoeHeal;
		public static bool ShouldAoe;
		public static bool ShouldPbaoe;
		//Lists
		public static List<TorCharacter> HealCandidates = new List<TorCharacter>();
		public static List<Vector3> HealCandidatePoints = new List<Vector3>();
		public static List<TorCharacter> Enemies = new List<TorCharacter>();
		public static List<Vector3> EnemyPoints = new List<Vector3>();

		private static TorPlayer Me
		{
			get { return BuddyTor.Me; }
		}

		public static Composite ScanTargets
		{
			get
			{
				return new Action(delegate
				{
					using (BuddyTor.Memory.AcquireFrame())
					{
						//Reset counts
						AoeHealCount = 0;
						AoeDpsCount = 0;
						AoePeanutButterCount = 0;

						//Reset Targets
						Tank = null;
						HealTarget = null;
						AoeHealTarget = null;
						AoeHealPoint = Vector3.Zero;
						DispelTarget = null;
						var objects = GetTorCharacters();

						HealCandidates = new List<TorCharacter>();
						HealCandidatePoints = new List<Vector3>();
						EnemyPoints = new List<Vector3>();

						foreach (var c in objects)
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
									if (c.HealthPercent <= AoeHealHp)
										AoeHealCount++;
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

						if (Me.CurrentTarget != null)
							ShouldAoe = CheckDpsAoe(AoedpsCountNeeded, Distance.MeleeAoE, Me.CurrentTarget.Position);

						ShouldPbaoe = CheckDpsAoe(AoedpsCountNeeded, Distance.MeleeAoE, Me.Position);

						return RunStatus.Failure;
					}
				});
			}
		}

		public static void SetTank()
		{
			if (Me.CurrentTarget != null && Me.CurrentTarget.IsFriendly)
			{
				TankName = Me.CurrentTarget.Name;
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
				var npcs = ObjectManager.GetObjects<TorNpc>();
				var objects = npcs.Cast<TorCharacter>().ToList();
				
				if (DefaultCombat.IsHealer)
				{
					var players = ObjectManager.GetObjects<TorPlayer>();

					objects.AddRange(players);
				}

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

		private static int PointsAroundPoint(Vector3 pt, List<Vector3> l, float dist)
		{
			using (BuddyTor.Memory.AcquireFrame())
			{
				var maxDistance = dist*dist;
				return l.Count(p => p.DistanceSqr(pt) <= maxDistance);
			}
		}

		public static bool CheckDpsAoe(int minMobs, float distance, Vector3 center)
		{
			return PointsAroundPoint(center, EnemyPoints, distance) >= minMobs;
		}
	}
}