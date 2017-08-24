// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class ShieldSpecialist : RotationBase
	{
		public override string Name
		{
			get { return "Vanguard Shield Specialist"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Fortification"),
					Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Tenacity", ret => Me.IsStunned),
					Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 40),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Shoulder Cannon"),
					Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
					Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Storm", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Rotation
					new Decorator(ret => Me.ResourcePercent() < 60,
						new PrioritySelector(
							Spell.Cast("Energy Blast", ret => Me.BuffCount("Power Screen") == 3),
							Spell.Cast("Pulse Cannon", ret => Me.HasBuff("Pulse Engine") && Me.CurrentTarget.Distance <= 1f),
							Spell.Cast("Stockstrike", ret => Me.CurrentTarget.Distance <= .4f),
							Spell.Cast("Explosive Surge", ret => Me.HasBuff("Static Surge") && Me.CurrentTarget.Distance <= 0.5f),
							Spell.Cast("Hammer Shot")
							)),
					Spell.Cast("Riot Strike", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.CastOnGround("Smoke Grenade", ret => Me.CurrentTarget.Distance <= 0.8f),
					Spell.Cast("Shoulder Cannon"),
					Spell.Cast("Energy Blast", ret => Me.BuffCount("Power Screen") == 3),
					Spell.Cast("Stockstrike"),
					Spell.Cast("High Impact Bolt"),
					Spell.Cast("Pulse Cannon", ret => Me.HasBuff("Pulse Engine") && Me.CurrentTarget.Distance <= 1f),
					Spell.Cast("Explosive Surge", ret => Me.HasBuff("Static Surge") && Me.CurrentTarget.Distance <= 0.5f),
					Spell.Cast("Ion Pulse"),

					//HK-55 Mode Rotation
					Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
					Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
						Spell.CastOnGround("Artillery Blitz"),
						Spell.Cast("Ion Wave", ret => Me.CurrentTarget.Distance <= 1f),
						Spell.Cast("Flak Shell", ret => Me.CurrentTarget.Distance <= 1f),
						Spell.Cast("Explosive Surge", ret => Me.CurrentTarget.Distance <= .5f)
						));
			}
		}
	}
}
