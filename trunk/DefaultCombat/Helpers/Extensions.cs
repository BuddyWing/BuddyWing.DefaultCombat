using System;
using System.Collections.Generic;
using System.Linq;
using Buddy.Swtor;
using Buddy.Swtor.Objects;

namespace DefaultCombat.Helpers
{
	public static class Extensions
	{
		public static Debuff[] DebuffList =
		{
			new Debuff("Crushing Affliction (Force)", 0),
			new Debuff("Crushing Affliction (All)", 0),
			new Debuff("Corrosive Slime", 0),
			new Debuff("Laser", 4)
		};

		public static string[] DebuffNames_CrowdControl =
		{
			"Afraid (Mental)", // from Intimidating Roar / Awe
			"Blinded (Tech)", // from Flash Grenade / Flash Bang
			"Lifted (Force)", // from Whirlwind / Force Lift
			"Sliced", // from Slice Droid
			"Asleep (Mental)", // from Concussive Round & Mind Maze
			"Sleeping", // from Sleep Dart & Tranquilizer
			"Stunned" // from Debilitate / Dirty Kick
		};

		// This table contains the list of debuff names which prevent us from being shielded...
		public static string[] DebuffNames_Shielded =
		{
			"Deionized", // Result of Sorcerer shielding
			"Force-imbalance" // Result of Sage shielding
		};

		private static readonly string[] BuffNamesForCoverVariants = {"Crouch", "Cover"};

