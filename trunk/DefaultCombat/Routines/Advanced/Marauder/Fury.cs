// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Fury : RotationBase
	{
		public override string Name
		{
			get { return "Marauder Fury"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Shii-Cho Form"),
					Spell.Buff("Unnatural Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 50),
					Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 10),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Force Charge",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Smash", ret => Me.BuffCount("Shockwave") == 3 && Me.HasBuff("Dominate")),
					Spell.Cast("Force Crush"),
					Spell.Cast("Obliterate", ret => Me.HasBuff("Shockwave")),
					Spell.Cast("Force Scream", ret => Me.HasBuff("Battle Cry") || Me.ActionPoints >= 5),
					Spell.Cast("Dual Saber Throw"),
					Spell.Cast("Ravage"),
					Spell.Cast("Force Choke"),
					Spell.Cast("Vicious Slash", ret => Me.HasBuff("Berserk")),
					Spell.Cast("Battering Assault"),
					Spell.Cast("Assault")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Smash"),
						Spell.Cast("Sweeping Slash")
						));
			}
		}
	}
}