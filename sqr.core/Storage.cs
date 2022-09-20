using System;
using System.Collections.Generic;

namespace Qrakhen.Sqr.Core
{
    public class Storage<K, T> : Dictionary<K, T>
    {
        private Dictionary<K, T> dict { get { return this; } }

        public bool locked { get; private set; }
        public int count { get { return Count; } }

        public new T this[K key] {
            get {
                if (ContainsKey(key)) return base[key];
                else return default(T);
            }
            set {
                if (locked)
                    throw new SqrError("storage is locked");
                if (ContainsKey(key)) base[key] = value;
                else Add(key, value);
            }
        }

        public Storage() : base()
        {

        }

        public Storage(Dictionary<K, T> data) : base(data)
        {

        }

        public bool contains(K key)
        {
            return ContainsKey(key);
        }

        public void set(K key, T value)
        {
            if (locked)
                throw new SqrError("storage is locked");
            if (contains(key)) base[key] = value;
            else Add(key, value);
        }

        public T get(K key)
        {
            if (contains(key)) return base[key];
            else return default(T);
        }

        public void remove(K key)
        {
            if (locked)
                throw new SqrError("storage is locked");
            if (ContainsKey(key)) Remove(key);
        }

        public void clear()
        {
            if (locked)
                throw new SqrError("storage is locked");
            Clear();
        }

        public void forEach(Action<T> callback)
        {
            foreach (var item in this)
                callback(item.Value);
        }

        public void forEach(Action<K, T> callback)
        {
            foreach (var item in dict)
                callback(item.Key, item.Value);
        }

        public int removeOne(Func<T, bool> callback) => removeAll(callback, 1);

        public int removeAll(Func<T, bool> callback, int limit = 0)
        {
            if (locked)
                throw new SqrError("storage is locked");

            int count = 0;
            var list = new List<K>();
            foreach (var item in dict)
                if (callback(item.Value)) {
                    list.Add(item.Key);
                    count++;
                    if (limit > 0 && count >= limit) break;
                }

            foreach (var key in list)
                base.Remove(key);

            return count;
        }

        public T findOne(Func<T, bool> callback)
        {
            foreach (var item in Values)
                if (callback(item)) return item;
            return default(T);
        }

        public T[] findAll(Func<T, bool> callback)
        {
            List<T> items = new List<T>();
            foreach (var item in this.Values)
                if (callback(item)) items.Add(item);
            return items.ToArray();
        }

        public List<T> getAll()
        {
            return new List<T>(Values);
        }

        public List<K> getKeys()
        {
            return new List<K>(Keys);
        }

        public void copyFrom(Storage<K, T> source)
        {
            clear();
            source.forEach((k, v) => this[k] = v);
        }

        public void lockStorage()
        {
            locked = true;
        }
    }
}
