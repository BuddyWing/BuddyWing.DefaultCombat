// Copyright (C) 2011-2016 Bossland GmbH
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
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
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
					
					//Legacy Heroic Moment Abilities
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),

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
						Spell.DoT("Corrosive Dart", "Corrosive Dart"),
						Spell.Cast("Corrosive Grenade",	ret => Me.CurrentTarget.HasDebuff("Corrosive Dart") && !Me.CurrentTarget.HasDebuff("Corrosive Grenade")),
						Spell.CastOnGround("Suppressive Fire")
						));
			}
		}
	}
}