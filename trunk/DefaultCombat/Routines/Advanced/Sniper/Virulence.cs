// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Virulence : RotationBase
	{
		public override string Name
		{
			get { return "Sniper Virulence"; }
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
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Laze Target", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Cast("Target Acquired", ret => Me.CurrentTarget.StrongOrGreater())
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
					
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Low Energy
					Spell.Cast("Rifle Shot", ret => Me.EnergyPercent < 60),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.DoT("Corrosive Dart", "Corrosive Dart"),
					Spell.DoT("Corrosive Grenade", "Corrosive Grenade"),
					Spell.Cast("Weakening Blast",	ret => Me.CurrentTarget.HasDebuff("Corrosive Dart") && Me.CurrentTarget.HasDebuff("Corrosive Grenade")),
					Spell.Cast("Cull",	ret =>	Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") > 3 && Me.CurrentTarget.DebuffTimeLeft("Corrosive Grenade") > 3),
					Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Lethal Takedown")),
					Spell.Cast("Series of Shots"),
					Spell.Cast("Lethal Shot"),
					Spell.Cast("Snipe")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 4f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Orbital Strike", ret => Me.IsInCover() && Me.EnergyPercent > 30),
						Spell.Cast("Fragmentation Grenade"),
						Spell.DoT("Corrosive Dart", "Corrosive Dart"),
						Spell.Cast("Corrosive Grenade",	ret => Me.CurrentTarget.HasDebuff("Corrosive Dart") && !Me.CurrentTarget.HasDebuff("Corrosive Grenade")),
						Spell.CastOnGround("Suppressive Fire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
						));
			}
		}
	}
}