// Copyright (C) 2011-2015 Bossland GmbH
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
				return new LockSelector(
					Spell.Buff("Escape"),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Laze Target"),
					Spell.Cast("Target Acquired")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new LockSelector(
							Spell.Cast("Rifle Shot", ret => !Me.IsInCover())
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Series of Shots"),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
					Spell.CastOnGround("Plasma Probe", ret => !Me.CurrentTarget.HasDebuff("Slowed")),
					Spell.DoT("Interrogation Probe", "Interrogation Probe"),
                    Spell.CastOnGround("Orbital Strike"),
                    Spell.Cast("Frag Grenade", ret => Me.HasBuff("Energy Overrides")),
					Spell.DoT("Corrosive Dart", "Corrosive Dart"),
					Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Snipe")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
                        Spell.CastOnGround("Plasma Probe", ret => !Me.CurrentTarget.HasDebuff("Slowed")),
                        Spell.CastOnGround("Orbital Strike"),
						Spell.Cast("Fragmentation Grenade")
						));
			}
		}
	}
}