// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Concealment : RotationBase
	{
		public override string Name
		{
			get { return "Operative Concealment"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination"),
					Spell.Cast("Stealth", ret => !DefaultCombat.MovementDisabled && !Me.InCombat && !Me.HasBuff("Coordination"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") <= 2),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 50)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Backstab", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() < 60,
						new LockSelector(
							Spell.Cast("Backstab", ret => (Me.IsBehind(Me.CurrentTarget) || Me.HasBuff("Jarring Strike"))),
							Spell.Cast("Rifle Shot")
							)),
					Spell.Cast("Backstab", ret => Me.IsBehind(Me.CurrentTarget)),
					Spell.Cast("Crippling Slice"),
					Spell.DoT("Corrosive Dart", "", 12000),
					Spell.Cast("Veiled Strike", ret => Me.Level >= 41),
					Spell.Cast("Shiv", ret => Me.Level < 41),
					Spell.Cast("Laceration", ret => Me.HasBuff("Tactical Advantage")),
					Spell.Cast("Overload Shot", ret => Me.EnergyPercent >= 87)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Fragmentation Grenade"),
						Spell.Cast("Carbine Burst", ret => Me.HasBuff("Tactical Advantage")))
					);
			}
		}
	}
}