		private static readonly IEnumerable<ClassTunables> TunablesByClass = new List<ClassTunables>
		{
			// Republic
			new ClassTunables
			{
				Class = CharacterClass.Knight,
				RejuvenateAbilityName = "Introspection",
				SelfBuffName = "Force Might",
				IsRejuvenationNeeded = torPlayer => (torPlayer.HealthPercent < 70),
				IsRejuvenationComplete = torPlayer => (torPlayer.HealthPercent >= 95),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.Consular,
				RejuvenateAbilityName = "Meditation",
				SelfBuffName = "Force Valor",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat < 70)),
				IsRejuvenationComplete =
					torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat >= 95)),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.Smuggler,
				RejuvenateAbilityName = "Recuperate",
				SelfBuffName = "Lucky Shots",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat < 70)),
				IsRejuvenationComplete =
					torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat >= 95)),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.Trooper,
				RejuvenateAbilityName = "Recharge and Reload",
				SelfBuffName = "Fortification",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat < 70)),
				IsRejuvenationComplete = torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat > 95)),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},

			// Empire
			new ClassTunables
			{
				Class = CharacterClass.Warrior,
				RejuvenateAbilityName = "Channel Hatred",
				SelfBuffName = "Unnatural Might",
				IsRejuvenationNeeded = torPlayer => (torPlayer.HealthPercent < 70),
				IsRejuvenationComplete = torPlayer => (torPlayer.HealthPercent >= 95),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.Inquisitor,
				RejuvenateAbilityName = "Seethe",
				SelfBuffName = "Mark of Power",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat < 70)),
				IsRejuvenationComplete =
					torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat >= 95)),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.Agent,
				RejuvenateAbilityName = "Recuperate",
				SelfBuffName = "Coordination",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat < 70)),
				IsRejuvenationComplete =
					torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat >= 95)),
				NormalizedResource = torPlayer => (torPlayer.ResourceStat)
			},
			new ClassTunables
			{
				Class = CharacterClass.BountyHunter,
				RejuvenateAbilityName = "Recharge and Reload",
				SelfBuffName = "Hunter's Boon",
				IsRejuvenationNeeded = torPlayer => ((torPlayer.HealthPercent < 70) || (torPlayer.ResourceStat > 30)),
				IsRejuvenationComplete = torPlayer => ((torPlayer.HealthPercent >= 95) && (torPlayer.ResourceStat <= 5)),
				NormalizedResource = torPlayer => (100.0f - Math.Min(torPlayer.ResourceStat, 100.0f))
			},

			// Boundary condition --
			new ClassTunables
			{
				Class = CharacterClass.Unknown,
				RejuvenateAbilityName = "UNDEFINED",
				SelfBuffName = "UNDEFINED",
				IsRejuvenationNeeded = torPlayer => (false),
				IsRejuvenationComplete = torPlayer => (true),
				NormalizedResource = torPlayer => (0.0f)
			}
		};

		// Derived data structure for quick lookup --
		private static readonly Dictionary<CharacterClass, ClassTunables> TunablesMap =
			TunablesByClass.ToDictionary(x => x.Class, x => x);

		public static bool HasMyBuff(this TorCharacter u, string aura)
		{
			return u.Buffs.Any(a => a.Name == aura && a.CasterGuid == BuddyTor.Me.Guid);
		}

		public static bool HasMyDebuff(this TorCharacter u, string aura)
		{
			return u.Debuffs.Any(a => a.Name == aura && a.CasterGuid == BuddyTor.Me.Guid);
		}

		public static int BuffCount(this TorCharacter p, string buffName)
		{
			return !p.HasMyBuff(buffName)
				? 0
				: p.Buffs.FirstOrDefault(b => b.Name.Contains(buffName) && b.CasterGuid == BuddyTor.Me.Guid).GetStacks();
		}

		public static int DebuffCount(this TorCharacter p, string buffName)
		{
			return !p.HasMyDebuff(buffName)
				? 0
				: p.Debuffs.FirstOrDefault(b => b.Name.Contains(buffName) && b.CasterGuid == BuddyTor.Me.Guid).GetStacks();
		}

		public static double BuffTimeLeft(this TorCharacter p, string buffName)
		{
			return !p.HasMyBuff(buffName)
				? 0
				: p.Buffs.FirstOrDefault(b => b.Name.Contains(buffName) && b.CasterGuid == BuddyTor.Me.Guid).TimeLeft.TotalSeconds;
		}

		public static double DebuffTimeLeft(this TorCharacter p, string buffName)
		{
			return !p.HasMyDebuff(buffName)
				? 0
				: p.Debuffs.FirstOrDefault(b => b.Name.Contains(buffName) && b.CasterGuid == BuddyTor.Me.Guid).TimeLeft.TotalSeconds;
		}

		public static bool HasDebuffCount(this TorCharacter p, string debuff, int stacks)
		{
			return p.HasDebuff(debuff) && p.Debuffs.Any(d => d.Name.Contains(debuff) && d.GetStacks() >= stacks);
		}

		public static bool NeedsCleanse(this TorCharacter p)
		{
			foreach (var d in DebuffList)
			{
				if (d.Stacks == 0 && p.HasDebuff(d.Name))
					return true;

				if (d.Stacks > 0 && p.HasDebuffCount(d.Name, d.Stacks))
					return true;
			}
			return false;
		}

		public static bool IsCrowdControlled(this TorCharacter torCharacter)
		{
			return (torCharacter.Debuffs.FirstOrDefault(d => DebuffNames_CrowdControl.Contains(d.Name)) != null);
		}

		public static int GetStacks(this TorEffect t)
		{
			var result = 0;
			try
			{
				result = t.Stacks;
			}
			catch
			{
				result = 1;
			}
			return result;
		}

		public static bool IsValidTarget(this TorCharacter c)
		{
			return c != null && c.InCombat && c.IsHostile && !c.IsDead && !c.IsStunned && !c.IsCrowdControlled();
		}

		public static bool BossOrGreater(this TorCharacter unit)
		{
			if (unit != null) if (unit.Toughness >= CombatToughness.Boss1) return true;
			return false;
		}

		public static bool StrongOrGreater(this TorCharacter unit)
		{
			if (unit != null) if (unit.Toughness >= CombatToughness.Strong) return true;
			return false;
		}

		public static bool IsHealer(this TorPlayer p)
		{
			var result = false;
			switch (p.Discipline)
			{
				case CharacterDiscipline.CombatMedic:
					result = true;
					break;
				case CharacterDiscipline.Bodyguard:
					result = true;
					break;
				case CharacterDiscipline.Medicine:
					result = true;
					break;
				case CharacterDiscipline.Seer:
					result = true;
					break;
				case CharacterDiscipline.Sawbones:
					result = true;
					break;
				case CharacterDiscipline.Corruption:
					result = true;
					break;
			}
			return result;
		}

		public static bool IsInCover(this TorCharacter torCharacter)
		{
			return (torCharacter.Buffs.Any(b => BuffNamesForCoverVariants.Contains(b.Name)));
		}

		public static bool IsBehind(this TorCharacter torCharacter, TorCharacter Target)
		{
			return (Math.Abs(BuddyTor.Me.Heading - Target.Heading) <= 150); // && CurrentTarget.IsInRange(0.35f)
		}

		public static float ResourcePercent(this TorPlayer torPlayer)
		{
			return (TunablesMap[torPlayer.Class].NormalizedResource(torPlayer));
		}

		public static string RejuvenateAbilityName(this TorPlayer torPlayer)
		{
			return (TunablesMap[torPlayer.Class].RejuvenateAbilityName);
		}

		public static string SelfBuffName(this TorPlayer torPlayer)
		{
			return (TunablesMap[torPlayer.Class].SelfBuffName);
		}

		private delegate bool PredicateDelegate(TorPlayer torPlayer);

		private delegate float NormalizedResourceDelegate(TorPlayer torPlayer);

		private class ClassTunables
		{
			/// <summary>
			///     The <c>CharacterClass</c> to which this set of <c>ClassTunables</c> applies.
			/// </summary>
			public CharacterClass Class = CharacterClass.Unknown;

			public PredicateDelegate IsRejuvenationComplete = torPlayer => (true);
			public PredicateDelegate IsRejuvenationNeeded = torPlayer => (false);
			public NormalizedResourceDelegate NormalizedResource = torPlayer => (0.0f);
			public string RejuvenateAbilityName = "UNDEFINED";
			public string SelfBuffName = "UNDEFINED";
		};
	}

	public class Debuff
	{
		public string Name;
		public int Stacks;

		public Debuff(string name, int stacks)
		{
			Name = name;
			Stacks = stacks;
		}
	}
}