﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// See the file LICENSE for the source code's detailed license

using System.Diagnostics;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.Swtor;
using log4net;

namespace DefaultCombat.Helpers
{
    public class InventoryManagerItem
    {
        public delegate T Selection<out T>(object context);

        private readonly ILog _log = Log.Get();

        private readonly Stopwatch _timer;
        public int CoolDown;
        public string ItemName;

        public InventoryManagerItem(string name, int cd)
        {
            ItemName = name;
            CoolDown = cd;
            _timer = new Stopwatch();
            _log.Info(name + "  Created!");
        }

        public Composite UseItem(Selection<bool> reqs = null)
        {
            return new Decorator(ret => reqs == null || reqs(ret),
                new Action(delegate
                {
                    if (_timer.IsRunning && _timer.Elapsed.TotalSeconds < CoolDown)
                        return RunStatus.Failure;

                    if (!_timer.IsRunning)
                        _timer.Start();

                    foreach (var o in BuddyTor.Me.InventoryEquipment)
                    {
                        if (o.Name.Contains(ItemName))
                        {
                            o.Use();
                            o.Interact();
                            _timer.Stop();
                            _timer.Reset();
                            _timer.Start();
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                }));
        }
    }
}