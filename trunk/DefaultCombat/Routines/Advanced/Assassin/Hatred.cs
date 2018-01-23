// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Hatred : RotationBase
    {
        public override string Name
        {
            get { return "Assasin Hatred"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power"),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
                    Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Recklessness"),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
                    Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Phantom Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Force
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent <= 40),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.CastOnGround("Death Field"),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Bloodletting")),
                    Spell.Cast("Eradicate", ret => Me.HasBuff("Raze") && Me.Level >= 26),
                    Spell.DoT("Discharge", "Shocked (Discharge)"),
                    Spell.DoT("Creeping Terror", "Creeping Terror"),
                    Spell.Cast("Leeching Strike"),
                    Spell.Cast("Thrash", ret => Me.Force > 70),
                    Spell.Buff("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat),

                    //HK-55 Mode Rotation
                    Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
                    Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.CastOnGround("Death Field"),
                        Spell.DoT("Discharge", "Shocked (Discharge)"),
                        Spell.DoT("Creeping Terror", "Creeping Terror"),
                        Spell.Cast("Lacerate", ret => Me.CurrentTarget.HasDebuff("Shocked (Discharge)") && Me.CurrentTarget.HasDebuff("Creeping Terror") && Me.ForcePercent >= 60 && Targeting.ShouldPbaoe)
                        ));
            }
        }
    }
}
