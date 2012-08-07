using System.Linq;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.CommonBot.Logic;
using Buddy.Swtor;
using DefaultCombat.Dynamics;

namespace DefaultCombat.Routines
{
    public static class OperativeMedic
    {
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Agent, AdvancedClass.Operative, SkillTreeId.OperativeMedic)]
        public static Composite OperativeMedicPull()
        {
            return new PrioritySelector(
                Movement.StopInRange(0.4f),
                Spell.WaitForCast(),

                // We have stealth - I dont know if we need to put it in since we dont have any stealth attacks

            #region Cooldowns
                
                Spell.Cast("Stim Boost", ctx => BuddyTor.Me.ResourceStat <= 45), // Restores Energy - If I have Tactical Advantage buff
                Spell.Cast("Adrenaline Probe", ctx => BuddyTor.Me.ResourceStat <= 45), // Restores Energy

            #endregion

            #region Heal Me

                // 2.8f for heals
                // Heal me if Im below 50%, then heal Companion (unless its dead); If me and companion are > 90% HP - DPS

                new Decorator(ctx => BuddyTor.Me.HealthPercent <= 75, 
                    new PrioritySelector(
                Spell.Cast("Evasion", ctx => BuddyTor.Me.HealthPercent <= 20), // Immune to all dmg for 3s
                Spell.Cast("Kolto Infusion"), // Heals me - If I have Tactical Advantage buff
                Spell.Cast("Kolto Injection"), // Heals me
                Spell.Cast("Kolto Probe", ctx => !BuddyTor.Me.CurrentTarget.HasBuff("Kolto Probe (2)")), // HoT on me
                Spell.Cast("Diagnostic Scan", ctx => BuddyTor.Me.HealthPercent > 60 || BuddyTor.Me.ResourceStat < 25)
                )),

            #endregion

            #region Heal Companion
                // Lets heal our pet - If its not dead!
                new Decorator(ctx => !BuddyTor.Me.Companion.IsDead && BuddyTor.Me.Companion.HealthPercent <= 85,  
                    new PrioritySelector(
                Spell.Cast("Diagnostic Scan", ctx => BuddyTor.Me.Companion, ctx => (BuddyTor.Me.Companion.HealthPercent < 85 && BuddyTor.Me.Companion.HealthPercent > 75)), // Low HPS Channeled heal
                Spell.Cast("Kolto Probe", ctx => BuddyTor.Me.Companion, ctx => !BuddyTor.Me.CurrentTarget.HasBuff("Kolto Probe (2)")), // HoT on Pet
                Spell.Cast("Kolto Infusion", ctx => BuddyTor.Me.Companion, ctx => true),  // If I have Tactical Advantage buff
                Spell.Cast("Kolto Injection", ctx => BuddyTor.Me.Companion, ctx => true)
                )),

            #endregion

            #region DPS

                new Decorator(ctx => (BuddyTor.Me.HealthPercent > 80 && BuddyTor.Me.Companion.HealthPercent >= 90 && !BuddyTor.Me.Companion.IsDead) || (BuddyTor.Me.HealthPercent > 50 && BuddyTor.Me.Companion.IsDead),
                    new PrioritySelector(

                // DPS Stuff below - 0.4f for melee, 2.8f for ranged (default 0.4 for best dps)

                // Melee Attacks - can we force get in range?
                new Decorator(ctx => (BuddyTor.Me.CurrentTarget.Range <= .4f),
                    new PrioritySelector(
                Spell.Cast("Backstab"), // If we are behind the target; We need: get behind target and cast this (if possible)
                Spell.Cast("Shiv") // Melee attack
                )),

                // CCs and Dots
                new Decorator(ctx => (BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard && BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak),
                    new PrioritySelector(
                Spell.Cast("Flash Bang", ctx => BuddyTor.Me.CurrentTarget.Range <= 1f), // Blinds enemies
                Spell.Cast("Debilitate", ctx => BuddyTor.Me.CurrentTarget.Range <= .4f), // Stun Dart
                Spell.Cast("Corrosive Dart", ctx => !BuddyTor.Me.CurrentTarget.HasBuff("Poisoned (Tech)")) //Bleed Dart
                )),

                // AoE DPS
                new Decorator(ctx => (ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(o => o.Distance <= .4 && !o.IsDead) >= 3),
                    new PrioritySelector(
                Spell.Cast("Flash Bang", ctx => BuddyTor.Me.CurrentTarget.Range <= 1f), // Blinds enemies
                Spell.Cast("Fragmentation Grenade"), // AoE grenade
                        new Sequence(
                            new Action(ctx => BuddyTor.Me.CurrentTarget.Face()),
                            // wait 100ms to face ??
                            Spell.Cast("Carbine Burst", ret => BuddyTor.Me.CurrentTarget.Distance <= 1f)) //Frontal AoE cone burst fire
                )),

                // Cover so we can use our stuff
                Spell.Cast("Crouch", ctx => BuddyTor.Me.CurrentTarget.InLineOfSight && !BuddyTor.Me.HasBuff("Crouch") && !BuddyTor.Me.HasBuff("Cover") && BuddyTor.Me.CurrentTarget.Distance <= 0.4f),

                // Ranged or Melee Single Target DPS
                new Decorator(ctx => (BuddyTor.Me.CurrentTarget.InLineOfSight && BuddyTor.Me.CurrentTarget.Distance <= 2.8f),
                    new PrioritySelector(

                  new Decorator(ctx => (BuddyTor.Me.HasBuff("Crouch") || BuddyTor.Me.HasBuff("Cover")),
                      new PrioritySelector(
                Spell.Cast("Explosive Probe"), // Single Target Grenade - only avail in Crouch/Cover
                Spell.Cast("Snipe") // Medium DPS Shot - can only be used from Crouch/Cover
                )),

                Spell.Cast("Overload Shot"), // Medium-Low DPS Shot - Not really used unless we cant cover
                Spell.Cast("Rifle Shot") // Low DPS Shot - No Resource needed
                )),

            #endregion

                Movement.MoveTo(ctx => BuddyTor.Me.CurrentTarget.Position, 0.4f)
                )));
        }
    }
}