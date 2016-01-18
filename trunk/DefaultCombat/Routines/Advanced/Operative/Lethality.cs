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
				return new PrioritySelector(
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") < 1)
				//	Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75)
				//	Spell.Buff("Evasion", ret => Me.HealthPercent <= 50)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Hidden Strike", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Lethal Strike",	
						ret => 
						Me.IsStealthed)
						,
					Spell.Cast("Corrosive Dart", 
						ret => 
						(!Me.CurrentTarget.HasDebuff("Corrosive Dart") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") <= 2) &&
						!Me.IsStealthed)
						,
					Spell.Cast("Corrosive Grenade", 
						ret => 
					//	Me.HasBuff("Cut Down") && 
						(!Me.CurrentTarget.HasDebuff("Corrosive Grenade") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Grenade") <= 2) &&
						!Me.IsStealthed)
						,
					Spell.Cast("Corrosive Assault", 
						ret => 
						Me.HasBuff("Tactical Advantage") && 
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
						!Me.IsStealthed)
						,
					Spell.Cast("Toxic Blast", 
						ret => 
						Me.BuffCount("Tactical Advantage") < 2 && 
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
						!Me.IsStealthed)
						,
					Spell.Cast("Shiv", 
						ret => 
						Me.BuffCount("Tactical Advantage") < 2 && 
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
						!Buddy.CommonBot.AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) &&
						!Me.IsStealthed)
						,
					Spell.Cast("Lethal Strike",	
						ret => 
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade"))
						,
					Spell.Cast("Overload Shot",	
						ret => 
						Me.EnergyPercent > 85 &&
						!Me.HasBuff("Tactical Advantage") && 
						!Buddy.CommonBot.AbilityManager.CanCast("Shiv", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Lethal Strike", Me.CurrentTarget) &&
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
						!Me.IsStealthed)
						,
					Spell.Cast("Rifle Shot",
						ret =>
						Me.EnergyPercent < 85 &&
						!Me.HasBuff("Tactical Advantage") && 
						!Buddy.CommonBot.AbilityManager.CanCast("Shiv", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) &&
						!Buddy.CommonBot.AbilityManager.CanCast("Lethal Strike", Me.CurrentTarget) &&
						Me.CurrentTarget.HasDebuff("Corrosive Dart") && 
						Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
						!Me.IsStealthed)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Corrosive Grenade", 
							ret => 
							!Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
							!Me.IsStealthed)
							,
						Spell.Cast("Fragmentation Grenade", 
							ret => 
							Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
							!Me.IsStealthed)
							,
						Spell.Cast("Carbine Burst", 
							ret => 
							Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&
							!Me.IsStealthed)
						)
					);
			}
		}
	}
}
