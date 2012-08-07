using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Buddy.BehaviorTree;
using Buddy.Common.Math;
using Buddy.CommonBot;
using Buddy.CommonBot.Logic;
using Buddy.Navigation;
using Buddy.Swtor;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat
{
    static class Movement
    {
        public static Composite MoveToLineOfSight()
        {
            return new Decorator(
                ret => !BuddyTor.IsInLineOfSight(BuddyTor.Me, BuddyTor.Me.CurrentTarget),
                CommonBehaviors.MoveToLos(ret => BuddyTor.Me.CurrentTarget, true)
                );
            
        }

        public static Composite StopInRange(float range)
        {
            return new PrioritySelector(
                new Decorator(                    
                    ret => BuddyTor.Me.CurrentTarget.Distance <= range && BuddyTor.IsInLineOfSight(BuddyTor.Me, BuddyTor.Me.CurrentTarget),
                    new Sequence(
                        CommonBehaviors.MoveStop(),
                        // Force failure for pass-through on PrioritySelector.
                        new Action(ret => RunStatus.Failure))));
            
            
        }


        public static Composite MoveTo(CommonBehaviors.Retrieval<Vector3> position, float range)
        {
            return CommonBehaviors.MoveAndStop(position, range, true, "Target Position");
        }
        
        /*public static Composite MoveBehind(CommonBehaviors.Retrieval<Vector3> position, float range)
        {
            BuddyTor.Me.CurrentTarget.fa
            return CommonBehaviors.MoveAndStop(position, range, true, 
        }*/
    }
}
