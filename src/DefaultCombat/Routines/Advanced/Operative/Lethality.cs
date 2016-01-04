// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Lethality : RotationBase
	{
		public override string Name
		{
			get { return "Operative Lethality"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination"),
					Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Coordination"))
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
					Spell.Cast("Hidden Strike", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Rifle Shot", ret => Me.ResourcePercent() < 60),
					Spell.Cast("Hidden Strike",
						ret => !Me.HasBuff("Tactical Advantage") && Me.IsBehind(Me.CurrentTarget) && Me.IsStealthed),
					Spell.DoT("Corrosive Dart", "", 15000),
					Spell.DoT("Corrosive Grenade", "", 18000),
					Spell.Cast("Weakening Blast"),
					Spell.Cast("Cull", ret => Me.HasBuff("Tactical Advantage")),
					Spell.Cast("Shiv", ret => Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") < 6),
					Spell.Cast("Backstab", ret => Me.IsBehind(Me.CurrentTarget)),
					Spell.Cast("Overload Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("Orbital Strike"),
						Spell.DoT("Corrosive Grenade", "", 18000),
						Spell.Cast("Fragmentation Grenade"),
						Spell.Cast("Carbine Burst", ret => Me.HasBuff("Tactical Advantage")))
					);
			}
		}
	}
}