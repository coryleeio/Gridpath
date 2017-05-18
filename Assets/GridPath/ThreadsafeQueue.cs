using System.Collections.Generic;

namespace Assets.GridPath
{
    public class ThreadsafeQueue<TStored>
    {
        private Queue<TStored> unsafeQueue = new Queue<TStored>();
        private object _sync = new object();

        public void Enqueue(TStored value)
        {
            lock (_sync)
            {
                unsafeQueue.Enqueue(value);
            }
        }

        public TStored Dequeue()
        {
            lock (_sync)
            {
                return unsafeQueue.Dequeue();
            }
        }

        public bool Contains(TStored value)
        {
            lock (_sync)
            {
                return unsafeQueue.Contains(value);
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                unsafeQueue.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_sync)
                {
                    return unsafeQueue.Count;
                }
            }
        }
    }
}
