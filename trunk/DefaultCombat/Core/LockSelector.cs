// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.Swtor;

namespace DefaultCombat.Core
{
    // Use this type sparingly - nesting locks will be handled by the memory library implicitly:
    // acquiring a single lock for the entire duration of a tick will have the same effect as acquiring 20
    // locks by using LockSelector in various locations. More locking does therefore not guarantee "more" 
    // consistency between the game's state. If you decide to use this type, use it in the outermost part of your logic,
    // so that all children of the selector will run inside the lock, but if possible, refrain from replacing every
    // single PrioritySelector with a LockSelector - it has proven to be detriment to performance for no tangible gains.
    public class LockSelector : PrioritySelector
    {
        public LockSelector(params Composite[] children)
            : base(children)
        {
        }

        public override RunStatus Tick(object context)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                return base.Tick(context);
            }
        }
    }
}