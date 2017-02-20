// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

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
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
					Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") < 1),
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
					Spell.Cast("Hidden Strike", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

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
					Spell.Cast("Lethal Strike",	ret => Me.IsStealthed),
					Spell.Cast("Corrosive Dart", ret =>	(!Me.CurrentTarget.HasDebuff("Corrosive Dart") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") <= 2) && !Me.IsStealthed),
					Spell.Cast("Corrosive Grenade",	ret => (!Me.CurrentTarget.HasDebuff("Corrosive Grenade") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Grenade") <= 2) &&	!Me.IsStealthed),
					Spell.Cast("Corrosive Assault",	ret =>	Me.HasBuff("Tactical Advantage") &&	Me.CurrentTarget.HasDebuff("Corrosive Dart") &&	Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !Me.IsStealthed),
					Spell.Cast("Toxic Blast",	ret => Me.BuffCount("Tactical Advantage") < 2 && Me.CurrentTarget.HasDebuff("Corrosive Dart") &&	Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !Me.IsStealthed),
					Spell.Cast("Shiv", ret => 	Me.BuffCount("Tactical Advantage") < 2 && 	Me.CurrentTarget.HasDebuff("Corrosive Dart") &&	Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) && !Me.IsStealthed),
					Spell.Cast("Lethal Strike",	ret =>	Me.CurrentTarget.HasDebuff("Corrosive Dart") &&	Me.CurrentTarget.HasDebuff("Corrosive Grenade")),
					Spell.Cast("Overload Shot",	ret =>	Me.EnergyPercent > 85 && !Me.HasBuff("Tactical Advantage") &&	!AbilityManager.CanCast("Shiv", Me.CurrentTarget) && !AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) && !AbilityManager.CanCast("Lethal Strike", Me.CurrentTarget) &&	Me.CurrentTarget.HasDebuff("Corrosive Dart") && 	Me.CurrentTarget.HasDebuff("Corrosive Grenade") &&	!Me.IsStealthed),
					Spell.Cast("Rifle Shot", ret =>	Me.EnergyPercent < 85 && !Me.HasBuff("Tactical Advantage") && !AbilityManager.CanCast("Shiv", Me.CurrentTarget) && !AbilityManager.CanCast("Toxic Blast", Me.CurrentTarget) &&	!AbilityManager.CanCast("Lethal Strike", Me.CurrentTarget) &&	Me.CurrentTarget.HasDebuff("Corrosive Dart") &&	Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !Me.IsStealthed)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Corrosive Grenade",	ret => !Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !Me.IsStealthed),
						Spell.Cast("Fragmentation Grenade",	ret =>	Me.CurrentTarget.HasDebuff("Corrosive Grenade") && !Me.IsStealthed),
						Spell.Cast("Noxious Knives"),
						Spell.Cast("Toxic Haze", ret => Me.HasBuff("Tactical Advantage"))
						));
			}
		}
	}
}