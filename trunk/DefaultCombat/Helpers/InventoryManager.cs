using System.Diagnostics;
using System.Threading;
using Buddy.BehaviorTree;
using Buddy.Swtor;
using Buddy.Common;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Helpers
{
    public class IManItem
    {
        public delegate T Selection<out T>(object context);

        private Stopwatch Timer;
        public string ItemName;
        public int CoolDown;

        public IManItem(string name, int cd)
        {
            this.ItemName = name;
            this.CoolDown = cd;
            this.Timer = new Stopwatch();
            Logging.Write(name + "  Created!");
        }

        public Composite UseItem(Selection<bool> reqs = null)
        {
            return new Decorator(ret => (reqs == null || reqs(ret)),
                new Action(delegate
                {
                    if (Timer.IsRunning && Timer.Elapsed.TotalSeconds < CoolDown)
                        return RunStatus.Failure;

                    if (!Timer.IsRunning)
                        Timer.Start();

                    foreach (var o in BuddyTor.Me.InventoryEquipment)
                    {
                        if (o.Name.Contains(ItemName))
                        {
                            o.Use();
                            o.Interact();
                            Timer.Stop();
                            Timer.Reset();
                            Timer.Start();
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                }));
        }
    }
}
