// Copyright (C) 2011-2016 Bossland GmbH
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
				return new PrioritySelector(
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") < 2),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 50)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Backstab", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() < 60 && !Buddy.CommonBot.AbilityManager.CanCast("Adrenaline Probe", Me),
						new PrioritySelector(
							Spell.Cast("Rifle Shot")
							)),
					new Decorator(
						ret => 
						(Me.CurrentTarget.HasDebuff("Poisoned (Acid Blade)") || Me.CurrentTarget.HasDebuff("Corrosive Dart")) &&
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Volatile Substance")))
						,
					new Decorator(
						ret => 
						Me.HasBuff("Tactical Advantage") && 
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Laceration")))
						,
					new Decorator(
						ret => 
						(!Me.CurrentTarget.HasDebuff("Corrosive Dart") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") <= 2) &&
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Corrosive Dart")))
						,
					new Decorator(
						ret => 
						!Me.HasBuff("Tactical Advantage") &&
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Veiled Strike")))
						,
					new Decorator(
						ret => 
						!Me.HasBuff("Tactical Advantage") &&
						!Buddy.CommonBot.AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) &&
						Me.IsBehind(Me.CurrentTarget),
						new PrioritySelector(Spell.Cast("Backstab")))
						,
					new Decorator(
						ret => 
						!Me.HasBuff("Tactical Advantage") &&
						!Buddy.CommonBot.AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Backstab", Me.CurrentTarget) &&
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Crippling Slice")))
						,
					new Decorator(
						ret => 
						!Me.HasBuff("Tactical Advantage") &&
						Me.EnergyPercent >= 87 &&
						!Buddy.CommonBot.AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Crippling Slice", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Backstab", Me.CurrentTarget) &&
						!Me.IsStealthed,
						new PrioritySelector(Spell.Cast("Overload Shot")))
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Fragmentation Grenade"),
						Spell.Cast("Carbine Burst"))
					);
			}
		}
	}
}
