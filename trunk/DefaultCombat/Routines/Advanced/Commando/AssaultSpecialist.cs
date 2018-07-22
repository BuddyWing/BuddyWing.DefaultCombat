// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class AssaultSpecialist : RotationBase
    {
        public override string Name
        {
            get { return "Commando Assault Specialist"; }
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
                    Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 40),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10),
                    Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),
                    Spell.Cast("Echoing Deterrence", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => Me.ResourcePercent() < 60,
                        new PrioritySelector(
                            Spell.Cast("Mag Bolt", ret => Me.HasBuff("Ionic Accelerator")),
                            Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level < 57),
                            Spell.Cast("Hammer Shot")
                            )),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Solo Mode
                    Spell.Cast("Med Shot", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
                    Spell.Cast("Bacta Infusion", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
                    Spell.Cast("Medical Probe", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Mag Bolt", ret => Me.HasBuff("Ionic Accelerator")),
                    Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level < 57),
                    Spell.Cast("Explosive Round", ret => Me.HasBuff("Hyper Assault Rounds")),
                    Spell.Cast("Assault Plastique"),
                    Spell.DoT("Serrated Bolt", "Bleeding"),
                    Spell.DoT("Incendiary Round", "Burning (Incendiary Round)"),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Full Auto"),
                    Spell.Cast("Mag Bolt"),
                    Spell.Cast("High Impact Bolt", ret => Me.Level < 57),
                    Spell.Cast("Charged Bolts")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.CastOnGround("Mortar Volley"),
                        Spell.DoT("Serrated Bolt", "Bleeding"),
                        Spell.DoT("Incendiary Round", "Burning (Incendiary Round)"),
                        Spell.Cast("Plasma Grenade", ret => Me.CurrentTarget.HasDebuff("Burning (Incendiary Round)")),
                        Spell.Cast("Sticky Grenade", ret => Me.CurrentTarget.HasDebuff("Bleeding")),
                        Spell.Cast("Explosive Round", ret => Me.HasBuff("Hyper Assault Rounds")),
                        Spell.CastOnGround("Hail of Bolts", ret => Me.ResourcePercent() >= 90)
                        ));
            }
        }
    }
}
