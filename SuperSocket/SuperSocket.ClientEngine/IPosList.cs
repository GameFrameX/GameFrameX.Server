using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000009 RID: 9
    public interface IPosList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000040 RID: 64
        // (set) Token: 0x06000041 RID: 65
        int Position { get; set; }
    }
}