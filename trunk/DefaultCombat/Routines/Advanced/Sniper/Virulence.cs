// Copyright (C) 2011-2015 Bossland GmbH
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
				return new LockSelector(
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
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),

					//Low Energy
					Spell.Cast("Rifle Shot", ret => Me.EnergyPercent < 60),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.DoT("Corrosive Dart", "", 24000),
                    Spell.DoT("Corrosive Grenade", "", 24000),
                    Spell.Cast("Weakening Blast", ret => Me.CurrentTarget.HasDebuff("Poisoned (Tech)") && Me.CurrentTarget.HasDebuff("Poisoned (Corrosive Dart)")),
                    Spell.Cast("Cull", ret => Me.CurrentTarget.DebuffTimeLeft("Poisoned (Tech)") > 3 && Me.CurrentTarget.DebuffTimeLeft("Poisoned (Corrosive Dart)") > 3),
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Lethal Takedown")),
                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving && !DefaultCombat.MovementDisabled),
					Spell.Cast("Series of Shots"),
                    Spell.Cast("Lethal Shot"),
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
						Spell.Cast("Fragmentation Grenade"),
                        Spell.DoT("Corrosive Dart", "", 24000),
                        Spell.Cast("Corrosive Grenade", ret => Me.CurrentTarget.HasDebuff("Poisoned (Corrosive Dart)") && !Me.CurrentTarget.HasDebuff("Poisoned (Tech)")),
						Spell.CastOnGround("Suppressive Fire")
						));
			}
		}
	}
}