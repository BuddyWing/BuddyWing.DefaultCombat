// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Vigilance : RotationBase
	{
		public override string Name
		{
			get { return "Guardian Vigilance"; }
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
					Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Resolute", ret => Me.IsStunned),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Focused Defense", ret => Me.HealthPercent < 70),
					Spell.Buff("Enure", ret => Me.HealthPercent <= 30),
					Spell.Buff("Combat Focus", ret => Me.ActionPoints <= 6)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Saber Throw",	ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Leap", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

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

					//Interrupts
					Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),

					//Rotation
					Spell.Cast("Plasma Brand"),
					Spell.Cast("Overhead Slash"),
					Spell.Cast("Blade Storm", ret => Me.BuffCount("Force Rush") == 2),
					Spell.Cast("Blade Dance"),
					Spell.Cast("Whirling Blade", ret => Me.HasBuff("Keening") || Me.HasBuff("Ardent Advocate") || Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Sundering Strike", ret => Me.ActionPoints <= 7),
					Spell.Cast("Slash", ret => Me.ActionPoints >= 9),
					Spell.Cast("Strike"),
					Spell.Cast("Riposte"),
					Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Vigilant Thrust",	ret =>	Me.Level >= 57 && Me.CurrentTarget.HasDebuff("Burning (Plasma Brand)") &&	Me.CurrentTarget.HasDebuff("Burning (Burning Purpose)") && Me.CurrentTarget.HasDebuff("Burning (Burning Blade)")),
						Spell.Cast("Force Sweep",	ret =>	Me.Level < 57 && Me.CurrentTarget.HasDebuff("Burning (Plasma Brand)") &&	Me.CurrentTarget.HasDebuff("Burning (Burning Purpose)") && Me.CurrentTarget.HasDebuff("Burning (Burning Blade)")),
						Spell.Cast("Cyclone Slash",	ret => Me.CurrentTarget.HasDebuff("Burning (Plasma Brand)") || Me.CurrentTarget.HasDebuff("Burning (Burning Purpose)") ||	Me.CurrentTarget.HasDebuff("Burning (Burning Blade)"))
						));
			}
		}
	}
}