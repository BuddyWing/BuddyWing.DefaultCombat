using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Buddy.BehaviorTree;
using Buddy.Common.Math;
using Buddy.CommonBot;
using Buddy.Swtor;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat
{
    public static class Spell
    {

        //
        public static Composite WaitForCast()
        {
            return new Decorator(ret=>BuddyTor.Me.IsCasting,
                new Action(ret=>RunStatus.Success));
        }

        public static Composite Cast(string spell)
        {
            return DefaultCombat.Instance.Cast(spell, ret => AbilityManager.HasAbility(spell));
        }

        public static Composite Cast(string spell, BooleanValueDelegate requirements)
        {
            return DefaultCombat.Instance.Cast(spell, requirements);
        }

        public static Composite Cast(string spell, CharacterSelectionDelegate onCharacter, BooleanValueDelegate requirements )
        {
            return DefaultCombat.Instance.Cast(spell, onCharacter, requirements);
        }

        public static Composite CastOnGround(string spell, BooleanValueDelegate requirements, CommonBehaviors.Retrieval<Vector3> location)
        {
            return
                new Decorator(
                    ret =>
                    requirements != null && requirements(ret) && location != null && location(ret) != Vector3.Zero &&
                    AbilityManager.CanCast(spell, BuddyTor.Me.CurrentTarget),
                    new Action(ret => AbilityManager.Cast(spell, location(ret))));
        }

        public static Composite Debuff(string spell)
        {
            return DefaultCombat.Instance.Debuff(spell);
        }

        public static Composite Debuff(string spell, BooleanValueDelegate requirements)
        {
            return new Decorator(
                ret => requirements != null && requirements(ret),
                Debuff(spell));
        }

        public static Composite Debuff(string spell, CharacterSelectionDelegate onCharacter, BooleanValueDelegate requirements)
        {
            return DefaultCombat.Instance.Debuff(spell, onCharacter, requirements);
        }



        public static Composite Buff(string spell)
        {
            return 
                new Decorator(ret => AbilityManager.HasAbility(spell), DefaultCombat.Instance.Buff(spell));
        }

        public static Composite Buff(string spell, BooleanValueDelegate requirements)
        {
            return new Decorator(
                ret => requirements != null && requirements(ret),
                Buff(spell));
        }

        public static Composite Buff(string spell, CharacterSelectionDelegate onCharacter)
        {
            return 
                new Decorator(ret => AbilityManager.HasAbility(spell), DefaultCombat.Instance.Buff(spell, onCharacter, ret => true));
        }

        public static Composite Buff(string spell, CharacterSelectionDelegate onCharacter, BooleanValueDelegate requirements)
        {
            return 
                new Decorator(ret => AbilityManager.HasAbility(spell), DefaultCombat.Instance.Buff(spell, onCharacter, requirements));
        }

        public static Composite BuffSelf(string spell)
        {
            return new Decorator(
                ret => !BuddyTor.Me.HasBuff(spell) && AbilityManager.HasAbility(spell),
                Cast(spell, ret => BuddyTor.Me, ret => true));
        }

        public static Composite BuffSelf(string spell, BooleanValueDelegate requirements)
        {
            return new Decorator(
                ret => requirements != null && requirements(ret) && AbilityManager.HasAbility(spell),
                BuffSelf(spell));
        }
    }
}
