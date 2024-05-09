using System;
using System.Collections.Generic;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000007 RID: 7
    public interface IBatchQueue<T>
    {
        // Token: 0x06000038 RID: 56
        bool Enqueue(T item);

        // Token: 0x06000039 RID: 57
        bool Enqueue(IList<T> items);

        // Token: 0x0600003A RID: 58
        bool TryDequeue(IList<T> outputItems);

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600003B RID: 59
        bool IsEmpty { get; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600003C RID: 60
        int Count { get; }
    }
}