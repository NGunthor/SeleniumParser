using System;
using System.Collections.Generic;
using System.Threading;

namespace VKApp
{
    public class ThreadManager
    {
        public List<Thread> Threads { get; private set; }

        public ThreadManager(Manager manager)
        {
            var actions = new List<Action> {manager.FindLinks, manager.FindTexts, manager.FindImages, Reader.RunReading};
            RunActions(actions);
        }

        private void RunActions(List<Action> actions)
        {
            Threads = new List<Thread>();
            foreach (var action in actions)
                Threads.Add(new Thread(() => action())); 
            StartThreads();
            
        }

        private void StartThreads() => Threads.ForEach(action => action.Start());
    }
}