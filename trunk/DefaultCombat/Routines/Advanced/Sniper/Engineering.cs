// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Engineering : RotationBase
	{
		public override string Name
		{
			get { return "Sniper Engineering"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Escape"),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Laze Target"),
					Spell.Buff("Target Acquired")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 30,
						new PrioritySelector(
							Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
							Spell.Cast("Rifle Shot")
							)),

					//Burn-ish
					new Decorator(ret => Me.CurrentTarget.HealthPercent < 30,
						new PrioritySelector(
							Spell.Cast("Explosive Probe"),
							Spell.Cast("Series of Shots"),
							Spell.DoTGround("Plasma Probe", 8500),
							Spell.DoT("Interrogation Probe", "Interrogation Probe"),
							Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
							Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
							Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Marked [Physical]")),
							Spell.Cast("Takedown"),
							Spell.CastOnGround("Orbital Strike")
							)),


					//Rotation
					Spell.CastOnGround("Orbital Strike", ret => Me.HasBuff("Target Acquired")),
					Spell.Cast("Explosive Probe"),
					Spell.Cast("Series of Shots"),
					Spell.DoTGround("Plasma Probe", 8500),
					Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
					Spell.DoT("Interrogation Probe", "Interrogation Probe"),
					Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
					Spell.CastOnGround("Orbital Strike"),
					Spell.DoT("Corrosive Dart", "Corrosive Dart"),
					Spell.Cast("Fragmentation Grenade"),
					Spell.Cast("Rifle Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.CastOnGround("Orbital Strike"),
						Spell.DoTGround("Plasma Probe", 9000),
						Spell.Cast("Suppressive Fire")
						));
			}
		}
	}
}