using System.Collections;
using System.Collections.Generic;

namespace CodeHelpers.Collections
{
    public class UniqueQueue<T> : Queue<T>
    {
        readonly HashSet<T> items;

        public UniqueQueue()
        {
            items = new HashSet<T>();
        }

        public new bool Contains(T item)
        {
            return items.Contains(item);
        }

        public new void Enqueue(T item)
        {
            if (items.Add(item)) base.Enqueue(item);
        }

        public new T Dequeue()
        {
            T item = base.Dequeue();
            items.Remove(item);

            return item;
        }
    }
}