// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Sharpshooter : RotationBase
	{
		public override string Name
		{
			get { return "Gunslinger Sharpshooter"; }
		}


		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots")
					);
			}
		}


		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Escape"),
					Spell.Buff("Burst Volley"),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 70),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Smuggler's Luck"),
					Spell.Buff("Illegal Mods")
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
							Spell.Cast("Flurry of Bolts")
							)),
							
							
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
					Spell.Cast("Trickshot"),
					Spell.Cast("Penetrating Rounds", ret => Me.Level >= 26),
					Spell.Cast("Speed Shot", ret => Me.Level < 26),
					Spell.DoT("Vital Shot", "Vital Shot"),
					Spell.Cast("Aimed Shot", ret => Me.BuffCount("Charged Aim") == 2),
					Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Charged Burst"),
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
						Spell.CastOnGround("XS Freighter Flyby"),
						Spell.Cast("Thermal Grenade"),
						Spell.CastOnGround("Sweeping Gunfire")
						));
			}
		}
	}
}