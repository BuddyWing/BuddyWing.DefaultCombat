// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Marksmanship : RotationBase
	{
		public override string Name
		{
			get { return "Sniper Marksmanship"; }
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
					Spell.Buff("Escape", ret => Me.IsStunned),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 70),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 40),
					Spell.Buff("Sniper Volley", ret => Me.EnergyPercent <= 60),
					Spell.Buff("Entrench", ret => Me.CurrentTarget.StrongOrGreater() && Me.IsInCover()),
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
					CombatMovement.CloseDistance(Distance.Ranged),

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new PrioritySelector(
							Spell.Cast("Rifle Shot")
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
					Spell.Cast("Followthrough"),
					Spell.Cast("Penetrating Blasts", ret => Me.Level >= 26),
					Spell.Cast("Series of Shots", ret => Me.Level < 26),
					Spell.DoT("Corrosive Dart", "Corrosive Dart"),
					Spell.Cast("Ambush", ret => Me.BuffCount("Zeroing Shots") == 2),
					Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Snipe"),
					Spell.Cast("Maim")
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
						Spell.Cast("Fragmentation Grenade"),
						Spell.CastOnGround("Suppressive Fire")
						));
			}
		}
	}
}