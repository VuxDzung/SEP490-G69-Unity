namespace SEP490G69.Addons.LoadScreenSystem
{
    using System;
    using System.Collections;

    public class LoadTask
    {
        public string name;
        public Func<IEnumerator> task;

        public LoadTask(string name, Func<IEnumerator> task)
        {
            this.name = name;
            this.task = task;
        }
    }
}