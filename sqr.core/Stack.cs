using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Stack<T>
    {
        protected T[] items;
        public int index { get; protected set; }
        public int length => items.Length;
        public bool done => (index >= length);

        public Stack(T[] items = null)
        {
            this.items = (items?.Clone() as T[]);
        }

        public void set(T[] items)
        {
            this.items = items.Clone() as T[];
        }

        public T peek(int delta = 0)
        {
            if (index + delta >= length)
                return default(T);

            return items[index + delta];
        }

        public T digest()
        {
            if (done)
                throw new SqrError("stack is done, can not digest any further.");

            return items[index++];
        }

        public T[] digestUntil(T value)
        {
            List<T> buffer = new List<T>();
            while (!peek().Equals(value) && !done) {
                buffer.Add(digest());
            }
            return buffer.ToArray();
        }
             
        public T[] digestRange(int from, int amount)
        {
            var r = items.AsSpan(index + from, amount).ToArray();
            index += amount;
            return r;
        }

        public T[] digestRange(int amount) => digestRange(index, amount);

        public void process(Action<Func<T>> callback, Func<bool> condition = null)
        {
            while (!done && (condition != null ? condition() : true)) {
                callback(() => peek());
            }
        }

        public void process(Func<bool> condition, Action<Func<T>> callback) => process(callback, condition);
    }
}
