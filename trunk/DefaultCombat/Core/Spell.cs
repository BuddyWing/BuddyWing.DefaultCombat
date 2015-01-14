using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using Buddy.Common;
using Buddy.BehaviorTree;
using Buddy.Common.Math;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DefaultCombat.Helpers;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Core
{
    public static class Spell
    {
        private static Buddy.Swtor.Objects.TorPlayer Me { get { return BuddyTor.Me; } }
        public delegate TorCharacter UnitSelectionDelegate(object context);
        public delegate T Selection<out T>(object context);
        public static List<ExpiringItem> BlackListedSpells = new List<ExpiringItem>();

        public static Composite WaitForCast()
        {
            return new Decorator(ret => BuddyTor.Me.IsCasting,
                new Action(ret => RunStatus.Success));
        }

        #region Cast
        public static Composite Cast(string spell, Selection<bool> reqs = null)
        {
            return Cast(spell, ret => Me.CurrentTarget, reqs);
        }

        public static Composite Cast(string spell, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && AbilityManager.CanCast(spell, onUnit(ret))),
                    new PrioritySelector(
                        new Action(delegate
            {
                Logger.Write(">> Casting <<   " + spell);
                return RunStatus.Failure;
            }),
                        new Action(ret => AbilityManager.Cast(spell, onUnit(ret))))

                );
        }
                
        public static Composite CastOnGround(string spell, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret =>
                    (reqs == null || reqs(ret)) && Me.CurrentTarget != null,
                    CastOnGround(spell, ctx => Me.CurrentTarget.Position, ctx => true));
        }

        public static Composite CastOnGround(string spell, CommonBehaviors.Retrieval<Vector3> location, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret =>
                    (reqs == null || reqs(ret)) && location != null && location(ret) != Vector3.Zero &&
                    AbilityManager.CanCast(spell, BuddyTor.Me.CurrentTarget),
                    new Action(ret => AbilityManager.Cast(spell, location(ret))));
        }
        #endregion

        #region Buff
        public static Composite Buff(string spell, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret => (reqs == null || reqs(ret)) && !Me.HasBuff(spell),
                    Spell.Cast(spell, ret => Me, ret => true));

        }
        #endregion

        #region DoT
        public static Composite DoT(string spell, string debuff, float time = 0, Selection<bool> reqs = null)
        {
            return DoT(spell, ret => Me.CurrentTarget, debuff, time, reqs);
        }

        public static Composite DoT(string spell, UnitSelectionDelegate onUnit, string debuff, float time, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret))
                        && !onUnit(ret).HasMyDebuff(debuff) 
                        && AbilityManager.CanCast(spell, onUnit(ret)))
                        && !SpellBlackListed(spell, onUnit(ret).Guid),
                    new PrioritySelector(
                        new Action(ctx =>
                        {
                            BlackListedSpells.Add(new ExpiringItem(spell, (GetCooldown(spell) + 25 + time), onUnit(ctx).Guid));
                            Logger.Write(">> Casting <<   " + spell);
                            return RunStatus.Failure;
                        }),
                        new Action(ret => AbilityManager.Cast(spell, onUnit(ret)))));
        }

        public static float GetCastTime(string spell)
        {
            float castTime = 0;
            var v = AbilityManager.KnownAbilities.FirstOrDefault(a => a.Name.Contains(spell)).CastingTime;
            castTime += v * 1000;
            return castTime;
        }

        public static float GetCooldown(string spell)
        {
            float time = 0;
            var v = AbilityManager.KnownAbilities.FirstOrDefault(a => a.Name.Contains(spell)).CooldownTime;
            time += v * 1000;
            return time;
        }

        public static bool SpellBlackListed(string spell, float guid)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                BlackListedSpells.RemoveAll(s => s.item.Equals(""));
                return BlackListedSpells.Any(s => s.item.Equals(spell) && s.targetGuid == guid);
            }
        }
        #endregion

        #region Heal

        private static bool Target(TorCharacter onUnit)
        {
            onUnit.Target();
            return true;
        }

        public static Composite Cleanse(string spell, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => (Targeting.DispelTarget != null && (reqs == null || reqs(ret)) && Target(Targeting.DispelTarget)),
                Cast(spell, ret => Targeting.DispelTarget, reqs));
        }


        public static Composite Heal(string spell, int HP = 100, Selection<bool> reqs = null)
        {
            return Heal(spell, onUnit => Targeting.HealTarget, HP, reqs);
        }

        public static Composite Heal(string spell, UnitSelectionDelegate onUnit, int HP = 100, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && onUnit(ret).HealthPercent <= HP && Target(onUnit(ret))),
                Cast(spell, onUnit, reqs));
        }

        public static Composite HealAOE(string spell, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => ((reqs == null || reqs(ret)) && Targeting.ShouldAOEHeal && Targeting.AOEHealTarget != null),
                Cast(spell, onUnit => Targeting.AOEHealTarget, reqs));
        }

        public static Composite HoT(string spell, int HP = 100, Selection<bool> reqs = null)
        {
            return new Decorator( ret => Targeting.HealTarget != null, 
                HoT(spell, onUnit => Targeting.HealTarget, HP, reqs));
        }

        public static Composite HoT(string spell, UnitSelectionDelegate onUnit, int HP = 100, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && !onUnit(ret).HasMyBuff(spell) && onUnit(ret).HealthPercent <= HP),
                Cast(spell, onUnit, reqs));
        }

        public static Composite HealGround(string spell, CanRunDecoratorDelegate reqs = null)
        {
            return new Decorator(
                ret => (Targeting.AOEHealPoint != null && Targeting.AOEHealPoint != Vector3.Zero && (reqs == null || reqs(ret)) && Targeting.ShouldAOEHeal),
                CastOnGround(spell, ret => Targeting.AOEHealPoint, ret => true));
        }
        #endregion

        public static Composite BuffBehavior
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Sprint"),
                    Spell.Buff(Me.SelfBuffName()),
                    Rest.HandleRest);
            }
        }
    }

    public class ExpiringItem
    {
        public string item;
        public ulong targetGuid;
        Timer t;

        public ExpiringItem(string str, float milisecs, ulong g)
        {
            item = str;
            t = new Timer(milisecs);
            targetGuid = g;
            t.Elapsed += new ElapsedEventHandler(Elapsed_Event);
            t.Start();
        }
        private void Elapsed_Event(object sender, ElapsedEventArgs e)
        {
            item = "";
        }

    }
}
