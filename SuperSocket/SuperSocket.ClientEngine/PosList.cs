using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperSocket.ClientEngine
{
	// Token: 0x0200000A RID: 10
	public class PosList<T> : List<T>, IPosList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002F33 File Offset: 0x00001133
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002F3B File Offset: 0x0000113B
		public int Position { get; set; }
	}
}
