using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000003 RID: 3
    public class ConcurrentBatchQueue<T> : IBatchQueue<T>
    {
        // Token: 0x06000015 RID: 21 RVA: 0x00002821 File Offset: 0x00000A21
        public ConcurrentBatchQueue() : this(16)
        {
        }

        // Token: 0x06000016 RID: 22 RVA: 0x0000282B File Offset: 0x00000A2B
        public ConcurrentBatchQueue(int capacity) : this(new T[capacity])
        {
        }

        // Token: 0x06000017 RID: 23 RVA: 0x00002839 File Offset: 0x00000A39
        public ConcurrentBatchQueue(int capacity, Func<T, bool> nullValidator) : this(new T[capacity], nullValidator)
        {
        }

        // Token: 0x06000018 RID: 24 RVA: 0x00002848 File Offset: 0x00000A48
        public ConcurrentBatchQueue(T[] array) : this(array, (T t) => t == null)
        {
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00002870 File Offset: 0x00000A70
        public ConcurrentBatchQueue(T[] array, Func<T, bool> nullValidator)
        {
            this.m_Entity = new ConcurrentBatchQueue<T>.Entity
            {
                Array = array
            };
            this.m_BackEntity = new ConcurrentBatchQueue<T>.Entity();
            this.m_BackEntity.Array = new T[array.Length];
            this.m_NullValidator = nullValidator;
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000028BC File Offset: 0x00000ABC
        public bool Enqueue(T item)
        {
            bool flag;
            while (!this.TryEnqueue(item, out flag) && !flag)
            {
            }

            return !flag;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000028DC File Offset: 0x00000ADC
        private bool TryEnqueue(T item, out bool full)
        {
            full = false;
            ConcurrentBatchQueue<T>.Entity entity = this.m_Entity as ConcurrentBatchQueue<T>.Entity;
            T[] array = entity.Array;
            int count = entity.Count;
            if (count >= array.Length)
            {
                full = true;
                return false;
            }

            if (entity != this.m_Entity)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref entity.Count, count + 1, count) != count)
            {
                return false;
            }

            array[count] = item;
            return true;
        }

        // Token: 0x0600001C RID: 28 RVA: 0x0000293C File Offset: 0x00000B3C
        public bool Enqueue(IList<T> items)
        {
            bool flag;
            while (!this.TryEnqueue(items, out flag) && !flag)
            {
            }

            return !flag;
        }

        // Token: 0x0600001D RID: 29 RVA: 0x0000295C File Offset: 0x00000B5C
        private bool TryEnqueue(IList<T> items, out bool full)
        {
            full = false;
            ConcurrentBatchQueue<T>.Entity entity = this.m_Entity as ConcurrentBatchQueue<T>.Entity;
            T[] array = entity.Array;
            int count = entity.Count;
            int count2 = items.Count;
            int num = count + count2;
            if (num > array.Length)
            {
                full = true;
                return false;
            }

            if (entity != this.m_Entity)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref entity.Count, num, count) != count)
            {
                return false;
            }

            foreach (T t in items)
            {
                array[count++] = t;
            }

            return true;
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002A04 File Offset: 0x00000C04
        public bool TryDequeue(IList<T> outputItems)
        {
            ConcurrentBatchQueue<T>.Entity entity = this.m_Entity as ConcurrentBatchQueue<T>.Entity;
            if (entity.Count <= 0)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref this.m_Entity, this.m_BackEntity, entity) != entity)
            {
                return false;
            }

            SpinWait spinWait = default(SpinWait);
            spinWait.SpinOnce();
            int count = entity.Count;
            T[] array = entity.Array;
            int num = 0;
            for (;;)
            {
                T t = array[num];
                while (this.m_NullValidator(t))
                {
                    spinWait.SpinOnce();
                    t = array[num];
                }

                outputItems.Add(t);
                array[num] = ConcurrentBatchQueue<T>.m_Null;
                if (entity.Count <= num + 1)
                {
                    break;
                }

                num++;
            }

            entity.Count = 0;
            this.m_BackEntity = entity;
            return true;
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600001F RID: 31 RVA: 0x00002ABC File Offset: 0x00000CBC
        public bool IsEmpty
        {
            get { return this.Count <= 0; }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000020 RID: 32 RVA: 0x00002ACA File Offset: 0x00000CCA
        public int Count
        {
            get { return ((ConcurrentBatchQueue<T>.Entity)this.m_Entity).Count; }
        }

        // Token: 0x04000006 RID: 6
        private object m_Entity;

        // Token: 0x04000007 RID: 7
        private ConcurrentBatchQueue<T>.Entity m_BackEntity;

        // Token: 0x04000008 RID: 8
        private static readonly T m_Null;

        // Token: 0x04000009 RID: 9
        private Func<T, bool> m_NullValidator;

        // Token: 0x0200001F RID: 31
        private class Entity
        {
            // Token: 0x17000034 RID: 52
            // (get) Token: 0x06000123 RID: 291 RVA: 0x000053B9 File Offset: 0x000035B9
            // (set) Token: 0x06000124 RID: 292 RVA: 0x000053C1 File Offset: 0x000035C1
            public T[] Array { get; set; }

            // Token: 0x04000046 RID: 70
            public int Count;
        }
    }
}