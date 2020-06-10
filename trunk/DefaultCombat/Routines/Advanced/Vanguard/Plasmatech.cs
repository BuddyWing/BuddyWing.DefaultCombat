// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Plasmatech : RotationBase
    {
        public override string Name
        {
            get { return "Vanguard Plasmatech"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Fortification")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Tenacity", ret => Me.IsStunned),
                    Spell.Cast("Reserve Powercell", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
                    Spell.Cast("Battle Focus", ret => Me.InCombat && Me.CurrentTarget.BossOrGreater()),
                    Spell.Cast("Recharge Cells", ret => Me.ResourcePercent() <= 40),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 60),
                    Spell.Cast("Shoulder Cannon"),
                    Spell.DoT("Incendiary Round", "Burning (Incendiary Round)"),
                    Spell.DoT("Plasmatize", "Plasmatize"),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Storm", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Low Energy
                    new Decorator(ret => Me.ResourcePercent() <= 40,
                        new PrioritySelector(
                            Spell.Cast("Ion Pulse", ret => Me.HasBuff("Plasma Barrage")),
                            Spell.Cast("Hammer Shot")
                            )),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Riot Strike", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Shockstrike"),
                    Spell.Cast("Ion Pulse", ret => Me.HasBuff("Plasma Barrage")),
                    Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon")),
                    Spell.Cast("Ion Wave", ret => Me.BuffCount("Pulse Generator") == 2),
                    Spell.Cast("High Impact Bolt", ret => Me.CurrentTarget.HasDebuff("Burning")),
                    Spell.Cast("Plasma Flare", ret => Me.HasBuff("Overcharged Plasma")),
                    Spell.Cast("Ion Pulse", ret => Me.ResourcePercent() >= 65)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => Targeting.ShouldAoe,
                        new PrioritySelector(
                            Spell.DoT("Plasmatize", "Plasmatize"),
                            Spell.CastOnGround("Artillery Blitz")
                            )),
                    new Decorator(ret => Targeting.ShouldPbaoe,
                        new PrioritySelector(
                            Spell.DoT("Plasmatize", "Plasmatize"),
                            Spell.Cast("Ion Wave"),
                            Spell.Cast("Explosive Surge"),
                            Spell.Cast("Flak Shell")
                            )));
            }
        }
    }
}
