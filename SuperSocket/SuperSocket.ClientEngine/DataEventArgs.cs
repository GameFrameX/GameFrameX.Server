using System;

namespace SuperSocket.ClientEngine
{
	// Token: 0x02000010 RID: 16
	public class DataEventArgs : EventArgs
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000038D6 File Offset: 0x00001AD6
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000038DE File Offset: 0x00001ADE
		public byte[] Data { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000038E7 File Offset: 0x00001AE7
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000038EF File Offset: 0x00001AEF
		public int Offset { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000038F8 File Offset: 0x00001AF8
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00003900 File Offset: 0x00001B00
		public int Length { get; set; }
	}
}
