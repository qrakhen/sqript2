using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Stack<T>
    {
        protected T[] __items;
        public int index { get; protected set; }
        public int length => __items.Length;
        public bool done => (index >= length);

        public T[] items => (T[])__items.Clone();

        public Stack(T[] items = null)
        {
            this.__items = (items?.Clone() as T[]);
        }

        public void set(T[] items)
        {
            this.__items = items.Clone() as T[];
        }

        public T peek(int delta = 0)
        {
            if (index + delta >= length)
                return default(T);

            return __items[index + delta];
        }

        public T digest()
        {
            if (done)
                throw new SqrError("stack is done, can not digest any further.");

            return __items[index++];
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
            var r = __items.AsSpan(index + from, amount).ToArray();
            index += amount;
            return r;
        }

        public T[] digestRange(int amount) => digestRange(index, amount);

        public delegate void ProcessCallback(
                Func<bool> condition,
                Func<T> current,
                Func<T> take,
                int index,
                Action abort);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">(currentFunc, index, abortFunc) => { currentFunc(); ...if (index > 0); ...abortFunc(); }</param>
        /// <param name="condition"></param>
        public void process(Action<Func<T>, Func<T>, int, Action> callback, Func<bool> condition = null)
        {
            int relativeIndex = 0;
            bool aborted = false;
            while (!aborted && !done && (condition != null ? condition() : true)) {
                callback(() => peek(), digest, relativeIndex++, () => aborted = true);
            }
        }

        public void process(Action callback, Func<bool> condition = null) => process((a, b, c, d) => callback(), condition);
        public void process(Action<int> callback, Func<bool> condition = null) => process((a, b, c, d) => callback(c), condition);
        public void process(Action<Action> callback, Func<bool> condition = null) => process((a, b, c, d) => callback(d), condition);
        public void process(Func<bool> condition, Action<Func<T>, Func<T>, int, Action> callback) => process(callback, condition);
    }
}
