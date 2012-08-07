using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.CommonBot.Logic;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class OperativeConcealment
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Agent, AdvancedClass.Operative, SkillTreeId.OperativeConcealment)]
        public static Composite OperativeConcealmentPull()
        {
            return new PrioritySelector(
                Movement.StopInRange(0.4f),
                Spell.WaitForCast(),

                // Heal companion if he's injured kthx!
                Spell.Cast("Recuperate", ctx => BuddyTor.Me.Companion, ret => BuddyTor.Me.Companion.HealthPercent < 95 && !BuddyTor.Me.InCombat),
                Spell.Cast("Kolto Infusion", ctx => BuddyTor.Me.Companion, ret => BuddyTor.Me.Companion.HealthPercent < 60),
                Spell.Cast("Kolto Injection", ctx => BuddyTor.Me.Companion, ret => BuddyTor.Me.Companion.HealthPercent < 60),

				// Cover so we can use our stuff
                // Spell.Cast("Take Cover", ret => !BuddyTor.Me.HasBuff("Cover") && !BuddyTor.Me.HasBuff("Crouch")),
                Spell.Cast("Crouch", ret => BuddyTor.Me.CurrentTarget.InLineOfSight && !BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover") && BuddyTor.Me.CurrentTarget.Distance <= 0.4f && BuddyTor.Me.InCombat),
                // Spell.Cast("Crouch", ret => !BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover") && BuddyTor.Me.CurrentTarget.Distance <= 0.4f && BuddyTor.Me.InCombat),

                Spell.Cast("Evasion", castWhen => (BuddyTor.Me.HealthPercent <= 20)),
                Spell.Cast("Kolto Infusion", castWhen => (BuddyTor.Me.HealthPercent <= 70)), // heals me - If I have Tactical Advantage buff
                Spell.Cast("Kolto Injection", castWhen => (BuddyTor.Me.HealthPercent <= 70)), // heals me
                Spell.Cast("Stim Boost", castWhen => BuddyTor.Me.ResourceStat <= 45), // restores Energy
                Spell.Cast("Adrenaline Probe", castWhen => BuddyTor.Me.ResourceStat <= 45), // restores Energy
				
				// cast Stim Boost if we have tactical advantage buff
             
				// Spell: Followthrough / Instant / Energy: 10 / Cooldown: 9s / Range: 30 m (Requires level 1)
                // Fires a well-controlled follow-up shot at the target that deals 1310 - 1532 weapon damage. !!!!Only usable within the 4.5 seconds immediately following a Snipe or Ambush!!!!
                Spell.Cast("Followthrough", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Ambush / Activation: 2.5 secs / Energy: 15 / Cooldown: 15s / Range: 30 m (Requires level 10)
                // Fires a high-powered shot that deals 2431 - 2755 weapon damage. Must be in cover to use. 
                Spell.Cast("Ambush", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

				Spell.Cast("Backstab", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
				Spell.Cast("Shiv", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
				
				Spell.Cast("Flash Bang", ret => BuddyTor.Me.CurrentTarget.Range <= 1f),
				//Spell.Cast("Backstab", ret => BuddyTor.Me.CurrentTarget.Range <= .4f), // If I am behind target
				Spell.Cast("Debilitate", ret => BuddyTor.Me.CurrentTarget.Range <= .4f && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak),

                // Spell: Explosive Probe / Instant / Energy: 20 / Cooldown: 30s / Range: 30 m (Requires level 1)
                // Desc: Puts an explosive probe on the target that detonates when the target takes damage, dealing 3016 - 3196 kinetic damage to the target. 
                // Standard and weak targets are additionally knocked down and set on fire, dealing 427 additional elemental damage over 3 seconds. Can only be used from cover.
                Spell.Cast("Explosive Probe", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Fragmentation Grenade / Instant / Energy: 20 / Cooldown: 6s / Range: 30 m (Requires level 3)
                // Desc: Throws a fragmentation grenade that deals 681 - 746 kinetic damage to up to 5 enemies in an 8-meter radius. If the primary target is standard or weak, it is knocked down by the blast.
                Spell.Cast("Corrosive Grenade"),
                Spell.Cast("Fragmentation Grenade", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .4 && !o.IsDead) >= 3),
                Spell.Cast("Carbine Burst", castWhen => ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .4 && !o.IsDead) >= 3),

                 Spell.Cast("Corrosive Dart", ctx => BuddyTor.Me.ResourceStat >= 80 && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak && !BuddyTor.Me.CurrentTarget.HasBuff("Poisoned (Tech)")),//Bleed
               // Spell: Series of Shots / Instant / Energy: 20 Cooldown: 15s / Range: 30 m (Requires level 36)
                // Desc: Unleashes a series of shots that deals 3353 - 3736 weapon damage over the duration. Must be in cover to use.
                Spell.Cast("Series of Shots", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Orbital Strike / Activation: 3 secs / Energy: 30 / Cooldown: 60s / Range: 30 m (Requires level 48)
                // Desc: Calls in support from orbiting warships, dealing 2082 elemental damage over 9 seconds to all enemies within 8 meters of the targeted area. Standard and weak enemies are additionally knocked down by the blasts. 
                Spell.Cast("Orbital Strike", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Takedown / Instant / Energy: 15 / Cooldown: 12s / Range: 30 m (Requires level 18)
                // Desc: Attempts to take the target down with a single powerful shot that deals 3625 - 3889 weapon damage. !!!!Only usable on targets at or below 30% max health!!!!
                Spell.Cast("Takedown", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),


                // Spell: Corrosive Dart / Instant / Energy: 20 / Range: 30 m (Requires level 5)
                // Desc: Fires a dart at the target that deals 2538 internal damage over 15 seconds.


                // Spell: Snipe / Activation: 1.5 secs / Energy: 20 / Range: 30 m (Requires level 1)
                // Desc: Shoots a target for 2736 - 2917 weapon damage. Can only be used from cover.
                Spell.Cast("Snipe", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
				
                // Spell: Overload Shot / Instant / Energy: 17 / Range: 10 m (Requires level 8)
                // Desc: Blasts a target for 917 - 1039 weapon damage. 
                Spell.Cast("Overload Shot", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Suppressive Fire / Instant / Energy: 35 / Range: 30 m (Requires level 22) 
                // Desc: Sprays a wave of bolts over the target area, dealing 3360 - 3611 weapon damage over the duration to up to 3 targets within 5 meters.
                Spell.Cast("Suppressive Fire", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),

                // Spell: Shatter Shot / Instant / Energy: 10 / Cooldown: 4.5s / Range: 30 m (Requires level 28)
                // Desc: Fires a high-impact shot that deals 499 - 555 weapon damage and reduces the target's armor by 20% for 45 seconds.
                Spell.Cast("Shatter Shot", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),
                Spell.Cast("Rifle Shot", ret => BuddyTor.Me.CurrentTarget.Range <= .4f),//No Resource needed

                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, 0.4f)
                );
        }
    }
}