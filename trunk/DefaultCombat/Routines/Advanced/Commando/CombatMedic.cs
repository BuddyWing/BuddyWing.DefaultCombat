// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using DefaultCombat.Extensions;

namespace DefaultCombat.Routines
{
    public class CombatMedic : RotationBase
    {
        public override string Name
        {
            get { return "Commando Combat Medic"; }
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
                    Spell.Buff("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10 && Me.ResourcePercent() <= 80 && HealTarget.HealthPercent <= 80),
                    Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() >= 70),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 40),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Echoing Deterrence", ret => Me.HealthPercent <= 20),
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
                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Concussion Charge", ret => Me.InCombat && Me.CurrentTarget.Distance <= 0.4f && Me.CurrentTarget.IsHostile),
                    Spell.Cast("Hammer Shot", ret => Me.ResourcePercent() > 40),
                    Spell.Cast("Full Auto", ret => Me.Level < 57),
                    Spell.Cast("High Impact Bolt"),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Charged Bolts", ret => Me.ResourceStat <= 70),
                    Spell.Cast("Explosive Round")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(

                        //Legacy Heroic Moment Ability
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--

                        //Solo Mode
                        Spell.Buff("Reserve Powercell", ret => CombatHotkeys.EnableSolo),
                        Spell.CastOnGround("Mortar Volley", ret => Me.HasBuff("Reserve Powercell") && CombatHotkeys.EnableSolo && Me.InCombat && Me.CurrentTarget.IsHostile),
                        Spell.Cast("Sticky Grenade", ret => CombatHotkeys.EnableSolo),
                        Spell.Cast("Plasme Grenade", ret => CombatHotkeys.EnableSolo && Me.ResourcePercent() <= 35 && Me.CurrentTarget.IsHostile),
                        Spell.CastOnGround("Hail of Bolts", ret => CombatHotkeys.EnableSolo && Me.ResourcePercent() <= 35 && Me.InCombat && Me.CurrentTarget.IsHostile),

                        //Cleanse
                        //NEWCODE

                        //Buff Party
                        Spell.Heal("Trauma Probe", on => HealTarget, 100, ret => !HealTarget.HasBuff("Trauma Probe")),

                        //AoE Healing 
                        new Decorator(ctx => Tank != null,
                            Spell.CastOnGround("Kolto Bomb", on => HealTarget.Position, ret => HealTarget.HealthPercent < 85 && !HealTarget.HasBuff("Kolto Residue") && Me.InCombat)),

                        //Free, so use it!
                        Spell.Heal("Bacta Infusion", 80, ret => Me.InCombat),

                        //Important Buffs to take advantage of
                        new Decorator(ctx => Tank != null,
                            Spell.CastOnGround("Kolto Bomb", on => HealTarget.Position, ret => Me.HasBuff("Supercharged Cell") && Me.InCombat)),

                        Spell.Heal("Medical Probe", 80, ret => Me.HasBuff("Field Triage") && Me.InCombat),
                        Spell.Heal("Advanced Medical Probe", 80, ret => Me.HasBuff("Supercharged Cell") && Me.InCombat),

                        //Single Target Priority
                        Spell.Heal("Advanced Medical Probe", 75, ret => Me.BuffCount("Field Triage") >= 3),
                        Spell.Heal("Medical Probe", 88, ret => Me.ResourcePercent() <= 50),
                        Spell.Heal("Successive Treatment", 99),
                        Spell.Heal("Med Shot", 95),
                        Spell.Heal("Bacta Infusion", 85),
                        Spell.Heal("Advanced Medical Probe", 65),

                        //Filler
                        Spell.Heal("Med Shot", 95, ret => Me.IsMoving),
                        Spell.HoT("Successive Treatment", 85, ret => Me.IsMoving)
                        );
            }
        }
    }
}
