using System;

namespace SuperSocket.ClientEngine
{
	// Token: 0x0200000C RID: 12
	public class SearchMarkState<T> where T : IEquatable<T>
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002FD9 File Offset: 0x000011D9
		public SearchMarkState(T[] mark)
		{
			this.Mark = mark;
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002FE8 File Offset: 0x000011E8
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002FF0 File Offset: 0x000011F0
		public T[] Mark { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002FF9 File Offset: 0x000011F9
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00003001 File Offset: 0x00001201
		public int Matched { get; set; }
	}
}
