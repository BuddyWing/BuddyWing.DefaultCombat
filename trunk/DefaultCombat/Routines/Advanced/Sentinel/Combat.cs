// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Combat : RotationBase
	{
		public override string Name
		{
			get { return "Sentinel Combat"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
					Spell.Buff("Resolute"),
					Spell.Buff("Rebuke", ret => Me.HealthPercent <= 90),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 70),
					Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 50),
					Spell.Buff("Unity", ret => Me.HealthPercent <= 30),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Valorous Call", ret => Me.BuffCount("Centering") < 5),
					Spell.Buff("Zen", ret => Me.CurrentTarget.Distance <= 0.4f)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Leap", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Dual Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),

					//Rotation
					Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Dispatch", ret => Me.HasBuff("Hand of Justice") || Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Precision", ret => Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Blade Barrage", ret => Me.HasBuff("Precision")),
					Spell.Cast("Clashing Blast", ret => Me.HasBuff("Opportune Attack") && Me.Level >= 57),
					Spell.Cast("Blade Storm", ret => Me.HasBuff("Opportune Attack") && Me.Level < 57),
					Spell.Cast("Blade Rush"),
					Spell.Cast("Lance"),
					Spell.Cast("Slash", ret => Me.ActionPoints >= 7 && Me.Level < 26),
					Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 7),
					Spell.Cast("Strike", ret => Me.ActionPoints <= 10)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Force Sweep"),
						Spell.Cast("Cyclone Slash")
						));
			}
		}
	}
}
