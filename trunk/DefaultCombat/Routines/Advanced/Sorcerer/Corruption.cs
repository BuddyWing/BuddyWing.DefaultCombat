using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Corruption : RotationBase
    {
        public override string Name { get { return "Sorcerer Corruption"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Recklessness", ret => Targeting.ShouldAOEHeal),
                    Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50)
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

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Crushing Darkness"),
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
                    Spell.Cast("Lightning Strike"),
                    Spell.Cast("Shock"));
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new LockSelector(
                    //BuffLog.Instance.LogTargetBuffs,

                    //Cleanse if needed
                    Spell.Cleanse("Purge"),

                    //Emergency Heal (Insta-cast)
                    Spell.Heal("Dark Heal", 40, ret => Me.HasBuff("Penetrating Darkness")),

                    //Single Target Healing
                    Spell.Heal("Innervate", 80),
                    Spell.HoT("Static Barrier", 75, ret => HealTarget != null && !HealTarget.HasDebuff("Deionized")),     

                    //Buff Tank
                    Spell.HoT("Static Barrier", on => Tank, 100, ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Deionized")),
                    
                    //Use Force Bending
                    new Decorator(ret => Me.HasBuff("Force Bending"),
                        new LockSelector(
                            Spell.Heal("Innervate", 90),
                            Spell.Heal("Dark Infusion", 50)
                        )),
                        
                    //Build Force Bending
                    Spell.HoT("Resurgence", 80),
                    Spell.HoT("Resurgence", on => Tank, 100, ret => Tank != null && Tank.InCombat),
                    
                    //Force Regen
                    Spell.Cast("Consumption", on => Me, ret => Me.HasBuff("Force Surge") && Me.HealthPercent > 15 && Me.ForcePercent < 80),

                    //Aoe Heal
                    Spell.HealGround("Revivification"),

                    //Single Target Healing                  
                    Spell.Heal("Dark Heal", 35),
                    Spell.Heal("Dark Infusion", 80));
            }
        }
    }
}