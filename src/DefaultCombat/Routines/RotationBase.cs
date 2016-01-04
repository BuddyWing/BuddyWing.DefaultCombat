// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Core;

namespace DefaultCombat.Routines
{
	public abstract class RotationBase
	{
		/// <summary>
		///     Gets the local player.
		/// </summary>
		protected static TorPlayer Me
		{
			get { return BuddyTor.Me; }
		}

		/// <summary>
		///     Gets the character functioning as the tank.
		/// </summary>
		public static TorCharacter Tank
		{
			get { return Targeting.Tank; }
		}

		/// <summary>
		///     Gets the current heal target.
		/// </summary>
		public static TorCharacter HealTarget
		{
			get { return Targeting.HealTarget; }
		}

		/// <summary>
		///     Gets the rotation's name.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		///     Gets the buff logic for this routine.
		/// </summary>
		public abstract Composite Buffs { get; }

		/// <summary>
		///     Gets the cooldown usage logic for this routine.
		/// </summary>
		public abstract Composite Cooldowns { get; }

		/// <summary>
		///     Gets the single target logic for this routine.
		/// </summary>
		public abstract Composite SingleTarget { get; }

		/// <summary>
		///     Gets the area of effect logic for this routine.
		/// </summary>
		public abstract Composite AreaOfEffect { get; }
	}
}